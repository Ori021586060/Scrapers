using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using ScraperCore;
using ScraperModels.Models;
using ScraperModels.Models.Excel;
using ScraperModels.Models.WinWinDto;
using ScraperServices.Models.WinWin;
using ScraperServices.Services;
using ScrapySharp.Extensions;
using ScraperModels;
using ScraperModels.Models.Domain;
using ScraperRepositories.Repositories;

namespace ScraperServices.Scrapers
{
    public class ScraperWinWin: ScraperBase
    {
        public ScraperWinWin(ScraperWinWinStateModel state = null)
        {
            if (state is null) state = new ScraperWinWinStateModel();

            _state = state;

            //_config = _loadScraperConfig(state);
        }

        public ExcelWinWinService GetExcelService()
        {
            return new ExcelWinWinService((IStateExcelService)_state);
        }

        protected override async Task<bool> ScrapeInnerAsync()
        {
            var state = (ScraperWinWinStateModel)_state;

            _prepareToWorkBase(state);

            await ScrapePhase1Async(state); // use Selenoid / generate list-regions.json

            await ScrapePhase2Async(state); // scrape to pages/region-[name_region]-page-[num_page].json

            await ScrapePhase2_GenerateListItemsAsync(state); // generate list-items.json

            await ScrapePhase3Async(state); // scrape to items/[item-id].json

            return true;
        }

        private async Task ScrapePhase2_GenerateListItemsAsync(ScraperWinWinStateModel state)
        {
            SetWorkPhaseBase("GenerateListItems", state);

            var listFilePages = _loadListPages(state);
            var listItems = await _loadListItemsAsync(state);

            foreach (var filePage in listFilePages)
            {
                var filename = $"{filePage.FullName}";
                var listShortItems = JsonConvert.DeserializeObject<List<ShortItemDtoModel>>(await File.ReadAllTextAsync(filename));

                foreach (var shortItem in listShortItems)
                {
                    if (!listItems.ContainsKey(shortItem.ItemId))
                    {
                        shortItem.Done = false;
                        listItems.Add(shortItem.ItemId, shortItem);
                    }
                }
            }

            await _saveListItemsAsync(listItems, state);

            LogDone(state);
        }

        private async Task<Dictionary<string, ShortItemDtoModel>> _loadListItemsAsync(ScraperWinWinStateModel state)
        {
            var listItems = new Dictionary<string, ShortItemDtoModel>();

            var filename = $"{state.ListItemsFilename}";

            if (File.Exists(filename))
                listItems = JsonConvert.DeserializeObject<Dictionary<string, ShortItemDtoModel>>(await File.ReadAllTextAsync(filename));
            else
            {
                await _saveListItemsAsync(listItems, state);
            }

            return listItems;
        }

        private async Task _saveListItemsAsync(Dictionary<string, ShortItemDtoModel> list, ScraperWinWinStateModel state)
        {
            var filename = $"{state.ListItemsFilename}";

            await File.WriteAllTextAsync(filename, JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
        }

        protected async Task<List<AdItemWinWinDomainModel>> ScrapePhase_GetDomainModelAsync(IState state)
        {
            var listDomainItems = new List<AdItemWinWinDomainModel>();
            var files = GetListItemFiles(state);

            foreach (var file in files)
            {
                var itemDto = await LoadItemDtoFromStoreAsync<AdItemWinWinDtoModel>(file, state);
                var itemDomain = new AdItemWinWinDomainModel().FromDto(itemDto);
                listDomainItems.Add(itemDomain);
            }

            return listDomainItems;
        }

        public async override Task<DataDomainModel> GetDomainModelAsync()
        {
            var state = _state;
            SetWorkPhaseBase($"DomainModel", state);
            var listDomainItems = await ScrapePhase_GetDomainModelAsync(state);

            var result = new DataDomainModel()
            {
                Scraper = state.TypeScraper,
                Data = listDomainItems,
            };

            _logBase($"DomainModel has {listDomainItems.Count()} AdItems", state);
            LogDone(state);

            return result;
        }


        private async Task<AdItemWinWinDtoModel> _loadItemDtoAsync(string file, ScraperWinWinStateModel state)
        {
            var filename = $"{file}";

            var itemDto = JsonConvert.DeserializeObject<AdItemWinWinDtoModel>(await File.ReadAllTextAsync(filename));

            var filenameShort = Path.GetFileName(filename);
            _log($"Load itemDto from filename:{filenameShort}");

            return itemDto;
        }

        private async Task ScrapePhase3Async(ScraperWinWinStateModel state)
        {
            SetWorkPhaseBase("DownloadItems", state);

            await _scrapePhase3Inner(state);

            LogDone(state);
        }

        private async Task _scrapePhase3Inner(ScraperWinWinStateModel state)
        {
            var listItems = await _loadListItemsAsync(state);
            await _fixListItems(listItems, state);
            var maxItems = 50;
            Func<int,List<ShortItemDtoModel>> NeedToDo = (i) => listItems.Where(x => x.Value.Done == false).Select(x => x.Value).Take(i).ToList();
            var task = new List<Task<bool>>();
            var needToDo = NeedToDo(maxItems);

            do
            {
                foreach (var shortItem in needToDo)
                {
                    var item = shortItem;
                    item.Done = true;
                    task.Add(Task.Run( async() => item.Done = await _downloadItemAsync(shortItem, state)));
                }

                Thread.Sleep(1000 * 3);
                Task.WaitAny(task.ToArray());
                task.RemoveAll(x => x.IsCompleted);

                needToDo = NeedToDo(maxItems - task.Count());
            } while (needToDo.Count() > 0);

            Task.WaitAll(task.ToArray());
        }

        private ShortItemDtoModel _loadShortItems(string itemId, ScraperWinWinStateModel state)
        {
            var filename = $"{state.ItemsPath}/{itemId}.json";

            var shortItem = JsonConvert.DeserializeObject<ShortItemDtoModel>(File.ReadAllText(filename));

            return shortItem;
        }

        private async Task _fixListItems(Dictionary<string, ShortItemDtoModel> listItems, ScraperWinWinStateModel state)
        {
            var addedFiles = 0;
            var fixedFiles = 0;
            var itemsFiles = new DirectoryInfo(state.ItemsPath).GetFiles();
            foreach (var itemFile in itemsFiles)
            {
                var filename = itemFile.Name;
                var id = Path.GetFileNameWithoutExtension(filename);
                
                if (!listItems.ContainsKey(id))
                {
                    _log($"Need added file, WTF");
                }
                else
                {
                    if (listItems[id].Done == false)
                    {
                        listItems[id].Done = true;
                        fixedFiles++;
                    }
                }
            }
            _log($"Added {addedFiles}, fixed {fixedFiles} items");
            await _saveListItemsAsync(listItems, state);
        }

        private void _regErrorScrape_ListItemIds(List<string> list, ScraperWinWinStateModel state)
        {
            var filename = state.LogErrorFilename;
            var message = $"Errors itemIds:{string.Join(",", list)}\r\n";

            File.AppendAllText(filename, message);

            _log($"Save errors");
        }

        private async Task<List<string>> _downloadItemsFromFilePage(FileInfo file, ScraperWinWinStateModel state)
        {
            _log($"Load list-items from file-page: {file.Name}");

            var listShortItems = _loadListItemsFromFilePage(file);
            //var listTasks = new List<Task<bool>>();
            var listTasks = new Dictionary<string,bool>();

            foreach (var shortItem in listShortItems)
            {
                if (_isDownloadedItem(shortItem, state))
                    _log($"Skip file listId={shortItem.ItemId}");
                else
                    listTasks.Add(shortItem.ItemId, await _downloadItemAsync(shortItem, state));
            }

            //Task.WaitAll(listTasks.ToArray());
            var listNoData = listTasks.Where(x => !x.Value).Select(x => x.Key).ToList();

            return listNoData;
        }

        private bool _isDownloadedItem(ShortItemDtoModel shortItem, ScraperWinWinStateModel state)
        {
            var filename = $"{state.ItemsPath}/{shortItem.ItemId}.json";

            var result = File.Exists(filename);

            return result;
        }

        private async Task<bool> _downloadItemAsync(ShortItemDtoModel shortItem, ScraperWinWinStateModel state)
        {
            var result = false;

            try
            {
                var itemPage = await _loadItemPageAsync(shortItem, state);

                var itemDto = await _parseItemPageAsync(itemPage, shortItem);

                await _saveItemDtoAsync(itemDto, state);

                result = true;
            }
            catch(Exception exception)
            {
                _log($"Error-z2. {exception.Message}");
            }

            return result;
        }

        private async Task _saveItemDtoAsync(AdItemWinWinDtoModel item, ScraperWinWinStateModel state)
        {
            var filename = $"{state.ItemsPath}/{item.ItemId}.json";

            await File.WriteAllTextAsync($"{filename}", JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented));

            _log($"Save item {item.ItemId}, filename:{filename}");
        }

        private List<ShortItemDtoModel> _loadListItemsFromFilePage(FileInfo file)
        {
            var list = JsonConvert.DeserializeObject<List<ShortItemDtoModel>>(File.ReadAllText(file.FullName));

            return list;
        }

        private FileInfo[] _loadListPages(ScraperWinWinStateModel state)
        {
            var list = new DirectoryInfo($"{state.PagesPath}").GetFiles();

            return list;
        }

        private async Task ScrapePhase2Async(ScraperWinWinStateModel state)
        {
            SetWorkPhaseBase("Phase-2", state);

            await _scrapePhase2Inner(state);

            LogDone(state);
        }

        private async Task _scrapePhase2Inner(ScraperWinWinStateModel state)
        {
            var listRegions = await _loadListRegionsAsync(state);

            foreach(var region in listRegions)
            {
                _downloadRegionPages(region, state);
            }
        }

        private void _downloadRegionPages(RegionModel region, ScraperWinWinStateModel state)
        {
            var listTasks = new List<Task<bool>>();

            foreach(var page in Enumerable.Range(1, region.AmountPages))
            {
                listTasks.Add( Task.Run( ()=>_downloadRegionPageAsync(page, region, state)));
            }

            Task.WaitAll(listTasks.ToArray());
        }

        private async Task<bool> _downloadRegionPageAsync(int page, RegionModel region, ScraperWinWinStateModel state)
        {
            var result = false;
            var filename = $"{state.PagesPath}/region-{region.Id}-page-{page}.json";

            if (!File.Exists(filename))
            {
                try
                {
                    var webPage = await _loadPageAsync(page, region, state);
                    var listShortItems = _parsePage(webPage, state);
                    _savePage(listShortItems, page, region, state);
                    result = true;
                }
                catch(Exception exception)
                {
                    _log($"Error-z1. {exception.Message}");
                }
            }
            else
            {
                _log($"No need download page {page} from {region.Id}");
                result = true;
            }

            return result;
        }

        private async Task<List<RegionModel>> _loadListRegionsAsync(ScraperWinWinStateModel state)
        {
            var list = new List<RegionModel>();
            var filename = $"{state.ListRegionsFilename}";

            if (File.Exists(filename))
                list = JsonConvert.DeserializeObject<List<RegionModel>>(await File.ReadAllTextAsync(filename));

            _log($"Load list-regions is done (file:{filename})");

            return list;
        }

        private async Task ScrapePhase1Async(ScraperWinWinStateModel state)
        {
            SetWorkPhaseBase("Phase-1", state);

            await _scrapePhase1InnerAsync(state);

            LogDone(state);
        }

        private async Task _scrapePhase1InnerAsync(ScraperWinWinStateModel state)
        {
            var hasError = false;
            var selenoidState = new SelenoidStateModel() {
                JavaScriptEnable = true,
                ShowPictures = false,
            };
            var indexRegion = 0;

            var listRegions = await _loadListRegionsAsync(state);

            if (listRegions.Count == 0 || state.IsNew)
            {

                _initSelenoidBase(selenoidState, state);

                _openMainPage(selenoidState, state);

                var regions = _getRegions(selenoidState);

                foreach (var regionIndex in Enumerable.Range(0, regions.Count))
                {
                    var indexTry = 0;
                    var tryMax = 50;
                    do
                    {
                        try
                        {
                            hasError = false;
                            var regionModel = _selectRegion(regionIndex, selenoidState);
                            _scrapeRegion(regionModel, selenoidState);
                            listRegions.Add(regionModel);
                            _log($"Scraped region {regionModel.Id}, amount pages: {regionModel.AmountPages}");
                            indexRegion++;
                            if (indexRegion % 10 == 0)
                                _saveListRegions(listRegions, state);
                        }
                        catch (NoSuchElementException)
                        {
                            _openMainPage(selenoidState, state);
                        }
                        catch (Exception exception)
                        {
                            var type = exception.GetType();
                            indexTry++;
                            if (indexTry < tryMax)
                            {
                                hasError = true;
                                _initSelenoidBase(selenoidState, state);
                                Thread.Sleep(1000 * 5);
                            }
                        }
                    } while (hasError);
                }

                _closeSelenoidBase(selenoidState, state);

                _saveListRegions(listRegions, state);
            }
        }

        private void _closeSelenoidBase(SelenoidStateModel selenoidState, ScraperWinWinStateModel state)
        {
            if (selenoidState.WindowMain != null)
            {
                selenoidState.WindowMain.Quit();
                //_windowMain.Close();

                _log($"Close Selenoid Service done");
            }
        }

        private void _saveListRegions(List<RegionModel> list, ScraperWinWinStateModel state)
        {
            var filename = $"{state.ListRegionsFilename}";

            File.WriteAllText($"{filename}", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));

            _log($"Save list-regions, filename:{filename}");
        }

        private void _scrapeRegion(RegionModel regionModel, SelenoidStateModel selenoidState)
        {
            var pagination = selenoidState.WindowMain.FindElementByClassName("pagination").FindElements(By.TagName("a"));
            var firstPage = pagination.FirstOrDefault();
            var lastPageElement = pagination.Skip(pagination.Count() - 2).FirstOrDefault();

            var href = firstPage.GetAttribute("href");
            var search = href.Split("search=")[1];
            var lastPage = lastPageElement.Text;

            //var firstNode = region.FindElement(By.TagName("div"));

            regionModel.Search = search;
            regionModel.AmountPages = Int32.Parse(lastPage);
        }

        private RegionModel _selectRegion(int regionIndex, SelenoidStateModel selenoidState)
        {
            var result = new RegionModel();

            var regions = _getRegions(selenoidState);
            _clearRegionsClick(selenoidState);

            var region = regions.Skip(regionIndex).FirstOrDefault();
            region.Click();

            var firstNode = region.FindElement(By.TagName("div"));
            result.Id = firstNode.GetAttribute("id");
            result.NameRegion = firstNode.GetAttribute("text");
            Thread.Sleep(1000 * 2);
            _clickSearchButtonAndWait(selenoidState);

            return result;
        }

        private void _clickSearchButtonAndWait(SelenoidStateModel selenoidState)
        {
            _clickSearchButton(selenoidState);

            selenoidState.WaitMain.Until(ExpectedConditions.ElementIsVisible(By.Id("carAreasDV")));
        }

        private void _clickSearchButton(SelenoidStateModel selenoidState)
        {
            var searchButton = selenoidState.WindowMain.FindElementById("searchBtnDiv");
            searchButton.Click();
        }

        private void _clearRegionsClick(SelenoidStateModel selenoidState)
        {
            var clearRegions = selenoidState.WindowMain.FindElementByClassName("CancelAllSelections");
            clearRegions.Click();
        }

        private void _openRegionClick(SelenoidStateModel selenoidState)
        {
            if (!_isOpenedRegionList(selenoidState))
            {
                var selectRegion = selenoidState.WindowMain.FindElementById("carAreasDV");

                selectRegion.Click();
            }
        }

        private bool _isOpenedRegionList(SelenoidStateModel selenoidState)
        {
            var selectRegion = selenoidState.WindowMain.FindElementById("box27");
            var classes = selectRegion.GetAttribute("class");

            var result = classes.Contains("boxTree2On");

            return result;
        }

        private IReadOnlyCollection<IWebElement> _getRegions(SelenoidStateModel selenoidState)
        {
            _openRegionClick(selenoidState);

            var listOptions = selenoidState.WindowMain.FindElementById("searchDataAreasMD");
            var listGoodOptions = listOptions.FindElements(By.ClassName("cSelD"));

            return listGoodOptions;
        }

        private void _openMainPage(SelenoidStateModel selenoidState, ScraperWinWinStateModel state)
        {
            var url = $"https://www.winwin.co.il/RealEstate/ForRent/Search/SearchResults/RealEstatePage.aspx?search=8bdb5277c594afddcf9414e7541fd518";
            var hasError = false;
            var tryCountMax = 20;
            var indexTry = 0;

            do
            {
                try
                {
                    hasError = false;

                    selenoidState.WindowMain.Navigate().GoToUrl(url);

                    selenoidState.WaitMain.Until(ExpectedConditions.ElementIsVisible(By.Id("carAreasDV")));
                }
                catch
                {
                    indexTry++;
                    if (indexTry < tryCountMax)
                    {
                        hasError = true;
                        _initSelenoidBase(selenoidState, state);
                        Thread.Sleep(1000 * 5);
                    }
                }
            } while (hasError);

        }

        private async Task<AdItemWinWinDtoModel> _parseItemPageAsync(HtmlDocument itemPage, ShortItemDtoModel shortItem)
        {
            DataCoordinatesLatLng coordinates = new DataCoordinatesLatLng();

            var iframe = _parseItemPage_GetMediaFrameAsync(itemPage).Result;

            var itemId = shortItem.ItemId;
            var city = shortItem.City;
            var dateUpdate = shortItem.DateUpdate;
            var area = _parseItemPage_GetArea(itemPage);
            var streetAddress = _parseItemPage_GetStreetAddress(itemPage);
            var rooms = _parseItemPage_GetRooms(itemPage);
            var floor = _parseItemPage_GetFloor(itemPage);
            var state = _parseItemPage_GetState(itemPage);
            var dateEnter = _parseItemPage_GetDateEnter(itemPage);
            var square = _parseItemPage_GetSquare(itemPage);
            var isPartners = _parseItemPage_GetIsPartners(itemPage);
            var amountPayment = _parseItemPage_GetAmountPayments(itemPage);
            var description = _parseItemPage_GetDescription(itemPage);
            var price = _parseItemPage_GetPrice(itemPage);
            
            var isAgent = _parseItemPage_IsAgency(itemPage);
            string phone1, phone2, contactName;
            if (isAgent)
            {
                contactName = _parseItemPage_GetContactName_Agent(itemPage);
                phone1 = _parseItemPage_GetPhone1_Agent(itemPage);
                phone2 = _parseItemPage_GetPhone2_Agent(itemPage);
            }
            else
            {
                contactName = _parseItemPage_GetContactName_Private(itemPage);
                phone1 = _parseItemPage_GetPhone1_Private(itemPage);
                phone2 = _parseItemPage_GetPhone2_Private(itemPage);
            }

            var isOk = _parseItemPage_GetCoordinates(itemPage, coordinates);
            if (!isOk)
            {
                isOk = _parseItemPage_GetCoordinates2(iframe, coordinates);
            }

            var images = _parseItemPage_GetImages(iframe);

            var result = new AdItemWinWinDtoModel() {
                ItemId = itemId,
                DateUpdate = dateUpdate,
                Longitude = coordinates.lng,
                Latitude = coordinates.lat,
                City = city,
                Area = area,
                StreetAddress = streetAddress,
                Rooms = rooms,
                Floor = floor,
                State = state,
                DateEnter = dateEnter,
                Square = square,
                IsPartners = isPartners,
                AmountPayment = amountPayment,
                Description = description,
                Price = price,
                ContactName = contactName,
                Phone1 = phone1,
                Phone2 = phone2,
                IsAgent = isAgent ? "yes":"no",
                Images = images,
            };

            return result;
        }

        private string _parseItemPage_GetPhone2_Agent(HtmlDocument page)
        {
            var phone = page.DocumentNode.CssSelect("#rightNumAgent")?.FirstOrDefault()?.InnerText;

            return phone;
        }

        private string _parseItemPage_GetPhone1_Agent(HtmlDocument page)
        {
            var phone = page.DocumentNode.CssSelect("#leftNumAgent")?.FirstOrDefault()?.InnerText;

            return phone;
        }

        private string _parseItemPage_GetContactName_Agent(HtmlDocument page)
        {
            var block1 = page.DocumentNode.CssSelect(".nameDirectLinkAgent")?.FirstOrDefault();
            var contactName = block1?.CssSelect("span")?.LastOrDefault()?.InnerText;

            return contactName;
        }

        private bool _parseItemPage_IsAgency(HtmlDocument page)
        {
            bool result;

            var elementAgency = page.DocumentNode.CssSelect("#ctl00_pageContent_contactCtl_agentDiv")?.FirstOrDefault();

            if (elementAgency is null )
                result = false;
            else
                result = true;

            return result;
        }

        private List<string> _parseItemPage_GetImages(string page)
        {
            var list = new List<string>();
            Regex regex = new Regex(@"ctl00_ContentPlaceHolder1_rptColumn1_ctl\d{2}_ImgColumn\d(.*)\/>");

            MatchCollection matches = regex.Matches(page);

            foreach(Match matche in matches)
            {
                var str1 = matche.Value?.Split("src=\"")[1];
                var urlImage = str1?.Split("\"")[0];
                list.Add(urlImage);
            }

            return list;
        }

        private string _parseItemPage_GetPhone1_Private(HtmlDocument page)
        {
            var phone = page.DocumentNode.CssSelect("#leftNum")?.FirstOrDefault()?.InnerText;

            return phone;
        }

        private string _parseItemPage_GetPhone2_Private(HtmlDocument page)
        {
            var phone = page.DocumentNode.CssSelect("#rightNum")?.FirstOrDefault()?.InnerText;

            return phone;
        }

        private string _parseItemPage_GetContactName_Private(HtmlDocument page)
        {
            var block1 = page.DocumentNode.CssSelect(".nameDirectLink")?.FirstOrDefault();
            var contactName = block1?.CssSelect("span")?.LastOrDefault()?.InnerText;

            return contactName;
        }

        private string _parseItemPage_GetPrice(HtmlDocument page)
        {
            var price = page.DocumentNode.CssSelect(".priceContent")?.FirstOrDefault()?.InnerText;

            return price;
        }

        private string _parseItemPage_GetDescription(HtmlDocument page)
        {
            var description = page.DocumentNode.CssSelect("#ctl00_pageContent_accs_comments")?.FirstOrDefault()?.InnerText;

            return description;
        }

        private string _parseItemPage_GetAmountPayments(HtmlDocument page)
        {
            var amountPayments = page.DocumentNode.CssSelect("#ctl00_pageContent_addDetails_payments")?.FirstOrDefault()?.InnerText;

            return amountPayments;
        }

        private string _parseItemPage_GetIsPartners(HtmlDocument page)
        {
            var isPartners = page.DocumentNode.CssSelect("#ctl00_pageContent_addDetails_partners")?.FirstOrDefault()?.InnerText;

            return isPartners;
        }

        private string _parseItemPage_GetSquare(HtmlDocument page)
        {
            var square = page.DocumentNode.CssSelect("#ctl00_pageContent_addDetails_space")?.FirstOrDefault()?.InnerText;

            return square;
        }

        private string _parseItemPage_GetDateEnter(HtmlDocument page)
        {
            var dateEnter = page.DocumentNode.CssSelect("#ctl00_pageContent_addDetails_entranceDate")?.FirstOrDefault()?.InnerText;

            return dateEnter;
        }

        private string _parseItemPage_GetState(HtmlDocument page)
        {
            var state = page.DocumentNode.CssSelect("#ctl00_pageContent_addDetails_condition")?.FirstOrDefault()?.InnerText;

            return state;
        }

        private string _parseItemPage_GetFloor(HtmlDocument page)
        {
            var floor = page.DocumentNode.CssSelect("#ctl00_pageContent_addDetails_floor")?.FirstOrDefault()?.InnerText;
            var floorsTxt = page.DocumentNode.CssSelect("#ctl00_pageContent_addDetails_floorsTxt")?.FirstOrDefault()?.InnerText;
            var floors = page.DocumentNode.CssSelect("#ctl00_pageContent_addDetails_floors")?.FirstOrDefault()?.InnerText;

            var result = $"{floor} {floorsTxt} {floors}";

            return floor;
        }

        private string _parseItemPage_GetRooms(HtmlDocument page)
        {
            var rooms = page.DocumentNode.CssSelect("#ctl00_pageContent_addDetails_rooms")?.FirstOrDefault()?.InnerText;

            return rooms;
        }

        private string _parseItemPage_GetStreetAddress(HtmlDocument page)
        {
            var block1 = page.DocumentNode.CssSelect(".addDetailsDirectLinkNew")?.FirstOrDefault();
            var block2 = block1?.CssSelect(".addDetailsAlternateRow")?.FirstOrDefault();
            var streetAddress = block2?.CssSelect(".addDetailsRowData")?.FirstOrDefault()?.InnerText?.ClearSymbols(); 

            return streetAddress;
        }

        private string _parseItemPage_GetArea(HtmlDocument page)
        {
            var area = page.DocumentNode.CssSelect("#ctl00_pageContent_addDetails_nHood")?.FirstOrDefault()?.InnerText;

            return area;
        }

        private async Task<string> _parseItemPage_GetMediaFrameAsync(HtmlDocument page)
        {
            var iframePage = "";
            var iframeElement = page.DocumentNode.CssSelect(".mediaFrame").FirstOrDefault();
            if (iframeElement != null)
            {
                var src = iframeElement.Attributes.Where(t => t.Name == "src").Select(t => t.Value).FirstOrDefault();
                var url = $"https://www.winwin.co.il{src.Replace("amp;", "")}";

                iframePage = await url.GetStringAsync();
            }

            return iframePage;
        }

        private bool _parseItemPage_GetCoordinates2(string iframe, DataCoordinatesLatLng coordinates)
        {
            var result = false;
            string x = "", y = "";
            MatchCollection matches;

            try
            {
                //var iframePage = await _parseItemPage_GetMediaFrame(page);

                Regex regex = new Regex(@"_Coord[X|Y]\s=\s(-?\d*\.\d*)");

                matches = regex.Matches(iframe);

                if (matches.Count > 0)
                {
                    x = matches[0].Value.Split("=")[1].Trim();
                    y = matches[1].Value.Split("=")[1].Trim();

                    coordinates.lat = x;
                    coordinates.lng = y;

                    result = true;
                }
            }
            catch (Exception exception)
            {
                //throw new Exception("iframe is null");
                _log($"Error-v1. {exception.Message} / {exception.StackTrace}");
            }

            return result;
        }

        private bool _parseItemPage_GetCoordinates(HtmlDocument page, DataCoordinatesLatLng coordinates)
        {
            var result = false;

            try
            {
                Regex regex = new Regex(@"var addLatLng = new google(.*);");

                MatchCollection matches = regex.Matches(page.ParsedText);

                var val = matches[0].Value;
                regex = new Regex(@"\((.*)\)");
                matches = regex.Matches(val);
                var cord = matches[0].Value?.Split(',');
                coordinates.lat = _clearCoordinates(cord[0]);
                coordinates.lng = _clearCoordinates(cord[1]);

                result = true;
            }
            catch {
                ;
            }

            return result;
        }

        private string _clearCoordinates(string x)
        {
            var result = x.Replace("(", "").Replace(")", "").Replace("'", "");

            return result;
        }

        private async Task<HtmlDocument> _loadItemPageAsync(ShortItemDtoModel shortItem, ScraperWinWinStateModel state)
        {
            var itemId = shortItem.ItemId;
            _log($"Loading item-id:{itemId}");
            var url = $"https://www.winwin.co.il/RealEstate/ForRent/Ads/RealEstateAds,{itemId}.aspx";
            var webGet = new HtmlWeb();
            HtmlDocument result = null;

            try
            {
                result = await webGet.LoadFromWebAsync(url);
            }
            catch (Exception exception)
            {
                _log($"Error-z3. {exception.Message}");
            }

            //_log($"Load item-id:{itemId} is done");

            return result;
        }

        private List<ShortItemDtoModel> _loadShortItemsFromPages(ScraperWinWinStateModel state)
        {
            var result = new List<ShortItemDtoModel>();
            var path = new DirectoryInfo(state.PagesPath);

            foreach(var file in path.GetFiles())
                result.AddRange(JsonConvert.DeserializeObject<List<ShortItemDtoModel>>(File.ReadAllText(file.FullName)));

            return result;
        }

        private void _savePage(List<ShortItemDtoModel> list, int numPage, RegionModel region, ScraperWinWinStateModel state)
        {
            var filename = $"{state.PagesPath}/region-{region.Id}-page-{numPage}.json";

            File.WriteAllText($"{filename}", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));

            _log($"Save page, filename:{filename}");
        }

        private List<ShortItemDtoModel> _parsePage(HtmlDocument document, ScraperWinWinStateModel state)
        {
            var result = new List<ShortItemDtoModel>();

            result.AddRange(_getItemShortFromNodes(document.DocumentNode.CssSelect(".paid")));

            result.AddRange(_getItemShortFromNodes(document.DocumentNode.CssSelect(".TitleData")));

            return result;
        }

        private List<ShortItemDtoModel> _getItemShortFromNodes(IEnumerable<HtmlNode> nodes)
        {
            var result = new List<ShortItemDtoModel>();

            foreach (var node in nodes)
            {
                var itemId = _getIdFromTr(node);
                var apportamentType = _getApportamentTypeFromTr(node);
                var city = _getCityFromTr(node);
                var address = _getAddressFromTr(node);
                var checkHour = _getCheckHourFromTr(node);
                var price = _getPriceFromTr(node);
                var dateUpdate = _getDateUpdateFromTr(node);

                var item = new ShortItemDtoModel() {
                    ItemId = itemId,
                    ApportamentType = apportamentType,
                    City = city,
                    Address = address,
                    CheckHour = checkHour,
                    Price = price,
                    DateUpdate = dateUpdate,
                };
                result.Add(item);
            }

            return result;
        }

        private string _cleanString(string data)
        {
            var result = data.Trim(new char[] { '\r', '\n', '\t', '₪', ' ' });

            return result;
        }

        private string _getDateUpdateFromTr(HtmlNode node)
        {
            var dateUpdate = node.ChildNodes.Skip(21).FirstOrDefault().InnerText; // date update

            return dateUpdate;
        }

        private string _getPriceFromTr(HtmlNode node)
        {
            var price = _cleanString(node.ChildNodes.Skip(17).FirstOrDefault().InnerText); // price

            return price;
        }

        private string _getCheckHourFromTr(HtmlNode node)
        {
            var checkHour = node.ChildNodes.Skip(15).FirstOrDefault().InnerText; // check hour

            return checkHour;
        }

        private string _getAddressFromTr(HtmlNode node)
        {
            var address = node.ChildNodes.Skip(13).FirstOrDefault().InnerText; // address

            return address;
        }

        private string _getCityFromTr(HtmlNode node)
        {
            var city = node.ChildNodes.Skip(11).FirstOrDefault().InnerText; // city

            return city;
        }

        private string _getApportamentTypeFromTr(HtmlNode node)
        {
            var apportamentType = node.ChildNodes.Skip(7).FirstOrDefault().InnerText;
            var floor = node.ChildNodes.Skip(9).FirstOrDefault().InnerText; // ??

            return apportamentType;
        }

        private string _getIdFromTr(HtmlNode node)
        {
            var onclick = node.Attributes["onclick"].Value;
            var listParams = onclick.Split(',');
            var id = listParams[7].Trim().Replace("'","");

            return id;
        }

        private void _generateNewListPages(ScraperWinWinStateModel state)
        {
            _log($"Generate new list-pages");

            var amountPages = _getAmountPages(state);

            var listPages = new Dictionary<int, bool>();

            foreach (var page in Enumerable.Range(1, amountPages))
                listPages.Add(page, false);

            _log($"Generated list-pages is done");

            _saveListPage(listPages, state);
        }

        private void _saveListPage(Dictionary<int, bool> list, ScraperWinWinStateModel state)
        {
            var filename = state.ListPagesFilename;

            File.WriteAllText($"{filename}", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));

            _log($"Saved list-page:{state.ListPagesFilename}");
        }

        private int _getAmountPages(ScraperWinWinStateModel state)
        {
            var result = 0;

            //var page = _loadPageAsync(1, state).Result;
            //result = _detectLastNumPage(page);

            return result;
        }

        private int _detectLastNumPage(HtmlDocument document)
        {
            var result = 0;

            try
            {
                var lastPage = document.DocumentNode.CssSelect(".lastPage").FirstOrDefault().InnerText;
                result = Int32.Parse(lastPage);
            }
            catch { }

            return result;
        }

        private async Task<HtmlDocument> _loadPageAsync(int numPage, RegionModel region, ScraperWinWinStateModel state)
        {
            _log($"Loading page {numPage}");
            var url = $"https://www.winwin.co.il/RealEstate/ForRent/RealEstatePage.aspx?PageNumberBottom={numPage}&PageNumberTop={numPage}&search={region.Search}";
            var webGet = new HtmlWeb();

            var result = await webGet.LoadFromWebAsync(url);

            _log($"Load page {numPage} is done");

            return result;
        }

        protected override void _clearWorkspaceInner(IState state)
        {
            _cleanDirectory(state.RootPath, state);
            _cleanFile($"{state.LogFilename}", state);
        }

        private void _log(string message, IState state = null)
        {
            _logBase(message, state is null?_state:state);
        }

        protected override void _checkWorkspaceInner(IState state)
        {
            _checkDirectory($"{state.RootPath}", state);
            _checkDirectory($"{state.PagesPath}", state);
            _checkDirectory($"{state.ItemsPath}", state);
        }

        private ScraperWinWinConfigModel _loadScraperConfig(ScraperWinWinStateModel state)
        {
            ScraperWinWinConfigModel result = null;
            var filename = state.ConfigFilename;

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<ScraperWinWinConfigModel>(File.ReadAllText(filename));
            else
            {
                result = new ScraperWinWinConfigModel();
                _saveScraperConfig(result, state);
            }

            return result;
        }

        private void _saveScraperConfig(ScraperWinWinConfigModel config, ScraperWinWinStateModel state)
        {
            var configFilename = state.ConfigFilename;

            File.WriteAllText(configFilename, JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));

            _log($"Save config:{configFilename} is done");
        }

        public ScraperWinWinStatusModel StatusWorkspace()
        {
            var state = (ScraperWinWinStateModel)_state;

            state.WorkPhase = "StatusWorkspace";

            _log("Start");

            var scrapeDate = _statusWorkspace_ScrapeDateBase(state);
            var amountItemsFromPath = _statusWorkspace_AmountItemsFromPathBase(state);
            var amountPages = _statusWorkspace_AmountPagesBase(state);
            var amountItemsFromPages = _statusWorkspace_AmountItemsFromPages(state);
            var amountItemDuplicatesFromPages = _statusWorkspace_AmountItemDuplicatesFromPages(state);

            var status = new ScraperWinWinStatusModel()
            {
                ScrapeDate = scrapeDate,
                AmountItemsFromPath = amountItemsFromPath,
                AmountPages = amountPages,
                AmountItemsFromPages = amountItemsFromPages,
                AmountItemUniquesFromPages = amountItemDuplicatesFromPages,
            };

            _log("End");

            return status;
        }

        private int _statusWorkspace_AmountItemDuplicatesFromPages(ScraperWinWinStateModel state)
        {
            var list = _statusWorkspace_AmountItemsFromPages_GetItems(state);

            var dups = list.GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.First().Id, x => x.First().Done);

            return dups.Count();
        }

        private int _statusWorkspace_AmountItemsFromPages(ScraperWinWinStateModel state)
        {
            var list = _statusWorkspace_AmountItemsFromPages_GetItems(state);

            //var dubs = list.GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.First().Id, x => x.First().Done);

            return list.Count();
        }

        private List<ItemTest> _statusWorkspace_AmountItemsFromPages_GetItems(ScraperWinWinStateModel state)
        {
            var listPages = _statusWorkspace_AmountPages_GetFilesBase(state);

            var totalItems = 0;
            var list = new List<ItemTest>();

            foreach (var page in listPages)
            {
                var filename = page.FullName;
                var pageData = JsonConvert.DeserializeObject<List<ShortItemDtoModel>>(File.ReadAllText(filename));
                var listItems = pageData.Select(x => new ItemTest() { Id = x.ItemId, Done =false }).ToList();
                list.AddRange(listItems);
                totalItems += listItems.Count;
            }

            return list;
        }

        public void PrintStatus(ScraperWinWinStatusModel status)
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

        public WinWinRepository GetRepository()
        {
            var result = new WinWinRepository();

            return result;
        }
    }
}