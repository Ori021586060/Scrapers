using Flurl.Http;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using ScraperCore;
using ScraperModels.Models;
using ScraperModels.Models.Excel;
using ScraperModels.Models.OnmapDto;
using ScraperServices.Models.Onmap;
using ScraperServices.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScraperModels;
using System.Text.RegularExpressions;

namespace ScraperServices.Scrapers
{
    public class ScraperOnmap: ScraperBase
    {
        private ScraperOnmapConfigModel _config { get; set; }
        private ScraperOnmapStateModel _state { get; set; } = new ScraperOnmapStateModel();
        private int _usedSelenoidService { get; set; } = 0;
        private SelenoidStateModel _selenoidState { get; set; }

        public ScraperOnmap(ScraperOnmapStateModel state = null)
        {
            if (state is null) state = new ScraperOnmapStateModel();
            _state = state;
            _initConfig(state);
        }

        public ExcelOnmapService GetExcelService()
        {
            return new ExcelOnmapService(_state);
        }

        private void _initConfig(ScraperOnmapStateModel state)
        {
            var filename = $"{state.ConfigFilename}";

            if (File.Exists(filename))
                _config = JsonConvert.DeserializeObject<ScraperOnmapConfigModel>(File.ReadAllText(filename));
            else
            {
                _config = new ScraperOnmapConfigModel();
                _saveConfig(_config, filename);
            }
        }

        private void _saveConfig(ScraperOnmapConfigModel config, string filename)
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        protected override void _checkWorkspaceInner(IState state)
        {
            _checkDirectory($"{state.RootPath}");

            _checkDirectory($"{state.ItemsPath}");
        }

        protected override async Task<bool> ScrapeInnerAsync()
        {
            var result = false;

            try
            {
                var state = _state;

                if (state.IsNew) _clearWorkspace(state);

                _checkWorkspace(state);

                ScrapePhase1(state);

                await ScrapePhase1_GenerateListItemsAsync(state);

                //await ScrapePhase2_Selenoid(state);

                await ScrapePhase2_WebClient(state);

                result = true;
            }
            catch (Exception exception) {
                _log($"ERROR-ScraperInner-01. {exception.Message}");
            }

            return result;
        }

        private async Task ScrapePhase2_WebClient(ScraperOnmapStateModel state)
        {
            SetWorkPhaseBase($"Phase-2/W", state);

            var listItems = await _loadListItemsAsync(state);
            await fixListItemsByPathAsync(listItems, state);

            var maxTasks = 50;
            var tasks = new List<Task<bool>>();
            Func<int,List<string>> NeedToDo = (i) => listItems.Where(x => x.Value == false).Select(x => x.Key).Take(i).ToList();
            var needToDo = NeedToDo(maxTasks);

            do
            {
                foreach(var itemId in needToDo)
                {
                    listItems[itemId] = true;
                    tasks.Add(Task.Run(async()=> listItems[itemId] = await DownloadItem_WebClient(itemId, state)));
                }

                Thread.Sleep(1000 * 1);
                Task.WaitAny(tasks.ToArray());

                tasks.RemoveAll(x => x.IsCompleted);

                needToDo = NeedToDo(maxTasks - tasks.Count());

            } while (needToDo.Count() > 0);

            Task.WaitAll(tasks.ToArray());
        }

        private async Task<bool> DownloadItem_WebClient(string itemId, ScraperOnmapStateModel state)
        {
            var result = false;

            try
            {
                var page = await DownloadPage_WebClient(itemId);
                var json = ParseObjectFromPage(page);
                var item = DeserializeJson(json);
                await SaveItemToStore(itemId, item.propertyDetails.data, state);

                result = true;
            }catch (Exception exception)
            {
                _log($"Error p1. {exception.Message}");
            }

            return result;
        }

        private async Task<string> DownloadPage_WebClient(string itemId)
        {
            var page = "";
            var url = $"https://www.onmap.co.il/home_details/{itemId}";
            
            try
            {
                page = await url
                    .WithHeaders(new
                    {
                        User_Agent = "Wow browsaer",
                    })
                    .GetStringAsync();
            }
            catch (Exception exception)
            {
                _log($"Error x4. {exception.Message}");
            }

            return page;
        }

        private async Task SaveItemToStore(string itemId, object item, ScraperOnmapStateModel state)
        {
            var filename = $"{state.ItemsPath}/{itemId}.json";

            await File.WriteAllTextAsync(filename, JsonConvert.SerializeObject(item, Formatting.Indented));

            _log($"Save item into file {filename}");
        }

        private Phase3ItemDto DeserializeJson(string json)
        {
            var obj = JsonConvert.DeserializeObject<Phase3ItemDto>(json);

            return obj;
        }

        private string ParseObjectFromPage(string page)
        {
            var result = "";
            Regex regex = new Regex(@"window\.__data=(.*)}};<");
            MatchCollection matches = regex.Matches(page);
            if (matches.Count > 0)
            {
                result = CleanDataFromPage(matches[0].Value);
            }

            return result;
        }

        private string CleanDataFromPage(string value)
        {
            var result = value.Replace("window.__data=", "").Replace(";<", "");

            return result;
        }

        private async Task ScrapePhase1_GenerateListItemsAsync(ScraperOnmapStateModel state)
        {
            SetWorkPhaseBase($"ScrapePhase1_GenerateListItems", state);

            var listObjects = _loadListObjects(state);

            var list = new Dictionary<string, bool>();
            var duplicates = 0;

            foreach(var obj in listObjects)
            {
                if (!list.ContainsKey(obj.Key))
                    list.Add(obj.Key, false);
                else
                    duplicates++;
            }
            _log($"Has {duplicates} duplicates");

            await _saveListItemsAsync(list, state);
            _log("Done");
        }

        private async Task _saveListItemsAsync(Dictionary<string, bool> list, ScraperOnmapStateModel state)
        {
            var filename = $"{state.PathListItems}";
            await File.WriteAllTextAsync(filename, JsonConvert.SerializeObject(list, Formatting.Indented));
        }

        private async Task<Dictionary<string, bool>> _loadListItemsAsync(ScraperOnmapStateModel state)
        {
            Dictionary<string, bool> result = null;
            var filename = $"{state.PathListItems}";
            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<Dictionary<string, bool>>(await File.ReadAllTextAsync(filename));

            return result;
        }

        public async override Task<DataScrapeModel> GetDomainModelAsync()
        {
            var state = _state;
            SetWorkPhaseBase($"DomainModel", state);
            var domainModel = ScrapePhase3Async(state);

            var result = new DataScrapeModel()
            {
                Scraper = EnumScrapers.Onmap,
                Data = await domainModel,
            };

            LogDone(state);

            return result;
        }

        protected override void _clearWorkspaceInner(IState state)
        {
            _cleanDirectory($"{state.RootPath}", state);
            //var files = new DirectoryInfo(state.RootPath).GetFiles();

            //foreach (var dir in new DirectoryInfo(state.RootPath).GetDirectories()) dir.Delete(recursive:true);

            //foreach (var file in files) File.Delete(file.FullName);

            _cleanFile($"{state.LogFilename}");

        }

        private void _log(string message, IState state = null)
        {
            _logBase(message, state is null ? _state : state);
        }

        private void _cleanFile(string filename)
        {
            if (File.Exists(filename)) File.Delete(filename);
        }

        public void ScrapePhase1(ScraperOnmapStateModel state)
        {
            state.WorkPhase = "Phase-1";
            CultureInfo ci = new CultureInfo("en-US", true);
            Thread.CurrentThread.CurrentCulture = ci;

            if (!File.Exists(state.Phase1Filename))
            {
                _log($"Phase-1 start");

                Dictionary<string, DataRow> list = new Dictionary<string, DataRow>();

                var url = "";
                var foundCities = 0;

                var z8 = new Scale() { Height = 3.6, Width = 7.2 };
                var z9 = new Scale().Half(z8);
                var z10 = new Scale().Half(z9);
                var z11 = new Scale().Half(z10);
                var z12 = new Scale().Half(z11);
                var z13 = new Scale().Half(z12);
                var z14 = new Scale().Half(z13);
                var z15 = new Scale().Half(z14);
                var z16 = new Scale().Half(z15);

                // z8+z13 - 46 iter.stop
                // z8+z14 - 456 - all ok = 1820
                // z8+z15 - 
                // z15 - all ok
                var ws = z8;
                var currentLine = true;
                var iteration = 0;
                Point wp = BorderModel.M1.Clone();
                wp.SetZ(ws);

                do
                {
                    do
                    {
                        url = $"https://phoenix.onmap.co.il/v1/properties/search?option=rent,rent-short&section=residence" +
                            $"&loc[]={wp.Latitude}&loc[]={wp.Longitude}&loc[]={wp.Latitude + ws.Height / 2}&loc[]={wp.Longitude + ws.Width / 2}";

                        var response = url.GetJsonAsync<OnmapDto>();
                        var result = response.Result;

                        if (result.data.Count > 299)
                        {
                            // need scale
                            _log("Please need scale");
                        }

                        if (result.data.Count > 0)
                        {
                            foundCities += result.data.Count;
                            _log($"Found {foundCities} cities");
                        }

                        foreach (var row in result.data)
                            if (!list.ContainsKey(row.id)) list.Add(row.id, row);

                        wp.ToRight();

                        var outRightBorder = wp.OutRightBorder();
                        currentLine = !outRightBorder;
                        iteration++;

                    } while (currentLine);

                    ws = z14;
                    wp.ToNextLine(ws);

                    var outTopBorder = wp.OutTopBorder();

                    currentLine = !outTopBorder;
                } while (currentLine);

                File.WriteAllText(state.Phase1Filename, JsonConvert.SerializeObject(list, Formatting.Indented));

                _log($"Done");
            }
            else
            {
                _log($"Phase-1 has data, no generate new data");
            }
        }

        public async Task ScrapePhase2_Selenoid(ScraperOnmapStateModel state)
        {
            SetWorkPhaseBase($"Phase-2", state);

            var listItems = await _loadListItemsAsync(state);
            await fixListItemsByPathAsync(listItems, state);
                var needToDo = listItems.Where(x => x.Value == false).Select(x => x.Key).ToList();

                _initSelenoid(state);

                var list = new Dictionary<string, object>();
                var limitOpenPage = 50;
                var indexOpenedPage = 0;
                var i = 0;
                var doNeedRepeatRequest = false;

                foreach (var key in needToDo)
                {
                    if (indexOpenedPage > limitOpenPage)
                    {
                        await _saveListItemsAsync(listItems, state);
                        Thread.Sleep(1000 * 20);
                        indexOpenedPage = 0;
                    }

                    do
                    {
                        doNeedRepeatRequest = false;
                        var url = $"https://www.onmap.co.il/home_details/{key}";
                        try
                        {
                        _selenoidState.WindowMain.Navigate().GoToUrl(url);

                        _selenoidState.WaitMain.Until(ExpectedConditions.ElementIsVisible(By.ClassName("icon-square")));
                            //Thread.Sleep(1000 * 5);
                        }
                        catch
                        {
                            _log("!!! Need Reinit Selenoid !!!");
                            _initSelenoid(state);
                            doNeedRepeatRequest = true;
                        }
                    } while (doNeedRepeatRequest);

                    var netObjects = (Dictionary<string, object>)_selenoidState.WindowMain.ExecuteScript("return window.__data.propertyDetails.data;");

                    _saveItem(netObjects, key, state);
                    listItems[key] = true;
                    indexOpenedPage++;
                    i++;
                    _log($"add file items/{key}.json");
                }

            await _saveListItemsAsync(listItems, state);
            _log($"Done");
        }

        private async Task fixListItemsByPathAsync(Dictionary<string, bool> listItems, ScraperOnmapStateModel state)
        {
            var itemFiles = GetListItemFiles(state);
            var amountFixedFiles = 0;

            foreach(var itemFile in itemFiles)
            {
                var itemId = Path.GetFileNameWithoutExtension(itemFile.Name);
                if (listItems.ContainsKey(itemId) && listItems[itemId] == false)
                {
                    listItems[itemId] = true;
                    amountFixedFiles++;
                }
            }

            _log($"Fixed {amountFixedFiles} files");

            await _saveListItemsAsync(listItems, state);
        }

        private void _saveItem(Dictionary<string, object> netObjects, string key, ScraperOnmapStateModel state)
        {
            var filename = $"{state.ItemsPath}/{key}.json";
            File.WriteAllText(filename, JsonConvert.SerializeObject(netObjects, Formatting.Indented));
        }

        private Dictionary<string, DataRow> _loadListObjects(ScraperOnmapStateModel state)
        {
            Dictionary<string, DataRow> result = null;

            var filename = state.Phase1Filename;
            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<Dictionary<string, DataRow>>(File.ReadAllText(filename));

            return result;
        }

        private void _initSelenoid(ScraperOnmapStateModel state)
        {
            if (_selenoidState is null) _selenoidState = new SelenoidStateModel();

            _initSelenoidBase(_selenoidState, state);
        }

        public async Task<List<ExcelRowOnmapModel>> ScrapePhase3Async(ScraperOnmapStateModel state)
        {
            var listRowsDomainModel = new List<ExcelRowOnmapModel>();
            var files = GetListItemFiles(state);

            foreach (var itemFile in files)
            {
                var dto = LoadDtoItemFromPathAsync(itemFile);
                var rowDomainModel = new ExcelRowOnmapModel().FromDto(await dto);
                listRowsDomainModel.Add(rowDomainModel);
            }

            return listRowsDomainModel;
        }

        private async Task<Phase3ObjectDto> LoadDtoItemFromPathAsync(FileInfo itemFile)
        {
            Phase3ObjectDto result = null;
            var filename = $"{itemFile.FullName}";

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<Phase3ObjectDto>(await File.ReadAllTextAsync(filename));

            return result;
        }

        public ScraperOnmapStatusModel StatusWorkspace()
        {
            var state = _state;

            state.WorkPhase = "StatusWorkspace";

            _log("Start");

            var scrapeDate = _statusWorkspace_ScrapeDateBase(state);
            var amountItemsFromPath = _statusWorkspace_AmountItemsFromPathBase(state);
            //var amountPages = _statusWorkspace_AmountPagesBase(state);
            //var amountItemsFromPages = _statusWorkspace_AmountItemsFromPages(state);
            //var amountItemDuplicatesFromPages = _statusWorkspace_AmountItemDuplicatesFromPages(state);

            var status = new ScraperOnmapStatusModel()
            {
                ScrapeDate = scrapeDate,
                AmountItemsFromPath = amountItemsFromPath,
                //AmountPages = amountPages,
                //AmountItemsFromPages = amountItemsFromPages,
                //AmountItemUniquesFromPages = amountItemDuplicatesFromPages,
            };

            _log("End");

            return status;
        }

        private int _statusWorkspace_AmountItemsFromPages(ScraperOnmapStateModel state)
        {
            var list = _statusWorkspace_AmountItemsFromPages_GetItems(state);

            return list.Count();
        }

        private List<ItemTest> _statusWorkspace_AmountItemsFromPages_GetItems(ScraperOnmapStateModel state)
        {
            var listPages = _statusWorkspace_AmountPages_GetFilesBase(state);

            var totalItems = 0;
            var list = new List<ItemTest>();

            //foreach (var page in listPages)
            //{
            //    var filename = page.FullName;
            //    var pageData = JsonConvert.DeserializeObject<Dictionary<string, bool>>(File.ReadAllText(filename));
            //    var listItems = pageData.Select(x => new ItemTest() { Id = x.Key, Done = x.Value }).ToList();
            //    list.AddRange(listItems);
            //    totalItems += listItems.Count;
            //}

            return list;
        }

        private int _statusWorkspace_AmountItemDuplicatesFromPages(ScraperOnmapStateModel state)
        {
            var list = _statusWorkspace_AmountItemsFromPages_GetItems(state);

            var dups = list.GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.First().Id, x => x.First().Done);

            return dups.Count();
        }

        public void PrintStatus(ScraperOnmapStatusModel status)
        {
            _log($"ScrapeDate: {status.ScrapeDate}");
            _log($"AmountItemsFromPath: {status.AmountItemsFromPath}");
            _log($"AmountPages: {status.AmountPages}");
            _log($"AmountItemsFromPages: {status.AmountItemsFromPages}");
            _log($"AmountItemDuplicatesFromPages: {status.AmountItemDuplicatesFromPages}");
            _log($"AmountItemUnicsFromPages: {status.AmountItemUniquesFromPages}");
        }

        public bool SaveStatus(object status)
        {
            var result = SaveStatusBase(status, _state);

            return result;
        }
    }
}
