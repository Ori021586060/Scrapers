using Flurl.Http;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using ScraperCore;
using ScraperModels.Models.Excel;
using ScraperModels.Models.HomeLess;
using ScraperServices.Models;
using ScraperServices.Models.HomeLess;
using ScraperServices.Services;
using ScrapModels;
using ScrapModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ScrapySharp.Extensions;
using HtmlAgilityPack;

namespace ScraperServices.Scrapers
{
    public class ScraperHomeLess : ScraperBase
    {
        private ScraperHomeLessStateModel _state { get; set; }
        private int _usedSelenoidService { get; set; } = 0;
        private ScraperHomeLessConfigModel _config { get; set; }
        private SelenoidStateModel _selenoidState { get; set; }

        public ScraperHomeLess(ScraperHomeLessStateModel state = null)
        {
            if (state is null) state = new ScraperHomeLessStateModel();

            _state = state;

            _loadScraperConfig(_state);

            _checkDirectory(_state.RootPath);
        }

        private void _initSelenoid(ScraperHomeLessStateModel state)
        {
            if (_selenoidState is null) _selenoidState = new SelenoidStateModel();

            _initSelenoidBase(_selenoidState, state);
        }

        public ExcelHomeLessService GetExcelService()
        {
            return new ExcelHomeLessService(_state);
        }

        protected override async Task<bool> ScrapeInner()
        {
            var result = true;
            var state = _state;

            SetWorkPhaseBase("Scrape", state, $"Start scraper HomeLess (isNew:{_state.IsNew})");

            if (state.IsNew) _clearWorkspace(state);

            _checkWorkspace(state);

            ScrapePhase1_GenerateListPages(state);

            await ScrapePhase1_DownloadPages_Scrapy(state);

            ScrapePhase2_GenerateListItems(state);

            ScrapePhase2_DownloadItems(state);

            _log($"Scraper HomeLess done (isNew:{_state.IsNew})");

            return result;
        }

        private async Task ScrapePhase1_DownloadPages_Scrapy(ScraperHomeLessStateModel state)
        {
            SetWorkPhaseBase("DownloadPages", state);

            var listPages = _loadListPages(state);
            fixListPages(listPages, state);

            var needToDo = listPages.Where(x => x.Value == false).Select(x => x.Key).ToList();

            var tasks = new List<Task<bool>>();
            
            if (needToDo.Count > 0)
            {
                foreach (var page in needToDo)
                    tasks.Add(Task.Run(async() => listPages[page] = await GetPageAsync(page, state)));

                await _saveListPagesAsync(listPages, state);
            }

            Task.WaitAll(tasks.ToArray());

            _log($"Phase done");
        }

        private async Task<bool> GetPageAsync(int page, ScraperHomeLessStateModel state)
        {
            var result = false;
            try
            {
                var html = await GetPage_ScrappyAsync(page, state);
                var listAdsFromPage = ParseAdsFromPage(html, state);

                if (listAdsFromPage.Count > 0)
                {
                    await _savePageAsync(page, listAdsFromPage, state);
                    result = true;
                }
            }catch(Exception exception)
            {
                _log($"Error T1. {exception.Message} / {exception.StackTrace}");
            }

            return result;
        }

        private List<AdDtoModel> ParseAdsFromPage(HtmlDocument html, ScraperHomeLessStateModel state)
        {
            var result = new List<AdDtoModel>();

            var list1 = ParseAdsFromPage_Rent(html, EnumTypeItems.Rent);
            var list2 = ParseAdsFromPage_Rent(html, EnumTypeItems.RentTivuch);

            result.AddRange(list1);
            result.AddRange(list2);

            return result;
        }

        private List<AdDtoModel> ParseAdsFromPage_Rent(HtmlDocument html, EnumTypeItems typeItem)
        {
            var rows = ParseAdsFromPage_GetRows(html, typeItem);
            var result = ParseAdsFromPage_ParseRows(rows, typeItem);

            return result;
        }

        private List<AdDtoModel> ParseAdsFromPage_ParseRows(List<HtmlNode> rows, EnumTypeItems typeItem)
        {
            var result = new List<AdDtoModel>();

            foreach (var row in rows)
            {
                var ad = ParseAdsFromPage_ParseAd(row, typeItem);
                if (ad != null) result.Add(ad);
            }

            return result;
        }

        private AdDtoModel ParseAdsFromPage_ParseAd(HtmlNode row, EnumTypeItems typeItem)
        {
            AdDtoModel result = null;
            var listTds = row.CssSelect("td").ToList();
            var attr = listTds?.FirstOrDefault()?.Attributes["class"]?.Value;
            var positionDate = ParseAdsFromPage_ParseAd_GetPositionDate(typeItem);

            if (listTds.Count > 0 && attr == "selectionarea")
            {
                string id="", city = "", region = "", price = "", dateUpdated = "";

                try
                {
                    id = row.Attributes["id"].Value.Replace("ad_", "");
                    city = listTds.Skip(5).Take(1).FirstOrDefault().ChildNodes.FirstOrDefault().InnerHtml;//6
                    region = listTds.Skip(4).Take(1).FirstOrDefault().InnerHtml;//5
                    price = listTds.Skip(7).Take(1).FirstOrDefault().InnerHtml;//8
                    dateUpdated = listTds.Skip(positionDate-1).Take(1).FirstOrDefault()?.ChildNodes?.FirstOrDefault()?.InnerHtml;//10, revuch=11
                }catch (Exception exception)
                {
                    _log($"Error x4. {exception.Message} / {exception.StackTrace}");
                }

                result = new AdDtoModel()
                {
                    TypeItem = typeItem,
                    Id = id,
                    City = city,
                    Region = region,
                    Price = price,
                    DateUpdated = dateUpdated,
                };
            }

            return result;
        }

        private int ParseAdsFromPage_ParseAd_GetPositionDate(EnumTypeItems typeItem)
        {
            var result = 0;

            switch (typeItem)
            {
                case EnumTypeItems.Rent:
                    result = 10;
                    break;
                case EnumTypeItems.RentTivuch:
                    result = 11;
                    break;
            }

            return result;
        }

        private List<HtmlNode> ParseAdsFromPage_GetRows(HtmlDocument html, EnumTypeItems typeItem)
        {
            var idBlock = ParseAdsFromPage_GetIdBlock(typeItem);

            var block1 = html.DocumentNode.CssSelect($"#{idBlock}").FirstOrDefault();

            var rows = block1.CssSelect("tr").ToList();

            return rows;
        }

        private string ParseAdsFromPage_GetIdBlock(EnumTypeItems typeItem)
        {
            var idBlock = "mainresults";
            switch (typeItem)
            {
                case EnumTypeItems.Rent:
                    idBlock = "mainresults";
                    break;

                case EnumTypeItems.RentTivuch:
                    idBlock = "relatedresults";
                    break;
            }

            return idBlock;
        }

        private async Task<HtmlDocument> GetPage_ScrappyAsync(int pageNumber, ScraperHomeLessStateModel state)
        {
            HtmlDocument result = null;
            var needReplay = false;
            var web = new HtmlWeb();

            do
            {
                needReplay = false;
                try
                {
                    result = await web.LoadFromWebAsync($"https://www.homeless.co.il/rent/{pageNumber}");
                }
                catch (Exception exception)
                {
                    _log($"Error-g1. Wait 1 src. {exception.Message}");
                    needReplay = true;
                    Thread.Sleep(1000 * 1);
                }
            } while (needReplay);

            return result;
        }

        public DataScrapeModel GetDomainModel()
        {
            var list = ScrapePhase4(_state);

            var result = new DataScrapeModel(){
                Scraper = _state.TypeScraper,
                Data = list,
            };

            return result;
        }

        private void ScrapePhase2_GenerateListItems(ScraperHomeLessStateModel state)
        {
            SetWorkPhaseBase("GenerateListItems", state);

            var listItems = _loadListItems(state);

            if (listItems == null || listItems.Count == 0 || state.IsNew)
            {
                _log($"Start generate list-items");

                var listPages = _loadListPages(state);
                listItems = new Dictionary<string, AdDtoModel>();
                var listItemDublicate = new List<string>();

                var pages = listPages.Select(x => x.Key).ToList();

                foreach (var page in pages)
                {
                    var itemsOnPage = _loadPage(page, state);

                    if (itemsOnPage != null)
                    {
                        foreach (var item in itemsOnPage)
                        {
                            var key = item.Id;
                            if (listItems.ContainsKey(key))
                                listItemDublicate.Add(key);
                            else
                                listItems.Add(key, item);
                        }
                    }
                }

                _log($"Scraped uniq items:{listItems.Count}, items-dublicates:{listItemDublicate.Count}, total:{listItems.Count+ listItemDublicate.Count}");

                _saveListItems(listItems, state);
                _saveListItemsDublicates(listItemDublicate, state);
            }
            else
                _log($"Generate list-item skipped (isNew:{state.IsNew})");
        }

        private void _saveListItemsDublicates(List<string> list, ScraperHomeLessStateModel state)
        {
            var filename = state.ListItemsDublicatesFilename;

            _log($"Saving list-items: {filename}");

            File.WriteAllText($"{filename}", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));

            _log($"Save list-items done");
        }

        private void _saveListItems(Dictionary<string, AdDtoModel> list, ScraperHomeLessStateModel state)
        {
            var filename = state.ListItemsFilename;

            _log($"Saving list-items: {filename}");

            File.WriteAllText($"{filename}", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));

            _log($"Save list-items done");
        }

        private Dictionary<string, AdDtoModel> _loadListItems(ScraperHomeLessStateModel state)
        {
            Dictionary<string, AdDtoModel> result = null;

            var filename = state.ListItemsFilename;

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<Dictionary<string, AdDtoModel>>(File.ReadAllText(filename));

            return result;
        }

        protected override void _checkWorkspaceInner(IState state)
        {
            _checkDirectory($"{state.PagesPath}");
            _checkDirectory($"{state.ItemsPath}");
        }

        private void ScrapePhase2_DownloadItems(ScraperHomeLessStateModel state)
        {
            SetWorkPhaseBase("DownloadItems", state);

            var listItems = _loadListItems(state);
            _fixListItems(listItems, state);

            Func<List<KeyValuePair<string,AdDtoModel>>> NeedToDo = () => listItems.Where(x => x.Value.DownloadedItem == false).ToList();
            var countDownloaded = 0;
            var maxTasks = 5;
            var tasks = new List<Task<bool>>();
            var needToDoItems = NeedToDo();

            do
            {
                var freeTasks = maxTasks - tasks.Count();
                foreach (var item in needToDoItems.Take(freeTasks))
                {
                    var id = item.Key;
                    listItems[id].DownloadedItem = true;
                    tasks.Add(Task.Run(async () => listItems[id].DownloadedItem = await _downloadItemAsync(item.Value, state)));
                }

                //Thread.Sleep(1000 * 2);
                Task.WaitAny(tasks.ToArray());

                countDownloaded+=tasks.Where(x=>x.IsCompleted).Count();
                tasks.RemoveAll(x => x.IsCompleted);

                if (countDownloaded % 100 == 0)
                {
                    _log($"CountDownloaded are {countDownloaded}");
                    _saveListItems(listItems, state);
                }

                needToDoItems = NeedToDo();

            } while (needToDoItems.Count>0);

            Task.WaitAll(tasks.ToArray());

            _log($"Scraped {countDownloaded} items");
            _saveListItems(listItems, state);

            _log($"Download items done");
        }

        private void _fixListItems(Dictionary<string, AdDtoModel> list, ScraperHomeLessStateModel state)
        {
            var itemsFiles = new DirectoryInfo(state.ItemsPath).GetFiles();
            var amountFixed = 0;

            foreach (var itemFile in itemsFiles)
            {
                var id = Path.GetFileNameWithoutExtension(itemFile.Name);
                if (list.ContainsKey(id) && !list[id].DownloadedItem)
                {
                    list[id].DownloadedItem = true;
                    amountFixed++;
                }
            }

            _log($"Fixed {amountFixed} files");
        }

        private async Task<bool> _downloadItemAsync(AdDtoModel item, ScraperHomeLessStateModel state)
        {
            var result = true;
            var id = item.Id;

            try
            {
                var adDetails = _getAdDetailsFromService(item);
                var coordinates = _getCoordinatesFromService(item);
                var phones = _getPhonesFromService(item);
                var details = _getDetailsFromService(item);

                var detailsItemDto = new DetailsItemDtoModel
                {
                    Coordinates = await coordinates,
                    Phones = await phones,
                    Details = await details,
                    AdDetails = await adDetails,
                    RowDataFromPage = item,
                };

                await _saveItemDetailsAsync(id, detailsItemDto, state);

            }
            catch (Exception exception)
            {

                _log($"Error-f1. {exception.Message} / {exception.StackTrace}");
                result = false;
            }

            return result;
        }

        private async Task<AdDetailsDtoModel> _getAdDetailsFromService(AdDtoModel item)
        {
            AdDetailsDtoModel result = null;
            PostAdDetailsDtoModel postData = null;
            var id = item.Id;
            var request = "";
            var board = item.TypeItem.ToString();
            var needDo = false;
            var url = $"https://www.homeless.co.il/webservices/icardos.asmx/Cpart_GetAdDetails?boardid={board}&itemid={id}";

            do
            {
                needDo = false;
                try
                {
                    request = await url
                        .WithHeaders(new
                        {
                            User_Agent = "wow hackers, I need your money",
                            Accept = "*/*",
                            Content_Type = "application/json; charset=utf-8",
                        })
                        .PostJsonAsync(new { boardid = board, itemid = id })
                        .ReceiveString();

                    postData = JsonConvert.DeserializeObject<PostAdDetailsDtoModel>(request);
                    result = postData.d;
                }
                catch (Exception exception)
                {
                    _log($"Error d4. Wait 1 sec. {exception.Message} ");
                    needDo = true;
                    Thread.Sleep(1000);
                }
            } while (needDo);

            return result;
        }

        private async Task<List<PostDetailsDDtoModel>> _getDetailsFromService(AdDtoModel item)
        {
            PostDetailsDtoModel details = null;
            List<PostDetailsDDtoModel> result = null;
            string request = "";
            var id = item.Id;
            var board = item.TypeItem.ToString();
            var needDo = false;
            var url = $"https://www.homeless.co.il/webservices/icardos.asmx/GetItemDetails";

            do
            {
                needDo = false;
                try
                {
                    request = await url
                        .WithTimeout(60)
                        .WithHeaders(new
                        {
                            User_Agent = "wow hackers, I need your money",
                        })
                        .PostJsonAsync(new { boardid = board, itemid = id })
                        .ReceiveString();
                }
                catch (Exception exception)
                {
                    _log($"Error d3. {exception.Message} ");
                    needDo = true;
                    Thread.Sleep(1000);
                }
            } while (needDo);

            details = ParseDetailsFromService(request);
            result = details.D;

            return result;
        }

        private PostDetailsDtoModel ParseDetailsFromService(string request)
        {
            PostDetailsDtoModel details = null;
            string h1="", h2 = "";

            try
            {
                h1 = request.Replace(@"\\\""", @"***").Replace("\"[", "[").Replace("]\"", "]");
                h2 = h1.Replace(@"\", "").Replace(@"***", @"'");

                details = JsonConvert.DeserializeObject<PostDetailsDtoModel>(h2);
            }catch(Exception exception)
            {
                _log($"Error f3. {exception.Message} / {exception.StackTrace}");
            }

            return details;
        }

        private async Task<string> _getPhonesFromService(AdDtoModel item)
        {
            var result = "";
            var id = item.Id;
            var board = item.TypeItem.ToString();
            var url = $"https://www.homeless.co.il/webservices/icardos.asmx/IncrementClickesAndGetPhoneNumber";

            var request1 = url
                .WithTimeout(60)
                .WithHeaders(new
                {
                    User_Agent = "wow hackers, I need your money",
                })
                .PostJsonAsync(new { boardType = board, AdID = id })
                .ReceiveJson<PostPhonesDtoModel>();

            var phones = await request1;
            result = phones.d;

            return result;
        }

        private async Task<List<CoordinateDtoModel>> _getCoordinatesFromService(AdDtoModel item)
        {
            List<CoordinateDtoModel> coordinates = null;
            //var url = $"https://nominatim.openstreetmap.org/search?q={item.Region} {item.City}&format=json";
            //var request = url
            //    .WithTimeout(60)
            //    .WithHeaders(new
            //    {
            //        User_Agent = "wow hackers, I need your money",
            //    })
            //    .GetJsonAsync<List<CoordinateDtoModel>>();

            //var coordinates = await request;

            var client = new WebClient();

            //ICredentials credentials = new NetworkCredential("lum-customer-hl_87223775-zone-static", "fntl9hhiw7g5");
            //client.Proxy = new WebProxy(new Uri("http://zproxy.lum-superproxy.io:22225"), true, null, credentials);
            //client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");
            //var url = $"https://nominatim.openstreetmap.org/search?q={item.Region} {item.City}&format=json";

            var response = "";
            var needRepeat = false;

            do
            {
                needRepeat = false;

                //ICredentials credentials = new NetworkCredential("lum-customer-hl_89055c51-zone-static-country-il", "y7ic12hyfl9b");
                ICredentials credentials = new NetworkCredential("lum-customer-hl_89055c51-zone-static", "y7ic12hyfl9b");
                client.Proxy = new WebProxy(new Uri("http://zproxy.lum-superproxy.io:22225"), true, null, credentials);
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");
                var url = $"https://nominatim.openstreetmap.org/search?q={item.Region} {item.City}&format=json";

                try
                {
                    response = await client.DownloadStringTaskAsync(new Uri(url));
                    coordinates = JsonConvert.DeserializeObject<List<CoordinateDtoModel>>(response);
                }
                catch (Exception exception)
                {
                    _log($"Error g1. Coordinates. Wait 2 sec. {exception.Message}");
                    needRepeat = true;
                    Thread.Sleep(1000 * 2);
                }
            } while (needRepeat);

            return coordinates;
        }

        private async Task _saveItemDetailsAsync(string id, DetailsItemDtoModel response, ScraperHomeLessStateModel state)
        {
            var filename = $"{state.ItemsPath}/{id}.json";

            await File.WriteAllTextAsync(filename, JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented));

            _log($"Save json file {filename}");
        }

        private async Task _saveItemAsync(string id, string response, ScraperHomeLessStateModel state)
        {
            var filename = $"{state.ItemsPath}/{id}.xml";

            await File.WriteAllTextAsync(filename, response);

            _log($"Save xml-data file {filename}");
        }

        private List<AdDtoModel> _loadPage(int page, ScraperHomeLessStateModel state)
        {
            List<AdDtoModel> result = null;

            var filename = $"{state.PagesPath}/page-{page}.json";

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<List<AdDtoModel>>(File.ReadAllText(filename));

            return result;
        }

        private List<ExcelRowHomeLessModel> ScrapePhase4(ScraperHomeLessStateModel state)
        {
            var listRows = new List<ExcelRowHomeLessModel>();
            var files = new DirectoryInfo($"{state.ItemsPath}").GetFiles();
            var listPages = _loadPages(state);

            foreach(var file in files)
            {
                var id = Path.GetFileNameWithoutExtension(file.Name);
                var itemDto = LoadItemFromStore<DetailsItemDtoModel>(file);
                itemDto.RowDataFromPage = listPages.Where(x=>x.Key == id).Select(x=>x.Value).FirstOrDefault();

                var item = new ExcelRowHomeLessModel().FromDto(itemDto);

                listRows.Add(item);
            }

            return listRows;
        }

        private Dictionary<string, AdDtoModel> _loadPages(ScraperHomeLessStateModel state)
        {
            var result = new Dictionary<string, AdDtoModel>();

            var path = $"{state.PagesPath}";
            var listPageFiles = new DirectoryInfo(path).GetFiles();

            foreach(var pageFile in listPageFiles)
            {
                var listAdDto = JsonConvert.DeserializeObject<List<AdDtoModel>>(File.ReadAllText(pageFile.FullName));
                foreach(var adDto in listAdDto)
                {
                    if (!result.ContainsKey(adDto.Id))
                        result.Add(adDto.Id, adDto);
                }
            }

            return result;
        }

        private DetailsItemDtoModel _loadDetailsItem(FileInfo file)
        {
            DetailsItemDtoModel result = null;
            var id = Path.GetFileNameWithoutExtension(file.Name);
            var filename = $"{Path.GetDirectoryName(file.FullName)}/{id}.json";

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<DetailsItemDtoModel>(File.ReadAllText(filename));

            return result;
        }

        private Phase4XmlDtoModel _parseXmlData(XmlDocument xmlDoc)
        {
            Phase4XmlDtoModel itemDto = null;

            try
            {
                string jsonText = JsonConvert.SerializeXmlNode(xmlDoc).Replace("?xml", "xml");

                var dto = JsonConvert.DeserializeObject<object>(jsonText);
                itemDto = JsonConvert.DeserializeObject<Phase4XmlDtoModel>(jsonText);
            } catch (Exception exception)
            {
                _log($"Error-d1. {exception.Message} / {exception.StackTrace}");
            }

            return itemDto;
        }

        private XmlDocument _loadXmlDataFile(FileInfo file)
        {
            //var xmlData = File.ReadAllText(file.FullName);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file.FullName);

            return xmlDoc;
        }

        private void ScrapePhase1_DownloadPages_Selenoid(ScraperHomeLessStateModel state)
        {
            SetWorkPhaseBase("DownloadPages", state);

            var listPages = _loadListPages(state);
            fixListPages(listPages, state);

            var needToDo = listPages.Where(x => x.Value == false).Select(x=>x.Key).ToList();
            var needSaveListPages = false;
            var countPages = 0;
            var countPagesPerSave = 10;
            List<AdDtoModel> listAdFromPage = null;
            var needRepeate = false;
            var countTryRepeate = 0;
            var countMaxTryRepeate = 10;

            if (needToDo.Count > 0)
            {
                _initSelenoid(state);

                foreach (var page in needToDo)
                {
                    do
                    {
                        needRepeate = false;
                        try
                        {
                            listAdFromPage = _parsePage(page, state);
                        }
                        catch
                        {
                            countTryRepeate++;
                            _log($"\t\t!!! Need reinit SelenoidService !!!");
                            _initSelenoid(state);
                            Thread.Sleep(TimeSpan.FromMinutes(10));
                            needRepeate = true;
                            if (countTryRepeate > countMaxTryRepeate)
                            {
                                _log($"SelenoidService not inited.");
                                _abortApplication();
                            }
                        }
                    } while (needRepeate);

                    if (listAdFromPage.Count > 0)
                    {
                        _savePage(page, listAdFromPage, state);
                        listPages[page] = true;
                        if (needSaveListPages)
                        {
                            _saveListPagesAsync(listPages, state).Wait();
                            needSaveListPages = false;
                        }

                        countPages++;

                        if (countPages > countPagesPerSave)
                        {
                            countPages = 0;
                            needSaveListPages = true;
                        }
                    }

                }

                _saveListPagesAsync(listPages, state).Wait();

                _closeSelenoid();
            }

            _log($"Phase-2 done");
        }

        private void fixListPages(Dictionary<int, bool> listPages, ScraperHomeLessStateModel state)
        {
            var path = $"{state.PagesPath}";
            var listFiles = new DirectoryInfo(path).GetFiles();
            var amountFixed = 0;

            foreach (var file in listFiles)
            {
                var filename = Path.GetFileNameWithoutExtension(file.Name);
                var n = int.Parse(filename.Split("-")[1]);
                if (listPages.ContainsKey(n) && !listPages[n])
                {
                    listPages[n] = true;
                    amountFixed++;
                }
            }
            _log($"Fixed {amountFixed} files");
        }

        private void _abortApplication()
        {
            _log($"Abort Application");
            Environment.Exit(1);
        }

        private async Task _savePageAsync(int page, List<AdDtoModel> listAdFromPage, ScraperHomeLessStateModel state)
        {
            var filename = $"{state.PagesPath}/page-{page}.json";

            await File.WriteAllTextAsync(filename, JsonConvert.SerializeObject(listAdFromPage, Newtonsoft.Json.Formatting.Indented));

            _log($"Save file {filename}");
        }

        private void _savePage(int page, List<AdDtoModel> listAdFromPage, ScraperHomeLessStateModel state)
        {
            var filename = $"{state.PagesPath}/page-{page}.json";

            File.WriteAllText(filename, JsonConvert.SerializeObject(listAdFromPage, Newtonsoft.Json.Formatting.Indented));

            _log($"Save file {filename}");
        }

        private List<AdDtoModel> _parsePage(int page, ScraperHomeLessStateModel state)
        {
            List<AdDtoModel> result = new List<AdDtoModel>();

            var isDoneGetPage = _getPage_Selenoid(page: page, state: state);

            if (isDoneGetPage)
            {
                _scrapeFromPage_Rent(result, EnumTypeItems.Rent);
                _scrapeFromPage_Rent(result, EnumTypeItems.RentTivuch);
            }

            return result;
        }

        private void _scrapeFromPage_Rent(List<AdDtoModel> result, EnumTypeItems typeItem)
        {
            var idBlock = "mainresults";

            switch (typeItem)
            {
                case EnumTypeItems.Rent:
                    idBlock = "mainresults";
                    break;

                case EnumTypeItems.RentTivuch:
                    idBlock = "relatedresults";
                    break;
            }

            var data = _selenoidState.WindowMain.FindElementById(idBlock);//board mainresults/relatedresults rent/RentTivuch

            var rows = data.FindElements(By.TagName("tr"));

            foreach (var row in rows)
            {
                var listTds = row.FindElements(By.TagName("td"));

                if (listTds.Count > 0 && listTds.FirstOrDefault().GetAttribute("class") == "selectionarea")
                {
                    var id = row.GetAttribute("id");
                    var city = listTds.Skip(5).Take(1).FirstOrDefault().Text;//6
                    var region = listTds.Skip(4).Take(1).FirstOrDefault().Text;//5
                    var price = listTds.Skip(7).Take(1).FirstOrDefault().Text;//8
                    var dateUpdated = listTds.Skip(9).Take(1).FirstOrDefault().Text;//10

                    var item = new AdDtoModel()
                    {
                        TypeItem = typeItem,
                        Id = id.Replace("ad_", ""),
                        City = city,
                        Region = region,
                        Price = price,
                        DateUpdated = dateUpdated,
                    };

                    result.Add(item);
                }
            }
        }

        private void ScrapePhase1_GenerateListPages(ScraperHomeLessStateModel state)
        {
            SetWorkPhaseBase("GenerateListPages", state);

            var listPages = _loadListPages(state);

            if (listPages == null || listPages.Count == 0 || state.IsNew)
            {
                //var amountPages = ScrapePhase1_GetAmountPages_Selenoid(state);
                var amountPages = ScrapePhase1_GetAmountPages_WebClientAsync(state);
                var list = ScrapePhase1_GenerateListPages(amountPages.Result);
                _saveListPagesAsync(list, state).Wait();
            }
            else
                _log($"Generate list page not need (missing, isNew:{state.IsNew})");
        }

        private Dictionary<int, bool> ScrapePhase1_GenerateListPages(int amountPages)
        {
            var list = new Dictionary<int, bool>();

            //amountPages = 2; // for debug

            foreach (var i in Enumerable.Range(1, amountPages)) list.Add(i, false);

            _log($"Generate new list of pages done");

            return list;
        }

        private async Task<int> ScrapePhase1_GetAmountPages_WebClientAsync(ScraperHomeLessStateModel state)
        {
            var result = 0;

            var page = await GetPage_WebClientAsync(1, state);
            var amountPages = ScrapeAmountPages(page);

            result = amountPages;

            return result;
        }

        private int ScrapeAmountPages(string page)
        {
            var result = 0;

            try
            {
                Regex regex = new Regex(@"pagingdisplay(.*)span");
                MatchCollection matches = regex.Matches(page);
                if (matches.Count > 0)
                {
                    var text1 = matches[0].Value.Replace("<span>1</span>", "");
                    var text2 = new String(text1.Where(Char.IsDigit).ToArray());
                    result = int.Parse(text2);
                }
            }catch(Exception exception)
            {
                _log($"Error x4. {exception.Message} / {exception.StackTrace}");
            }

            return result;
        }

        private async Task<string> GetPage_WebClientAsync(int page, ScraperHomeLessStateModel state)
        {
            var result = "";
            try
            {
                var url = $"https://www.homeless.co.il/rent/1";
                result = await url
                    .WithHeaders(new
                    {
                        User_Agent = "Windows",
                    })
                    .GetStringAsync();
            }catch(Exception exception)
            {
                _log($"Error x2. {exception.Message} / {exception.StackTrace}");
            }

            return result;
        }

        private async Task _saveListPagesAsync(Dictionary<int, bool> list, ScraperHomeLessStateModel state)
        {
            await File.WriteAllTextAsync($"{state.ListPagesFilename}", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));

            _log($"Save list pages {state.ListPagesFilename} done");
        }

        private int ScrapePhase1_GetAmountPages_Selenoid(ScraperHomeLessStateModel state)
        {
            var result = 0;
            
            var tryCount = 1;
            var tryCountMax = 10;
            var doNeedTry = false;

            _log($"Detecting amount pages");

            _initSelenoid(state);

            do
            {
                doNeedTry = false;
                var isDoneGetPage1 = _getPage_Selenoid(page: 1, state: state);

                if (isDoneGetPage1)
                {
                    var pageNavigate = _selenoidState.WindowMain.FindElementByClassName("pagingdisplay");
                    var lastPageText = pageNavigate.FindElements(By.TagName("span")).LastOrDefault().Text;

                    result = int.Parse(lastPageText);
                }
                else
                {
                    tryCount++;
                    if (tryCount <= tryCountMax)
                    {
                        doNeedTry = true;
                        _log($"Try count {tryCount}/{tryCountMax}. Pause 10 sec.");
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                    }
                    else
                        _log($"Tred {tryCount} detect amount pages. Stop scrap process.");
                }
            } while (doNeedTry);

            _log($"Detected was {result} pages");

            _closeSelenoid();

            return result;
        }

        private void _closeSelenoid()
        {
            _log($"Closing Selenoid Service");

            _selenoidState.WindowMain.Quit();
            //_windowMain.Close();

            _log($"Close Selenoid Service done");
        }

        private bool _getPage_Selenoid(int page, ScraperHomeLessStateModel state)
        {
            var result = false;
            var doNeedRepeatRequest = false;
            var count = 0;
            var countMax = 10;

            do
            {
                doNeedRepeatRequest = false;
                var url = $"https://homeless.co.il/rent/{page}";

                try
                {
                    _selenoidState.WindowMain.Navigate().GoToUrl(url);

                    _selenoidState.WaitMain.Until(ExpectedConditions.ElementIsVisible(By.ClassName("pagingdisplay")));
                    //Thread.Sleep(1000 * 5);
                    result = true;
                }
                catch (Exception exception)
                {
                    count++;
                    if (count < countMax)
                    {
                        _log($"!!! Need Reinit Selenoid (try {count}) !!!");
                        _initSelenoid(state);
                        doNeedRepeatRequest = true;
                    }
                    else
                    {
                        _log($"Try is out. Error Selenoid");
                    }
                }
            } while (doNeedRepeatRequest);

            return result;
        }

        //private void _initSelenoid(ScraperHomeLessStateModel state)
        //{
        //    var options = new FirefoxOptions();
        //    options.SetPreference("permissions.default.image", 2);
        //    options.SetPreference("javascript.enabled", false);

        //    bool doNeedRepeatRequest = false;
        //    int countRepeat = 0;
        //    int countRepeatMax = 10;

        //    do
        //    {
        //        var selenoidService = _selectSelenoidService(state);
        //        var selenoidUrl = $"{selenoidService.Protocol}://{selenoidService.Address}:4444/wd/hub";
        //        _log($"\tInit Selenoid Service-{_usedSelenoidService}:{selenoidUrl}");

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
        //                _log($"\tBad config Selenoid Service-{_usedSelenoidService}:");
        //                _printSelenoidServiceParams(selenoidService);
        //            }
        //            else
        //                _log($"Try is out. Error");
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

        //private SelenoidConfigModel _selectSelenoidService(ScraperHomeLessStateModel state)
        //{
        //    _initConfig(state);

        //    Func<SelenoidConfigModel> GetSelenoidService = () => _config.Selenoid
        //                .Where(x => x.Enabled)
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

        private void _loadScraperConfig(ScraperHomeLessStateModel state)
        {
            var configFilename = state.ConfigFilename;

            if (File.Exists(configFilename))
                _config = JsonConvert.DeserializeObject<ScraperHomeLessConfigModel>(File.ReadAllText(configFilename));
            else
            {
                _config = new ScraperHomeLessConfigModel();
                _saveScraperConfig(_config, configFilename);
            }
        }

        private void _saveScraperConfig(ScraperHomeLessConfigModel config, string configFilename)
        {
            File.WriteAllText(configFilename, JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));

            _log($"Save config:{configFilename} is done");
        }

        private Dictionary<int,bool> _loadListPages(ScraperHomeLessStateModel state)
        {
            Dictionary<int, bool> result = null;

            var filename = state.ListPagesFilename;

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<Dictionary<int, bool>>(File.ReadAllText(filename));

            return result;
        }

        protected override void _clearWorkspaceInner(IState state)
        {
            _cleanDirectory(state.RootPath);
            _cleanFile($"{state.LogFilename}");
        }

        private void _cleanDirectory(string path)
        {
            var dirInfo = new DirectoryInfo(path);

            _log($"Cleaning path: {dirInfo.FullName}");

            foreach (var dir in dirInfo.GetDirectories()) dir.Delete(recursive:true);

            foreach (var file in dirInfo.GetFiles()) file.Delete();

            _log("Cleaning path done");
        }

        private void _cleanFile(string filename)
        {
            _log($"Clean/delete file {filename}");

            if (File.Exists(filename)) File.Delete(filename);
        }

        private void _log(string message, IState state = null)
        {
            _logBase(message, state is null ? _state : state);
        }

        public ScraperHomeLessStatusModel StatusWorkspace()
        {
            var state = _state;

            state.WorkPhase = "StatusWorkspace";

            _log("Start");

            var scrapeDate = _statusWorkspace_ScrapeDateBase(state);
            var amountItemsFromPath = _statusWorkspace_AmountItemsFromPathBase(state);
            var amountPages = _statusWorkspace_AmountPagesBase(state);

            var amountItemsFromPages = _statusWorkspace_AmountItemsFromPages(state);
            var amountItemUniquesFromPages = _statusWorkspace_AmountItemUniquesFromPages(state);

            var status = new ScraperHomeLessStatusModel()
            {
                ScrapeDate = scrapeDate,
                AmountItemsFromPath = amountItemsFromPath,
                AmountPages = amountPages,
                AmountItemsFromPages = amountItemsFromPages,
                AmountItemUniquesFromPages = amountItemUniquesFromPages,
            };

            _log("End");

            return status;
        }

        private int _statusWorkspace_AmountItemUniquesFromPages(ScraperHomeLessStateModel state)
        {
            var list = _statusWorkspace_AmountItemsFromPages_GetItems(state);

            var dups = list.GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.First().Id, x => x.First().Done);

            return dups.Count();
        }

        private int _statusWorkspace_AmountItemsFromPages(ScraperHomeLessStateModel state)
        {
            var list = _statusWorkspace_AmountItemsFromPages_GetItems(state);

            //var dubs = list.GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.First().Id, x => x.First().Done);

            return list.Count();
        }

        private List<ItemTest> _statusWorkspace_AmountItemsFromPages_GetItems(ScraperHomeLessStateModel state)
        {
            var listPages = _statusWorkspace_AmountPages_GetFilesBase(state);

            var totalItems = 0;
            var list = new List<ItemTest>();

            foreach (var page in listPages)
            {
                var filename = page.FullName;
                var pageData = JsonConvert.DeserializeObject<List<AdDtoModel>>(File.ReadAllText(filename));
                var listItems = pageData.Select(x => new ItemTest() { Id = x.Id, Done = x.DownloadedItem }).ToList();
                list.AddRange(listItems);
                totalItems += listItems.Count;
            }

            return list;
        }

        public void PrintStatus(ScraperHomeLessStatusModel status)
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
