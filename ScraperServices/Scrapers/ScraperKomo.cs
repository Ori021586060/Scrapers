﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using ScraperCore;
using ScraperModels.Models.Excel;
using ScraperModels.Models.KomoDto;
using ScraperServices.Models.Komo;
using ScraperServices.Services;
using ScraperModels.Models;
using ScrapySharp.Extensions;
using ScraperModels;
using ScraperRepositories.Repositories;
using ScraperModels.Models.Domain;

namespace ScraperServices.Scrapers
{
    public class ScraperKomo: ScraperBase
    {
        //private ScraperKomoStateModel _state { get; set; }
        private ScraperKomoConfigModel _config { get; set; }
        private SelenoidStateModel _selenoidState { get; set; }

        public ScraperKomo(ScraperKomoStateModel state = null)
        {
            if (state is null) state = new ScraperKomoStateModel();

            _state = state;

            _config = _initConfig((ScraperKomoStateModel)state);
        }

        public override async Task<DataDomainModel> GetDomainModelAsync()
        {
            var domainModel = ScrapePhase5Async((ScraperKomoStateModel)_state);

            var result = new DataDomainModel()
            {
                Scraper = EnumScrapers.Komo,
                Data = await domainModel,
            };

            return result;
        }

        public ExcelKomoService GetExcelService()
        {
            return new ExcelKomoService((ScraperKomoStateModel)_state);
        }

        protected override async Task<bool> ScrapeInnerAsync()
        {
            var state = (ScraperKomoStateModel)_state;
            SetWorkPhaseBase($"Scraper", state);

            var result = false;

            try
            {
                if (_state.IsNew) _clearWorkspace(state);

                _checkWorkspace(state);

                await _getSessionSerial(state);

                await ScrapePhase1_GenerateListCitiesAsync(state);

                await ScrapePhase2_GenerateListStreetsAsync(state);

                ScrapePhase3_DownloadPages(state);

                ScrapePhase4_GenerateListItems(state);

                await ScrapePhase5_DownloadItems(state);

                _log($"Scraper Komo done (isNew:{state.IsNew})");

                result = true;
            }
            catch (Exception exception) {
                _log($"Error-2. {exception.Message} / {exception.StackTrace}");
            }

            return result;
        }

        private async Task<List<AdItemKomoDomainModel>> ScrapePhase5Async(ScraperKomoStateModel state)
        {
            SetWorkPhaseBase($"DomainModel", state);
            var listRowsDomainModel = new List<AdItemKomoDomainModel>();
            var files = GetListItemFiles(state);

            foreach (var file in files)
            {
                var filename = file.FullName;
                var data = JsonConvert.DeserializeObject<ItemKomoDtoModel>(await File.ReadAllTextAsync(filename));
                var rowDomainModel = new AdItemKomoDomainModel().FromDto(data);
                if (rowDomainModel != null)
                    listRowsDomainModel.Add(rowDomainModel);
            }

            _log($"Done");

            return listRowsDomainModel;
        }

        //private FileInfo[] GetListItemFiles(ScraperKomoStateModel state)
        //{
        //    var files = new DirectoryInfo(state.ItemsPath).GetFiles();

        //    return files;
        //}

        private Dictionary<string, bool> _loadListItems(ScraperKomoStateModel state)
        {
            var listItems = new Dictionary<string, bool>();

            var filename = $"{state.ListItemsFilename}";

            if (File.Exists(filename))
                listItems = JsonConvert.DeserializeObject<Dictionary<string, bool>>(File.ReadAllText(filename));
            else
            {
                _saveListItems(listItems, state);
            }

            return listItems;
        }

        private void _saveListItems(Dictionary<string, bool> list, ScraperKomoStateModel state)
        {
            var filename = $"{state.ListItemsFilename}";

            File.WriteAllText(filename, JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
        }

        private void ScrapePhase4_GenerateListItems(ScraperKomoStateModel state)
        {
            SetWorkPhaseBase($"phase-4", state);

            var duplacates = 0;
            var pageFiles = new DirectoryInfo(state.PagesPath).GetFiles();
            var listItems = _loadListItems(state);

            foreach (var pageFile in pageFiles)
            {
                var filename = pageFile.FullName;
                var listItemsOnPage = _loadItemsFromStorePage(filename, state);

                if (listItemsOnPage != null)
                {
                    foreach (var item in listItemsOnPage)
                    {
                        if (!listItems.ContainsKey(item.Key))
                            listItems.Add(item.Key, false);
                        else
                            duplacates++;
                    }
                }
            }
            _saveListItems(listItems, state);

            _log($"Found {duplacates} duplicates from file list-items.json");
            _log($"Check existing files");

            var addedFiles = 0;
            var fixedFiles = 0;
            var itemsFiles = new DirectoryInfo(state.ItemsPath).GetFiles();
            foreach (var itemFile in itemsFiles)
            {
                var filename = itemFile.Name;
                var id = Path.GetFileNameWithoutExtension(filename);
                var listItemsOnPage = _loadItemsFromStorePage(filename, state);
                if (!listItems.ContainsKey(id))
                {
                    listItems.Add(id, true);
                    addedFiles++;
                }
                else
                {
                    if (listItems[id] == false)
                    {
                        listItems[id] = true;
                        fixedFiles++;
                    }
                }
            }
            _log($"Added {addedFiles}, fixed {fixedFiles} items");
            _saveListItems(listItems, state);

            LogDone(state);
        }

        private async Task ScrapePhase5_DownloadItems(ScraperKomoStateModel state)
        {
            state.WorkPhase = "phase-5";
            _log($"Start");

            _initSelenoid(state);

            if (string.IsNullOrEmpty(state.SessionSerial))
            {
                _log($"Wait SessionSerial");
                Thread.Sleep(1000 * 5);
            }

            var listItems = _loadListItems(state);
            List<Task> tasks = new List<Task>();
            var countParsedItems = 0;
            var needToDo = listItems.Where(x => !x.Value).Select(x => x.Key).ToList();
            _log($"Need to do {needToDo.Count()} from {listItems.Count()}");

            foreach (var itemId in needToDo)
            {
                //string itemIdClear;
                //DataPageDtoModel content = null;
                //DataContactsDtoModel contacts = null;
                //DataCoordinatesDtoModel coordinates = null;
                //try
                //{
                //    itemIdClear = itemId.Replace("modaaRow", "");
                //    var page = GetKomoPageData_WebClient(itemIdClear, state).Result;

                //    content = _downloadItemContent(itemIdClear, state);
                //    contacts = _downloadItemContacts(itemIdClear, state);
                //    coordinates = _downloadItemCoordinates_Selenoid(itemIdClear, content, state);
                //    //var coordinates2 = _downloadItemCoordinates_WebClient(page, state).Result;
                //}
                //catch (Exception exception)
                //{
                //    _log($"Error-x1. {exception.Message} / {exception.StackTrace}");
                //}

                //if (contacts.response == "Captcha not match")
                //{
                //    ;
                //}

                //ItemKomoDtoModel itemModel = null;

                //try
                //{
                //    itemModel = new ItemKomoDtoModel()
                //    {
                //        Id = itemId.Replace("modaaRow", ""),
                //        DataPage = content,
                //        DataContacts = contacts,
                //        DataCoordinates = coordinates,
                //    };
                //}
                //catch (Exception exception)
                //{
                //    _log($"Error-x2. {exception.Message} / {exception.StackTrace}");
                //}

                ItemKomoDtoModel itemModel = await GetItemDtoAsync(itemId, state);

                var isOk = await _saveItemDtoModelAsync(itemModel, state);

                if (isOk)
                {
                    listItems[itemId] = true;
                }
                countParsedItems++;
                if (countParsedItems % 10 == 0)
                {
                    _log($"Parsed {countParsedItems} items");
                    _saveListItems(listItems, state);
                }
            }

            _saveListItems(listItems, state);
            _log($"End");
        }

        public async Task<ItemKomoDtoModel> GetItemDtoAsync(string itemId, ScraperKomoStateModel state=null)
        {
            ItemKomoDtoModel result = null;

            if (state is null) state = (ScraperKomoStateModel)_state;
            _initSelenoid(state);

            string itemIdClear;
            DataPageDtoModel content = null;
            DataContactsDtoModel contacts = null;
            DataCoordinatesDtoModel coordinates = null;

            try
            {
                itemIdClear = itemId.Replace("modaaRow", "");
                var page = await GetKomoPageData_WebClientAsync(itemIdClear, state);

                content = await _downloadItemContentAsync(itemIdClear, state);
                contacts = await _downloadItemContactsAsync(itemIdClear, state);
                coordinates = _downloadItemCoordinates_Selenoid(itemIdClear, content, state);
                //var coordinates2 = _downloadItemCoordinates_WebClient(page, state).Result;
            }
            catch (Exception exception)
            {
                _log($"Error-x1. {exception.Message} / {exception.StackTrace}");
            }

            if (contacts?.response == "Captcha not match")
            {
                ;
            }

            try
            {
                result = new ItemKomoDtoModel()
                {
                    Id = itemId.Replace("modaaRow", ""),
                    DataPage = content,
                    DataContacts = contacts,
                    DataCoordinates = coordinates,
                };
            }
            catch (Exception exception)
            {
                _log($"Error-x2. {exception.Message} / {exception.StackTrace}");
            }

            return result;
        }

        private void _initSelenoid(ScraperKomoStateModel state)
        {
            if (_selenoidState is null)
            {
                _selenoidState = new SelenoidStateModel();
                _initSelenoidBase(_selenoidState, state);
            }
        }

        private async Task<DataCoordinatesDtoModel> _downloadItemCoordinates_WebClient(HtmlDocument page, ScraperKomoStateModel state)
        {
            var apiKey = GetApiKey(page, state);
            var googlePlaceId = GetGooglePlaceId(page, state);
            var coordinatesJson = await GetGoogleCoordinates(apiKey, googlePlaceId, state);

            return coordinatesJson;
        }

        private async Task<DataCoordinatesDtoModel> GetGoogleCoordinates(string apiKey, string googlePlaceId, ScraperKomoStateModel state)
        {
            // https://maps.googleapis.com/maps/api/js/GeocodeService.Search?4s{googleplaceId}&7sIL&9siw&callback=_xdc_._937pm9&key=AIzaSyAWn5wP5WSqdLkiy2HuOFHezXXHG3RAcpQ&token=18641
            var url = $"https://maps.googleapis.com/maps/api/js/GeocodeService.Search?4s{googlePlaceId}&7sIL&9siw&callback=_xdc_._937pm9&key={apiKey}&token=18641";

            var data = await url.GetStringAsync();

            var result = JsonConvert.DeserializeObject<DataCoordinatesDtoModel>(data);

            return result;
        }

        private string GetGooglePlaceId(HtmlDocument page, ScraperKomoStateModel state)
        {
            //var googlePlaceId = "";
            // #mapCanvasW
            // attr - addr
            var googlePlaceId = page.DocumentNode.CssSelect("#mapCanvasW").FirstOrDefault().Attributes["addr"].Value;

            return googlePlaceId;
        }

        private string GetApiKey(HtmlDocument page, ScraperKomoStateModel state)
        {
            // key=AIzaSyAWn5wP5WSqdLkiy2HuOFHezXXHG3RAcpQ
            var apiKey = "AIzaSyAWn5wP5WSqdLkiy2HuOFHezXXHG3RAcpQ";
            // &key () &

            return apiKey;
        }

        private async Task<HtmlDocument> GetKomoPageData_WebClientAsync(string itemId, ScraperKomoStateModel state)
        {
            var url = $"https://www.komo.co.il/code/nadlan/?modaaNum={itemId}";
            var result = await GetPageData_WebClient(url, state);

            return result;
        }

        //private bool _processingFilePage(string filename, ScraperKomoStateModel state)
        //{
        //    var result = true;

        //    var listItemIds = _loadItemsFromStorePage(filename, state);
        //    var needToDo = listItemIds.Where(x => !x.Value).Select(x => x.Key).ToList();

        //    foreach (var itemId in needToDo)
        //    {
        //        var itemIdClear = itemId.Replace("modaaRow", "");
        //        var content = _downloadItemContent(itemIdClear, state);
        //        var contacts = _downloadItemContacts(itemIdClear, state);
        //        var coordinates = _downloadItemCoordinates(itemIdClear, content, state);

        //        if (contacts.response == "Captcha not match")
        //        {
        //            ;
        //        }

        //        var itemModel = new ItemKomoDtoModel()
        //        {
        //            Id = itemId.Replace("modaaRow", ""),
        //            DataPage = content,
        //            DataContacts = contacts,
        //            DataCoordinates = coordinates,
        //        };

        //        var isOk = _saveItemDtoModelAsync(itemModel, state).Result;

        //        isOk = false;
        //        if (isOk)
        //        {
        //            listItemIds[itemId] = true;
        //            _saveItemsFromStorePageAsync(listItemIds, filename, state).Wait();
        //        }
        //    }

        //    return result;
        //}
        //private async Task<bool> _processingFilePageAsync(string filename, ScraperKomoStateModel state)
        //{
        //    var result = true;

        //    var listItemIds = await _loadItemsFromStorePageAsync(filename, state);
        //    var needToDo = listItemIds.Where(x => !x.Value).Select(x => x.Key).ToList();

        //    foreach (var itemId in needToDo)
        //    {
        //        var content = await _downloadItemContentAsync(itemId.Replace("modaaRow", ""), state);
        //        var contacts = await _downloadItemContactsAsync(itemId.Replace("modaaRow", ""), state);
        //        var coordinates = _downloadItemCoordinates(itemId.Replace("modaaRow", ""), content, state);

        //        if (contacts.response == "Captcha not match")
        //        {
        //            ;
        //        }

        //        var itemModel = new ItemKomoDtoModel()
        //        {
        //            Id = itemId.Replace("modaaRow", ""),
        //            DataPage = content,
        //            DataContacts = contacts,
        //            DataCoordinates = coordinates,
        //        };

        //        var isOk = await _saveItemDtoModelAsync(itemModel, state);

        //        isOk = false;
        //        if (isOk)
        //        {
        //            listItemIds[itemId] = true;
        //            await _saveItemsFromStorePageAsync(listItemIds, filename, state);
        //        }
        //    }

        //    return result;
        //}

        public void PrintStatus(ScraperKomoStatusModel status)
        {
            try
            {
                _log($"ScrapeDate: {status.ScrapeDate}");
                _log($"AmountItemsFromPath: {status.AmountItemsFromPath}");
                _log($"AmountPages: {status.AmountPages}");
                _log($"AmountItemsFromPages: {status.AmountItemsFromPages}");
                _log($"AmountItemDuplicatesFromPages: {status.AmountItemDuplicatesFromPages}");
                _log($"AmountItemUnicsFromPages: {status.AmountItemUniquesFromPages}");
                _log($"AmountCities: {status.AmountCities}");
                _log($"AmountItemsFromCitiesStreets: {status.AmountItemsFromCitiesStreets}");
            } catch (Exception exception)
            {
                _log($"No status. {exception.Message}");
            }
        }

        public bool SaveStatus(ScraperKomoStatusModel status)
        {
            var result = false;

            if (status != null)
                result = SaveStatusBase(status, _state);

            return result;
        }

        public ScraperKomoStatusModel StatusWorkspace()
        {
            var state = (ScraperKomoStateModel)_state;
            ScraperKomoStatusModel status = null;

            SetWorkPhaseBase("StatusWorkspace", state);

            try
            {
                var scrapeDate = _statusWorkspace_ScrapeDateBase(state);
                var amountItemsFromPath = _statusWorkspace_AmountItemsFromPathBase(state);
                var amountPages = _statusWorkspace_AmountPagesBase(state);
                var amountItemsFromPages = _statusWorkspace_AmountItemsFromPages(state);
                var amountItemUniquesFromPages = _statusWorkspace_AmountItemUniquesFromPages(state);

                var amountCities = _statusWorkspace_AmountCities(state);
                var amountItemsFromCitiesStreets = _statusWorkspace_ItemsFromCitiesStreets(state);

                status = new ScraperKomoStatusModel()
                {
                    ScrapeDate = scrapeDate,
                    AmountItemsFromPath = amountItemsFromPath,
                    AmountItemUniquesFromPages = amountItemUniquesFromPages,
                    AmountCities = amountCities,
                    AmountItemsFromCitiesStreets = amountItemsFromCitiesStreets,
                    AmountPages = amountPages,
                    AmountItemsFromPages = amountItemsFromPages,
                };
            }catch (Exception exception) {
                _log($"Error-3. {exception.Message}");
            }

            LogDone(state);

            return status;
        }
        
        private int _statusWorkspace_AmountItemUniquesFromPages(ScraperKomoStateModel state)
        {
            var list = _statusWorkspace_AmountItemsFromPages_GetItems(state);

            var dups = list.GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.First().Id, x => x.First().Done);

            return dups.Count();
        }

        private int _statusWorkspace_AmountItemsFromPages(ScraperKomoStateModel state)
        {
            var list = _statusWorkspace_AmountItemsFromPages_GetItems(state);

            return list.Count();
        }

        private List<ItemTest> _statusWorkspace_AmountItemsFromPages_GetItems(ScraperKomoStateModel state)
        {
            var listPages = _statusWorkspace_AmountPages_GetFilesBase(state);

            var totalItems = 0;
            var list = new List<ItemTest>();

            foreach (var page in listPages)
            {
                var filename = page.FullName;
                var pageData = JsonConvert.DeserializeObject<Dictionary<string, bool>>(File.ReadAllText(filename));
                var listItems = pageData.Select(x => new ItemTest() { Id = x.Key, Done = x.Value }).ToList();
                list.AddRange(listItems);
                totalItems += listItems.Count;
            }

            return list;
        }

        private int _statusWorkspace_ItemsFromCitiesStreets(ScraperKomoStateModel state)
        {
            var path = state.CitiesPath;
            var listCityStreets = new DirectoryInfo($"{path}").GetFiles();

            var totalItems = 0;

            foreach(var cityStreet in listCityStreets)
            {
                //var filename = $"{state.CitiesPath}/city-{cityId}-streets.json";
                //var filename = cityStreet.Name;
                //var cityId = _getCityIdFromFilename(filename);
                var listStreets = JsonConvert.DeserializeObject<Dictionary<string, StreetsDataDtoModel>>(File.ReadAllText(cityStreet.FullName));
                var total = listStreets.Sum(x => int.Parse(x.Value.counter));
                totalItems += total;
            }

            return totalItems;
        }

        private int _statusWorkspace_AmountCities(ScraperKomoStateModel state)
        {
            state.WorkPhase = "AmountCities";

            var filename = state.ListCitiesFilename; // list-cities.json
            var listCities = JsonConvert.DeserializeObject<Dictionary<string, CitiesDataDtoModel>>(File.ReadAllText(filename));

            var result = listCities.Count;

            return result;
        }

        private async Task<bool> _saveItemDtoModelAsync(ItemKomoDtoModel item, ScraperKomoStateModel state)
        {
            var result = true;

            var filename = $"{state.ItemsPath}/{item.Id}.json";

            //_log($"Saving item: {filename}");

            await File.WriteAllTextAsync($"{filename}", JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented));

            //_log($"Save item done");

            return result;
        }

        private DataCoordinatesDtoModel _downloadItemCoordinates_Selenoid(string itemId, DataPageDtoModel content, ScraperKomoStateModel state)
        {
            var result = new DataCoordinatesDtoModel();
            
            //var googlePlaceId = content.GooglePlaceId;
            //_log($"Download coordinates for item {googlePlaceId}");

            //https://maps.googleapis.com/maps/api/js/GeocodeService.Search?4s{googleplaceId}&7sIL&9siw&callback=_xdc_._937pm9&key=AIzaSyAWn5wP5WSqdLkiy2HuOFHezXXHG3RAcpQ&token=18641
            //var url = $"https://maps.googleapis.com/maps/api/js/GeocodeService.Search?4s{googlePlaceId}&7sIL&9siw&callback=_xdc_._937pm9&key=AIzaSyAWn5wP5WSqdLkiy2HuOFHezXXHG3RAcpQ&token=18641";
            var url = $"https://www.komo.co.il/code/nadlan/?modaaNum={itemId}";
            //var dataFromGoogle = await url.GetStringAsync();

            var isDone = false;
            int count=0, countMax = 30;
            try
            {
                //_selenoidState.WindowMain.Navigate().GoToUrl(url);
                var isOk = Selenoid_GoToUrl_Base(url, _selenoidState, state);

                do
                {
                    isDone = false;
                    string isReady = (string)_selenoidState.WindowMain.ExecuteScript("return document.readyState;");
                    if (isReady == "complete")
                    {
                        isDone = true;
                    }
                    else
                    {
                        Thread.Sleep(1000 * 1);
                        count++;
                    }
                    if (count > countMax) isDone = true;
                } while (!isDone);
            }catch(Exception exception)
            {
                _log($"Error-j2. url={url} / {exception.Message} / {exception.StackTrace}");
            }

            ReadOnlyCollection<object> networkRequests = null;
            try
            {
                var script1 = $"var geocoder = new google.maps.Geocoder();" +
                          $"var addr = decodeURIComponent(\"{content.GooglePlaceId}\");" +
                          "geocoder.geocode( { 'address': addr}, function(results, status) { _results_ = results; return results; });";
                //"do { console.log(_results_); } while (_results_===\"123\")" +
                //$"return _results_";
                var network = "var performance = window.performance || window.mozPerformance || window.msPerformance || window.webkitPerformance || {}; var network = performance.getEntries() || {}; return network;";
                string res1 = (string)_selenoidState.WindowMain.ExecuteScript(script1);
                Thread.Sleep(1000);
                networkRequests = (ReadOnlyCollection<object>)_selenoidState.WindowMain.ExecuteScript(network);
            }
            catch (Exception exception)
            {
                _log($"Error-j3. {exception.Message} / {exception.StackTrace}");
            }

            //var urlCoordinates = name.Split(",")[1].Trim();
            try
            {
                foreach (var item in networkRequests)
                {
                    var t = item.GetType();
                    if (item is Dictionary<string, object>)
                    {
                        var rr = (item as Dictionary<string, object>).Where(x => x.Key == "name").FirstOrDefault();
                        if (rr.Value != null && (rr.Value as string).StartsWith("https://maps.googleapis.com/maps/api/js/GeocodeService.Search"))
                        {
                            url = rr.Value.ToString();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _log($"Error-j4. ItemId={itemId} / {exception.Message} / {exception.StackTrace}");
            }

            try
            { 
                var dataFromGoogle = url.GetStringAsync().Result;
                result = _detectCoordinates(dataFromGoogle, itemId);
            }
            catch (Exception exception)
            {
                _log($"Error-j5. {exception.Message} / {exception.StackTrace}");
            }

            return result;
        }

        private DataCoordinatesDtoModel _detectCoordinates(string data, string itemId)
        {
            DataCoordinatesDtoModel result = null;

            var cleanData = _cleanDataFromGoogleCoordinates(data, itemId);

            try
            {
                result = JsonConvert.DeserializeObject<DataCoordinatesDtoModel>(cleanData);
            }
            catch(Exception exception) {
                _log($"Error-4. itemId={itemId} / {exception.Message}");
            }

            return result;
        }

        private string _cleanDataFromGoogleCoordinates(string data, string itemId)
        {
            var result = "";

            try
            {
                var p1 = data.IndexOf('{');
                var p2 = data.LastIndexOf(')');

                result = data.Substring(p1, p2 - p1);
            }catch (Exception exception)
            {
                _log($"Error-e1. data={data} / itemId={itemId} / {exception.Message} / {exception.StackTrace}");
            }

            return result;
        }

        private async Task _getSessionSerial(ScraperKomoStateModel state)
        {
            var sessionSerial = "";

            _log($"Start _getSessionSerial");

            using (var cli = new FlurlClient("https://www.komo.co.il").EnableCookies())
            {
                var request = await cli.Request("code/users/login.asp").PostUrlEncodedAsync(new
                {
                    email = "dda101179@mail.ru",
                    password = "Zx123456",
                });

                sessionSerial = cli.Cookies.FirstOrDefault(x => x.Key == "sessionSerial").Value?.Value;
            }

            if (string.IsNullOrEmpty(sessionSerial)) _log($"!!! Alert. SessionSerial didnt getting");
            else
                _log($"SessionSerial is {sessionSerial}");

            state.SessionSerial = sessionSerial;
            //return sessionSerial;
        }

        //private DataContactsDtoModel _downloadItemContacts(string itemId, ScraperKomoStateModel state)
        //{
        //    DataContactsDtoModel result = null;
        //    DataContactsDtoModel resultData = null;

        //    if (!string.IsNullOrEmpty(state.SessionSerial))
        //    {
        //        var url = "https://www.komo.co.il/api/modaotActions/showPhone.api.asp";

        //        try
        //        {
        //            resultData = url
        //                .WithCookie(new Cookie("sessionSerial", state.SessionSerial))
        //                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
        //                .PostStringAsync($"luachNum=2&modaaNum={itemId}")
        //                .ReceiveJson<DataContactsDtoModel>()
        //                .Result;
        //        }
        //        catch (Exception exception)
        //        {
        //            _log($"Error j1. {exception.Message}");
        //        }

        //        result = resultData;
        //    }
        //    else
        //    {
        //        _log($"SessionSerial is null");
        //    }

        //    return result;
        //}

        private async Task<DataContactsDtoModel> _downloadItemContactsAsync(string itemId, ScraperKomoStateModel state)
        {
            DataContactsDtoModel result = null;

            if (!string.IsNullOrEmpty(state.SessionSerial))
            {
                var url = "https://www.komo.co.il/api/modaotActions/showPhone.api.asp";

                result = await url
                    .WithCookie(new Cookie("sessionSerial", state.SessionSerial))
                    .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                    .PostStringAsync($"luachNum=2&modaaNum={itemId}")
                    .ReceiveJson<DataContactsDtoModel>();
            }
            else
            {
                _log($"SessionSerial is null");
            }

            return result;
        }

        private DataContactsDtoModel _scrapeDataContacts2Dto(HtmlDocument document)
        {
            var price = document.DocumentNode.CssSelect(".modaaWPrice .ModaaWDetailsValue").FirstOrDefault()?.InnerText;

            var dataDto = new DataContactsDtoModel()
            {
                
            };

            return dataDto;
        }

        private async Task _saveItemsFromStorePageAsync(Dictionary<string, bool> list, string filename, ScraperKomoStateModel state)
        {
            _log($"Saving page state: {filename}");

            await File.WriteAllTextAsync($"{filename}", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
        }

        private async Task<DataPageDtoModel> _downloadItemContentAsync(string itemId, ScraperKomoStateModel state)
        {
            //_log($"Download item {itemId}");

            DataPageDtoModel result = null;
            // https://www.komo.co.il/code/nadlan/?modaaNum=3033580

            var webGet = new HtmlWeb();

            var url = $"https://www.komo.co.il/code/nadlan/?modaaNum={itemId}";
            if (await webGet.LoadFromWebAsync(url) is HtmlDocument document)
            {
                result = _scrapeDataPage2Dto(document);
            }

            return result;
        }

        //private async Task<DataPageDtoModel> _downloadItemContentAsync(string itemId, ScraperKomoStateModel state)
        //{
        //    _log($"Download item {itemId}");

        //    DataPageDtoModel result = null;
        //    // https://www.komo.co.il/code/nadlan/?modaaNum=3033580

        //    var webGet = new HtmlWeb();

        //    var url = $"https://www.komo.co.il/code/nadlan/?modaaNum={itemId}";
        //    if (await webGet.LoadFromWebAsync(url) is HtmlDocument document)
        //    {
        //        result = _scrapeDataPage2Dto(document);
        //    }

        //    return result;
        //}

        private DataPageDtoModel _scrapeDataPage2Dto(HtmlDocument document)
        {
            var price = document.DocumentNode.CssSelect(".modaaWPrice .ModaaWDetailsValue").FirstOrDefault()?.InnerText;

            var contactName = document.DocumentNode.CssSelect(".modaaWContactArea .ModaaWDetailsValue").FirstOrDefault()?.InnerText;

            var minisite = document.DocumentNode.CssSelect(".modaaWMinisite .pratiName").FirstOrDefault()?.InnerText;

            var description = document.DocumentNode.CssSelect(".modaaWTeurArea .modaaWTeur1")
                .Skip(1)
                .FirstOrDefault()
                ?.InnerText;

            var extDescription = document.DocumentNode.CssSelect(".modaaWTosafot .ModaaWDetailsValue")
                .FirstOrDefault()
                ?.ChildNodes
                .Select(x=>x.InnerText)
                .ToList();

            var title = document.DocumentNode.CssSelect(".modaaWTitleArea h1").FirstOrDefault()?.InnerText;

            var listOptions = document.DocumentNode.CssSelect(".modaaWTitleArea h2 li").ToList();
            var rooms = listOptions.Skip(0).Take(1).Select(x => x.InnerText).FirstOrDefault();
            var floor = listOptions.Skip(1).Take(1).Select(x => x.InnerText).FirstOrDefault();
            var square = listOptions.Skip(2).Take(1).Select(x => x.InnerText).FirstOrDefault();
            var checkHour = listOptions.Skip(3).Take(1).Select(x => x.InnerText).FirstOrDefault();

            var googlePlaceId = document.DocumentNode.CssSelect("#mapCanvasW").FirstOrDefault()?.GetAttributeValue("addr");

            var files = document.DocumentNode.CssSelect(".modaaWPhotoGallery .ModaaWPhoto").ToList();
            var images = files.Select(x => x.GetAttributeValue("src")).ToList();
            var updated = document.DocumentNode.CssSelect(".modaaWTime")?.FirstOrDefault()?.InnerText;

            var dataDto = new DataPageDtoModel() {
                Updated = updated,
                Price = price,
                ContactName = contactName,
                Minisite = minisite,
                Description = description,
                ExtDescription = extDescription,
                Title = title,
                Rooms = rooms,
                Floor = floor,
                Square = square,
                CheckHour = checkHour,
                GooglePlaceId = googlePlaceId,
                Images = images,
            };

            return dataDto;
        }

        private Dictionary<string, bool> _loadItemsFromStorePage(string filename, ScraperKomoStateModel state)
        {
            Dictionary<string, bool> result = null;

            _log($"Load list-items-from-store-page: {filename}");

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<Dictionary<string, bool>>(File.ReadAllText(filename));

            return result;
        }

        private async Task<Dictionary<string, bool>> _loadItemsFromStorePageAsync(string filename, ScraperKomoStateModel state)
        {
            Dictionary<string, bool> result = null;

            _log($"Load list-items-from-store-page: {filename}");

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<Dictionary<string, bool>>(await File.ReadAllTextAsync(filename));

            return result;
        }

        private void ScrapePhase3_DownloadPages(ScraperKomoStateModel state)
        {
            SetWorkPhaseBase($"Phase-3", state);

            var files = new DirectoryInfo(state.CitiesPath).GetFiles(); // cities/city-[id]-streets.json
            List<Task> tasks = new List<Task>();
            var countTask = 0;
            var countTaskMax = 20;

            foreach (var file in files)
            {
                var filename = file.Name;

                //_log($"Start Task {countTask}");

                tasks.Add(_processingFileCityStreetsAsync(filename, state));

                countTask++;
                if (countTask >= countTaskMax)
                {
                    _log($"Wait task are done");
                    Task.WaitAll(tasks.ToArray());

                    tasks.Clear();
                    countTask = 0;
                }
            }

            LogDone(state);
        }

        private string _getCityIdFromFilename(string filename)
        {
            var cityId = "";
            try
            {
                cityId = filename.Split("-")[1];
            }
            catch (Exception exception)
            {
                _log($"Error ex-1. {exception.Message}");
            }

            return cityId;
        }

        private async Task<bool> _processingFileCityStreetsAsync(string filename, ScraperKomoStateModel state)
        {
            var result = true;
            var cityId = _getCityIdFromFilename(filename);

            var listStreets = await _loadListStreetsByCityAsync(cityId, state);

            var needToDo = GetNeedToDoListStreets(listStreets, state); //listStreets.Where(x => !x.Value.Scraped).Select(x => x.Value).ToList();

            foreach (var street in needToDo)
            {
                var streetId = street.num;
                var isOk = await _downloadItemsByStreetIdCityIdAsync(cityId, streetId, state);
                if (isOk)
                {
                    listStreets[streetId].Scraped = true;
                    //_saveListStreetsByCity(listStreets, cityId, state);
                }
            }

            if (needToDo.Count>0)
                await _saveListStreetsByCityAsync(listStreets, cityId, state);

            return result;
        }

        private List<StreetsDataDtoModel> GetNeedToDoListStreets(Dictionary<string, StreetsDataDtoModel> listStreets, ScraperKomoStateModel state)
        {
            List<StreetsDataDtoModel> needToDo = null;

            try
            {
                needToDo = listStreets.Where(x => !x.Value.Scraped).Select(x => x.Value).ToList();
            } catch (Exception exception)
            {
                _log($"Error d1. {exception.Message}");
            }

            return needToDo;
        }

        private async Task<bool> _downloadItemsByStreetIdCityIdAsync(string cityId, string streetId, ScraperKomoStateModel state)
        {
            var result = true;
            var page = 1;
            var pageTotal = 0;

            do
            {
                var pageHtml = GetPageByCityStreetAsync(cityId, streetId, page, state);
                //var url = $"https://www.komo.co.il/code/nadlan/apartments-for-rent.asp?cityNum={cityId}&streetNum={streetId}&&currPage={page}&subLuachNum=1";
                if (await pageHtml is HtmlDocument document)
                {
                    pageTotal = _detectTotalPages(document);

                    var listAds = _scrapeAds(document);

                    state.TotalScrapedAds += listAds.Count;
                    var msg = $"Scrape {listAds.Count} ads, total acraped ads:{state.TotalScrapedAds}";
                    _log(msg);
                    _logStat(msg);

                    await _savePageByCityIdStreetIdAsync(listAds, cityId, streetId, page, state);
                }

                page++;

            } while (page <= pageTotal);

            return result;
        }

        private async Task<HtmlDocument> GetPageByCityStreetAsync(string cityId, string streetId, int page, ScraperKomoStateModel state)
        {
            HtmlDocument result = null;
            var webGet = new HtmlWeb();
            var url = $"https://www.komo.co.il/code/nadlan/apartments-for-rent.asp?cityNum={cityId}&streetNum={streetId}&&currPage={page}&subLuachNum=1";

            try
            {
                result = await webGet.LoadFromWebAsync(url);
            } catch(Exception exception)
            {
                _log($"Error g5. {exception.Message}");
            }

            return result;
        }

        private void _logStat(string message)
        {
            lock (ScraperKomoStateModel.LockWriteToStatLog)
            {
                File.AppendAllText(_state.LogStatFilename, $"{message}\r\n");
            }
        }

        private async Task _savePageByCityIdStreetIdAsync(Dictionary<string, bool> list, string cityId, string streetId, int page, ScraperKomoStateModel state)
        {
            var filename = $"{state.PagesPath}/city-{cityId}-{streetId}-{page}.json";

            _log($"Save ads from {page} page city={cityId}, street={streetId}: {filename}");

            await File.WriteAllTextAsync($"{filename}", JsonConvert.SerializeObject(list, Formatting.Indented));

            //_log($"Save as done");
        }

        private Dictionary<string, bool> _scrapeAds(HtmlDocument document)
        {
            var result = new Dictionary<string, bool>();

            try
            {
                var nodes = document.DocumentNode.CssSelect(".tblModaa").ToList();
                foreach (var node in nodes)
                {
                    var id = node.Attributes["id"].Value.Replace("modaaRow", "");
                    result.Add(id, false);
                }
            } catch(Exception exception)
            {
                _log($"Error j2. {exception.Message}");
            }

            return result;
        }

        private int _detectTotalPages(HtmlDocument document)
        {
            var result = 1;

            try
            {
                var pager = document.DocumentNode.CssSelect("#paging").FirstOrDefault().ChildNodes;

                if (pager.Count > 0)
                {
                    result = pager.Count - 1;
                }
            } catch(Exception exception)
            {
                _log($"Error x2. {exception.Message}");
            }

            return result;
        }

        private async Task ScrapePhase2_GenerateListStreetsAsync(ScraperKomoStateModel state)
        {
            SetWorkPhaseBase($"GenerateListStreets", state);
            _log($"Start generate list-streets (isNew:{_state.IsNew})");

            var listCities = await _loadListCitiesAsync(state); // list-cities.json
            fixListCities(listCities, state);

            var ads = listCities.Sum(x => int.Parse(x.Value.counter));
            _log($"Total ads {ads}");
            var tasks = new List<Task<bool>>();
            var maxTasks = 100;

            Func<int,List<string>> NeedToDo = (i) => listCities.Where(x => !x.Value.Scraped).Select(x => x.Key).Take(i).ToList();
            var needToDo = NeedToDo(maxTasks);

            do {
                foreach (var cityId in needToDo)
                {
                    var id = cityId;
                    listCities[id].Scraped = true;
                    tasks.Add(Task.Run(async () => listCities[id].Scraped = await LoadSaveListStreetsAsync(id, state)));
                }

                Thread.Sleep(1000 * 7);
                Task.WaitAny(tasks.ToArray());

                tasks.RemoveAll(x => x.IsCompleted);
                await _saveListCitiesAsync(listCities, state);
                needToDo = NeedToDo(maxTasks - tasks.Count());
            } while (needToDo.Count()>0);

            _log($"Generate list-streets done");
        }

        private async Task<bool> LoadSaveListStreetsAsync(string cityId, ScraperKomoStateModel state)
        {
            var result = false;

            try
            {
                var listStreetsByCity = await _downloadListStreetsByCityIdAsync(cityId, state);
                await _saveListStreetsByCityAsync(listStreetsByCity, cityId, state);
                result = true;
            } catch(Exception exception)
            {
                result = false;
                _log($"Error-p1. {exception.Message}");
            }

            return result;
        }

        private void fixListCities(Dictionary<string, CitiesDataDtoModel> listCities, ScraperKomoStateModel state)
        {
            var path = $"{state.CitiesPath}";
            var listfiles = new DirectoryInfo(path).GetFiles();
            var fixedFiles = 0;
            foreach (var city in listCities) city.Value.Scraped = false;

            foreach (var file in listfiles)
            {
                var cityId = Path.GetFileNameWithoutExtension(file.Name).Split("-")[1];
                if (listCities.ContainsKey(cityId) && !listCities[cityId].Scraped)
                {
                    listCities[cityId].Scraped = true;
                    fixedFiles++;
                }
            }

            _log($"Fixed {fixedFiles} files");
        }

        private async Task _saveListStreetsByCityAsync(Dictionary<string, StreetsDataDtoModel> list, string cityId, ScraperKomoStateModel state)
        {
            var filename = $"{state.CitiesPath}/city-{cityId}-streets.json";

            //_log($"Saving list-streets: {filename}");

            await File.WriteAllTextAsync($"{filename}", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
        }

        private async Task<Dictionary<string, StreetsDataDtoModel>> _loadListStreetsByCityAsync(string cityId, ScraperKomoStateModel state)
        {
            var filename = $"{state.CitiesPath}/city-{cityId}-streets.json";
            Dictionary<string, StreetsDataDtoModel> listStreets = null;

            //_log($"Load list-streets: {filename}");
            try
            {
                listStreets = JsonConvert.DeserializeObject<Dictionary<string, StreetsDataDtoModel>>(await File.ReadAllTextAsync(filename));

                _log($"Load list-streets {filename} done");
            }
            catch (Exception exception)
            {
                _log($"Error ex-2. {exception.Message}");
            }

            return listStreets;
        }

        private async Task<Dictionary<string, StreetsDataDtoModel>> _downloadListStreetsByCityIdAsync(string cityId, ScraperKomoStateModel state)
        {
            _log($"Download streets by city {cityId}");

            Dictionary<string, StreetsDataDtoModel> result = new Dictionary<string, StreetsDataDtoModel>();

            var url = $"https://www.komo.co.il/api/nadlan/list-streets.api.asp?limit=5000&hideAlter=y&showCounters=y&subLuachNum=1&cityNum={cityId}";

            var listCitiesRawJson = await _downloadObjectAsync(url, state);
            var mark = "%$#_";
            var listCitiesJson = listCitiesRawJson.Replace("\"\\", mark).Replace($":{mark}", ":\"").Replace(mark, "\\");

            var listStreets = JsonConvert.DeserializeObject<StreetsDtoModel>(listCitiesJson);

            foreach (var streetDto in listStreets.data)
                result.Add(streetDto.num, streetDto);

            _log($"Downlaod streets by city {cityId} done");

            return result;
        }

        private async Task ScrapePhase1_GenerateListCitiesAsync(ScraperKomoStateModel state)
        {
            SetWorkPhaseBase($"GenerateListCities", state);

            var listCities = await _loadListCitiesAsync(state); 

            if (listCities == null || listCities.Count == 0 || state.IsNew)
            {
                _log($"Generating new list-cities (isNew:{state.IsNew}), filename:{state.ListCitiesFilename}");

                listCities = await _downloadListCitiesAsync(state);

                _log($"Generate new list-cities done");

                await _saveListCitiesAsync(listCities, state);
            }
            else
                _log($"Generate list-cities not need (missing, isNew:{state.IsNew})");

            LogDone(state);
        }

        private async Task<Dictionary<string,CitiesDataDtoModel>> _downloadListCitiesAsync(ScraperKomoStateModel state)
        {
            Dictionary<string, CitiesDataDtoModel> result = new Dictionary<string, CitiesDataDtoModel>();

            var url = "https://www.komo.co.il/api/nadlan/list-cities.api.asp?limit=5000&hideAlter=y&showCounters=y&subLuachNum=1";

            var listCitiesRawJson = await _downloadObjectAsync(url, state);
            var mark = "%$#_";
            var listCitiesJson = listCitiesRawJson.Replace("\"\\",mark).Replace($":{mark}",":\"").Replace(mark,"\\");

            var listCities = JsonConvert.DeserializeObject<CitiesDtoModel>(listCitiesJson);

            foreach (var cityDto in listCities.data)
                result.Add(cityDto.num, cityDto);

            return result;
        }

        private async Task<string> _downloadObjectAsync(string url, ScraperKomoStateModel state)
        {
            var result = "";

            try
            {
                var request = await url.GetStringAsync();

                result = request;
            }
            catch (Exception exception)
            {
                result = "";
                _log($"Exception-1. {exception.Message}");
            }

            return result;
        }

        private async Task<T> _downloadObject<T>(string url, ScraperKomoStateModel state)
        {
            T result;

            try
            {
                var request = await url.GetJsonAsync<T>();

                result = request;
            }
            catch (Exception exception)
            {
                result = default(T);
                _log($"Exception-3. {exception.Message}");
            }

            return result;
        }

        private async Task _saveListCitiesAsync(Dictionary<string, CitiesDataDtoModel> list, ScraperKomoStateModel state)
        {
            var filename = state.ListCitiesFilename;

            _log($"Saving list-cities: {filename}");

            await File.WriteAllTextAsync($"{filename}", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));

            _log($"Save list-cities done");
        }

        private async Task<Dictionary<string, CitiesDataDtoModel>> _loadListCitiesAsync(ScraperKomoStateModel state)
        {
            Dictionary<string, CitiesDataDtoModel> result = null;

            var filename = state.ListCitiesFilename; 

            _log($"Loading list-cities: {filename}");

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<Dictionary<string, CitiesDataDtoModel>>(await File.ReadAllTextAsync(filename));

            _log($"Load list-cities done");

            return result;
        }

        private void _cleanFile(string filename)
        {
            _log($"Clean/delete file {filename}");

            if (File.Exists(filename)) File.Delete(filename);
        }

        //private void _log(string message)
        //{
        //    var logMessage = $"{DateTime.UtcNow.ToString()} | {_state.TypeScraper.ToString()} |  {_state.WorkPhase} | {message}";

        //    Console.WriteLine(logMessage);

        //    //File.AppendAllText(_logFile, $"{logMessage}\r\n");
        //}

        private void _log(string message, IState state = null)
        {
            _logBase(message, state is null ? _state : state);
        }

        //private void _checkDirectory(string directory)
        //{
        //    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        //}

        protected override void _clearWorkspaceInner(IState state)
        {
            _log($"Cleaning workspace");

            _cleanDirectory(state.RootPath);

            _cleanFile($"{state.LogFilename}");

            _log($"Clear workspace done");
        }

        private void _cleanDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                var dirInfo = new DirectoryInfo(path);

                _log($"Cleaning directory: {dirInfo.FullName}");

                foreach (var dir in dirInfo.GetDirectories()) dir.Delete(recursive: true);

                foreach (var file in dirInfo.GetFiles()) file.Delete();

                _log("Cleaning directory done");
            }
        }

        protected override void _checkWorkspaceInner(IState state)
        {
            _log($"Start check workspace");

            _checkDirectory($"{state.RootPath}");

            _checkDirectory($"{((ScraperKomoStateModel)state).CitiesPath}");

            _checkDirectory($"{state.PagesPath}");

            _checkDirectory($"{state.ItemsPath}");

            _log($"Check workspace done");
        }

        private ScraperKomoConfigModel _initConfig(ScraperKomoStateModel state)
        {
            ScraperKomoConfigModel result = null;
            var filename = state.ConfigFilename;

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<ScraperKomoConfigModel>(File.ReadAllText(filename));
            else
            {
                result = new ScraperKomoConfigModel();
                _saveConfig(result, state);
            }

            return result;
        }

        private void _saveConfig(ScraperKomoConfigModel config, ScraperKomoStateModel state)
        {
            var configFilename = state.ConfigFilename;
            _log($"Start saving config:{configFilename}");

            File.WriteAllText(configFilename, JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));

            _log($"Save config:{configFilename} is done");
        }

        //private void _initSelenoid(ScraperKomoStateModel state)
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
        //        _log($"\tInit Selenoid Service-{state.UsedSelenoidService}:{selenoidUrl}");

        //        try
        //        {
        //            if (_windowMain != null)
        //            {
        //                _log($"SelenoidService isnt null");
        //                _windowMain.Quit();
        //                _log($"Quit SelenoidService done");
        //            }

        //            _printSelenoidServiceParams(selenoidService, state);

        //            doNeedRepeatRequest = false;

        //            _windowMain = new RemoteWebDriver(new Uri(selenoidUrl), options.ToCapabilities(), TimeSpan.FromMinutes(5));

        //            _waitMain = new WebDriverWait(_windowMain, TimeSpan.FromSeconds(120));
        //        }
        //        catch (Exception exception)
        //        {
        //            countRepeat++;
        //            if (countRepeat < countRepeatMax)
        //            {
        //                doNeedRepeatRequest = true;
        //                _log($"\t!!! Need change Selenoid Service !!!");
        //                _log($"\tBad config Selenoid Service-{state.UsedSelenoidService}:");
        //                _printSelenoidServiceParams(selenoidService, state);
        //            }
        //            else
        //                _log($"Try is out. Error");
        //        }

        //        if (doNeedRepeatRequest)
        //        {
        //            _log($"\tcountRepeat:{countRepeat}");
        //            state.UsedSelenoidService++;
        //            //Thread.Sleep(TimeSpan.FromMinutes(10));
        //        }

        //    } while (doNeedRepeatRequest);

        //}

        //private SelenoidConfigModel _selectSelenoidService(ScraperKomoStateModel state)
        //{
        //    _initConfig(state);

        //    Func<SelenoidConfigModel> GetSelenoidService = () => _config.Selenoid
        //                .Where(x => x.Enabled)
        //                .Skip(state.UsedSelenoidService)
        //                .FirstOrDefault();

        //    var selenoidService = GetSelenoidService();

        //    if (selenoidService is null)
        //    {
        //        state.UsedSelenoidService = 0;
        //        selenoidService = GetSelenoidService();
        //    }

        //    return selenoidService;
        //}

        //private void _printSelenoidServiceParams(SelenoidConfigModel service, ScraperKomoStateModel state)
        //{
        //    _log($"\tParams Selenoid:");
        //    _log($"\t\tDns:{service.Address}");
        //    _log($"\t\tIp:{Dns.GetHostAddresses(service.Address).FirstOrDefault()}");
        //    _log($"\t\tProtocol:{service.Protocol}");
        //    _log($"\t\tIndex:{state.UsedSelenoidService}");
        //}

        public KomoRepository GetRepository()
        {
            var result = new KomoRepository();

            return result;
        }
    }
}