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
using ScrapModels;
using ScrapModels.Models;
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

        //private void _checkDirectory(string directory)
        //{
        //    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        //}

        protected override async Task<bool> ScrapeInner()
        {
            var result = false;

            try
            {
                var state = _state;

                if (state.IsNew) _clearWorkspace(state);

                _checkWorkspace(state);

                ScrapePhase1(state);

                ScrapePhase1_GenerateListItems(state);

                ScrapePhase2(state);

                result = true;
            }
            catch (Exception exception) {
                _log($"ERROR-ScraperInner-01");
            }

            return result;
        }

        private void ScrapePhase1_GenerateListItems(ScraperOnmapStateModel state)
        {
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

            _saveListItems(list, state);
        }

        private void _saveListItems(Dictionary<string, bool> list, ScraperOnmapStateModel state)
        {
            var filename = $"{state.PathListItems}";
            File.WriteAllText(filename, JsonConvert.SerializeObject(list, Formatting.Indented));
        }

        private Dictionary<string, bool> _loadListItems(ScraperOnmapStateModel state)
        {
            Dictionary<string, bool> result = null;
            var filename = $"{state.PathListItems}";
            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<Dictionary<string, bool>>(File.ReadAllText(filename));

            return result;
        }

        public DataScrapeModel GetDomainModel()
        {
            var list = ScrapePhase3(_state);

            var result = new DataScrapeModel()
            {
                Scraper = EnumScrapers.Onmap,
                Data = list,
            };

            return result;
        }

        protected override void _clearWorkspaceInner(IState state)
        {
            _log($"Start clearing workspace");

            var files = new DirectoryInfo(state.RootPath).GetFiles();

            foreach (var dir in new DirectoryInfo(state.RootPath).GetDirectories()) dir.Delete(recursive:true);

            foreach (var file in files) File.Delete(file.FullName);

            _cleanFile($"{state.LogFilename}");

            _log($"Clear workspace done");
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
            state.WorkPhase = "phase-1";
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

                _log($"Phase-1 done");
            }
            else
            {
                _log($"Phase-1 has data, no generate new data");
            }
        }

        public void ScrapePhase2(ScraperOnmapStateModel state)
        {
            state.WorkPhase = "phase-2";
                _log($"Start");

                //var listObjects = JsonConvert.DeserializeObject<Dictionary<string, DataRow>>(File.ReadAllText(state.Phase1Filename));
                //var listObjects = _loadListObjects(state);
                var listItems = _loadListItems(state);
            _checkListItemsByPath(listItems, state);
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
                        _saveListItems(listItems, state);
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

                    //list.Add(key, netObjects);
                    _saveItem(netObjects, key, state);
                    listItems[key] = true;
                    indexOpenedPage++;
                    i++;
                    _log($"add file items/{key}.json");
                    //_log($"Index ad {i}, id:{listObjects.Where(x=>x.Key==key).FirstOrDefault().Value.id}");
                }

            //File.WriteAllText(state.Phase2Filename, JsonConvert.SerializeObject(list, Formatting.Indented));
            _saveListItems(listItems, state);
            _log($"Done");
            //else
            //{
            //    _log($"Phase-2 has data, no generate new data");
            //}
        }

        private void _checkListItemsByPath(Dictionary<string, bool> listItems, ScraperOnmapStateModel state)
        {
            var itemFiles = _loadItemsFromPath(state);
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

            _saveListItems(listItems, state);
        }

        private FileInfo[] _loadItemsFromPath(ScraperOnmapStateModel state)
        {
            var path = $"{state.ItemsPath}";
            var itemFiles = new DirectoryInfo(path).GetFiles();

            return itemFiles;
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

        //private void _initSelenoid(ScraperOnmapStateModel state)
        //{
        //    var options = new FirefoxOptions();
        //    options.SetPreference("permissions.default.image", 2);
        //    //options.SetPreference("javascript.enabled", false);

        //    bool doNeedRepeatRequest = false;
        //    int countRepeat = 0;
        //    int countRepeatMax = 10;

        //    do
        //    {
        //        var selenoidService = _selectSelenoidService(state);
        //        var selenoidUrl = $"{selenoidService.Protocol}://{selenoidService.Address}:4444/wd/hub";
        //        _log($"\tTry use SelenoidService-{_usedSelenoidService}:{selenoidUrl}");

        //        try
        //        {
        //            if (_windowMain != null)
        //            {
        //                _log($"SelenoidService isnt null");
        //                _windowMain.Quit();
        //                _log($"Quit SelenoidService done");
        //            }

        //            _printSelenoidServiceParams(selenoidService);

        //            doNeedRepeatRequest = false;

        //            _windowMain = new RemoteWebDriver(new Uri(selenoidUrl), options.ToCapabilities(), TimeSpan.FromMinutes(5));

        //            _waitMain = new WebDriverWait(_windowMain, TimeSpan.FromSeconds(120));
        //        }
        //        catch
        //        {
        //            countRepeat++;
        //            if (countRepeat < countRepeatMax)
        //            {
        //                doNeedRepeatRequest = true;
        //                _log($"\t!!! Need change Selenoid Service !!!");
        //                _log($"\tBad config SelenoidService-{_usedSelenoidService}:");
        //                _printSelenoidServiceParams(selenoidService);
        //            }
        //        }

        //        if (doNeedRepeatRequest)
        //        {
        //            _log($"\tcountRepeat:{countRepeat}");
        //            _usedSelenoidService++;
        //            //Thread.Sleep(TimeSpan.FromMinutes(10));
        //        }

        //    } while (doNeedRepeatRequest);

        //}

        //private void _printSelenoidServiceParams(SelenoidConfigModel service)
        //{
        //    _log($"\tParams Selenoid:");
        //    _log($"\t\tDns:{service.Address}");
        //    _log($"\t\tIp:{Dns.GetHostAddresses(service.Address).FirstOrDefault()}");
        //    _log($"\t\tProtocol:{service.Protocol}");
        //    _log($"\t\tIndex:{_usedSelenoidService}");
        //}

        //private SelenoidConfigModel _selectSelenoidService(ScraperOnmapStateModel state)
        //{
        //    _initConfig(state);

        //    Func<SelenoidConfigModel> GetSelenoidService = () => _config.Selenoid
        //                .Where(x=>x.Enabled)
        //                .Skip(_usedSelenoidService)
        //                .FirstOrDefault();

        //    var selenoidService = GetSelenoidService();

        //    if (selenoidService is null)
        //    {
        //        _usedSelenoidService = 0;
        //        selenoidService = GetSelenoidService();
        //    }

        //    return selenoidService;
        //}

        private void _initSelenoid(ScraperOnmapStateModel state)
        {
            if (_selenoidState is null) _selenoidState = new SelenoidStateModel();

            _initSelenoidBase(_selenoidState, state);
        }

        public List<ExcelRowOnmapModel> ScrapePhase3(ScraperOnmapStateModel state)
        {
            _log($"Generate domain model.");

            var listItemFiles = _loadItemsFromPath(state);
            var list = new List<ExcelRowOnmapModel>();

            foreach(var itemFile in listItemFiles)
            {
                var item = _loadItemFromPath(itemFile);
                var rowObj = item;
                var itemId = Path.GetFileNameWithoutExtension(itemFile.Name);

                if (rowObj != null)
                {
                    var row = new ExcelRowOnmapModel()
                    {
                        TagId_ = itemId,
                        DateCreate = rowObj?.created_at,
                        DateUpdate = rowObj?.updated_at,
                        EnCity = rowObj?.address.en?.city_name,
                        EnHouseNumber = rowObj?.address.en?.house_number,
                        EnNeighborhood = rowObj?.address.en?.neighborhood,
                        EnStreetName = rowObj?.address.en?.street_name,
                        HeCity = rowObj?.address.he?.city_name,
                        HeHouseNumber = rowObj?.address.he?.house_number,
                        HeNeighborhood = rowObj?.address.he?.neighborhood,
                        HeStreetName = rowObj?.address.he?.street_name,
                        Latitude = rowObj?.address.location?.lat,
                        Longitude = rowObj?.address.location?.lon,
                        AriaBase = rowObj?.additional_info.area.@base,
                        Balconies = rowObj?.additional_info.balconies,
                        Bathrooms = rowObj?.additional_info.bathrooms,
                        Elevators = rowObj?.additional_info.elevators,
                        FloorOn = rowObj?.additional_info.floor.on_the,
                        FloorOf = rowObj?.additional_info.floor.out_of,
                        Rooms = rowObj?.additional_info.rooms,
                        Toilets = rowObj?.additional_info.toilets,
                        ContactEmail = rowObj?.contacts.primary.email,
                        ContactName = rowObj?.contacts.primary.name,
                        ContactPhone = rowObj?.contacts.primary.phone,
                        Description = rowObj?.description,
                        Price = rowObj?.price,
                        PropertyType = rowObj?.property_type,
                        Section = rowObj?.section,
                        Images = rowObj?.images?.Select(x => new ExcelImageModel(x)).ToList(),
                        Videos = rowObj?.videos?.Select(x => new ExcelVideoModel(x)).ToList(),
                    };

                    list.Add(row);
                }
            }

            _log($"Generate domain model done");

            return list;
        }

        private Phase3ObjectDto _loadItemFromPath(FileInfo itemFile)
        {
            Phase3ObjectDto result = null;
            var filename = $"{itemFile.FullName}";

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<Phase3ObjectDto>(File.ReadAllText(filename));

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
