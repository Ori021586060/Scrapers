﻿using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using Flurl.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using System.Net;
using System.IO.Compression;
using System.Net.Http;
using ScraperModels.Models;
using ScraperCore;
using ScraperServices.Models.Yad2;
using ScraperModels.Models.Excel;
using ScraperModels.Models.Yad2Dto;
using ScraperServices.Services;
using ScraperModels;
using ScraperModels.Models.Domain;
using ScraperRepositories.Repositories;

namespace ScraperServices.Scrapers
{
    public class ScraperYad2: ScraperBase
    {
        private ScraperYad2ConfigModel _config { get; set; }

        public ScraperYad2(ScraperYad2StateModel state = null)
        {
            if (state is null) state = new ScraperYad2StateModel();

            _state = state;

            _config = _loadConfig((ScraperYad2StateModel)_state);
        }

        public ExcelYad2Service GetExcelService()
        {
            return new ExcelYad2Service((ScraperYad2StateModel)_state);
        }

        public DataDomainModel<List<AdItemYad2DomainModel>> GetDomainModelFromFile()
        {
            var model = GetDomainModelFromFileAsync();
            var result = model.Result;

            return result;
        }

        protected async Task<DataDomainModel<List<AdItemYad2DomainModel>>> GetDomainModelFromFileAsync()
        {
            var state = _state;
            var filename = $"{state.RootPath}/domain-model.json";

            var json = await File.ReadAllTextAsync(filename);
            var model = JsonConvert.DeserializeObject<DataDomainModel<List<AdItemYad2DomainModel>>>(json);

            return model;
        }

    private ScraperYad2ConfigModel _loadConfig(ScraperYad2StateModel state)
        {
            ScraperYad2ConfigModel result = null;
            var filename = state.ConfigFilename;

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<ScraperYad2ConfigModel>(File.ReadAllText(filename));
            else
            {
                result = new ScraperYad2ConfigModel();
                _saveConfig(result, state);
            }

            return result;
        }

        private void _saveConfig(ScraperYad2ConfigModel config, ScraperYad2StateModel state)
        {
            var configFilename = state.ConfigFilename;
            _log($"Start saving config:{configFilename}");

            File.WriteAllText(configFilename, JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));

            _log($"Save config:{configFilename} is done");
        }

        protected override async Task<bool> ScrapeInnerAsync()
        {
            var result = false;
            var state = (ScraperYad2StateModel)_state;

            _log($"Start scraper Yad2 (isNew:{state.IsNew})");
            try
            {
                if (state.IsNew)
                {
                    _clearWorkspace(state);
                }
                else
                    _log($"Clean workspace missing");

                _checkWorkspace(state);

                await _scrapingAsync(state);
                result = true;
            }
            catch (Exception exception){
                _log($"Error s1. {exception.Message}");
            }

            return result;
        }

        public override async Task<DataDomainModel> GetDomainModelAsync()
        {
            var state = (ScraperYad2StateModel)_state;
            SetWorkPhaseBase($"DomainModel", state);
            var domainModel = _scrapePhase7_GenerateDomainModelAsync(state);

            DataDomainModel result = new DataDomainModel()
            {
                Scraper = EnumScrapers.Yad2,
                Data = await domainModel,
            };

            LogDone(state);

            return result;
        }

        private void _log(string message, IState state = null)
        {
            _logBase(message, state is null ? _state : state);
        }

        private async Task<bool> _scrapingAsync(ScraperYad2StateModel state)
        {
            var result = false;

            try
            {
                await _scrapePhase1_GenerateListPreLoadsAsync(state);
                await _scrapePhase2_GetPreLoads(state);
                await _scrapePhase3_GenerateListItems(state);
                await _scrapePhase4_GetItems(state);
                await _scrapePhase5_GenerateListItemsContactsAsync(state);
                await _scrapePhase6_GetItemsContacts(state);

                result = true;
            }
            catch(Exception exception) {
                _log($"Erro-x1. {exception.Message} / {exception.StackTrace}");
            }

            return result;
        }

        private async Task<List<AdItemYad2DomainModel>> _scrapePhase7_GenerateDomainModelAsync(ScraperYad2StateModel state)
        {
            List<AdItemYad2DomainModel> result = null;

            var listItems = await _loadListItemsAsync(state)?? new Dictionary<string, bool>();

            if (listItems.Count>0) result = new List<AdItemYad2DomainModel>();
            var sb = new StringBuilder(1000);
            var indexParseItems = 0;

            foreach (var itemId in listItems)
            {
                try
                {
                    var key = itemId.Key;
                    var itemDto = JsonConvert.DeserializeObject<Phase3ObjectDto>(await File.ReadAllTextAsync($"{state.ItemsPath}/{key}.json"));
                    var itemContactsDto = JsonConvert.DeserializeObject<Phase3ObjectContactsDto>(await File.ReadAllTextAsync($"{state.PathItemsContacts}/item-contacts-{key}.json"));

                    var itemDomain = new AdItemYad2DomainModel().FromDto(itemDto, itemContactsDto);

                    result.Add(itemDomain);

                    sb.Append($",{itemId.Key}=ok");
                }
                catch (Exception exception)
                {
                    sb.Append($",{itemId.Key}=fail, {exception.Message}");
                }

                indexParseItems++;
                if (indexParseItems % 3000 == 0)
                    _log($"Parsed {indexParseItems} items");
            }

            _log($"Parsed items are {sb.ToString()}");

            return result;
        }

        private async Task<bool> _scrapePhase6_GetItemsContacts(ScraperYad2StateModel state)
        {
            var result = true;

            SetWorkPhaseBase($"GetItemsContacts", state);

            var listItemsContacts = await _loadListItemsContactsAsync(state);

            if (listItemsContacts != null)
            {
                await _scrapePhase6_DownloadsItemsContactsAsync(listItemsContacts, state);
            }
            else
            {
                Console.WriteLine($"Fail load list-items");
                result = false;
            }

            return result;
        }

        private async Task _scrapePhase6_DownloadsItemsContactsAsync(Dictionary<string, bool> list, ScraperYad2StateModel state)
        {
            var hasError = false;
            var amount = 0;

            Func<IEnumerable<string>> toDoAll = () => list.Where(x => x.Value == false).Select(x => x.Key);
            Func<IEnumerable<string>> toDone = () => list.Where(x => x.Value == true).Select(x => x.Key);
            Action showStat = () => _log($"Downloads: done: {toDone().Count()}, balance: {toDoAll().Count()}, total: {list.Count}");

            showStat();

            var amountGetters = state.CountScrapers;

            if (toDoAll().Count() > 0) _log($"Use {amountGetters} threads");

            while (toDoAll().Count() > 0)
            {
                List<Task<bool>> arrayTask = new List<Task<bool>>();
                var listToDo = toDoAll().Take(amountGetters).ToList();

                foreach (var toDo in listToDo)
                    arrayTask.Add(Task.Run(async() => await _downloadItemContactAsync(toDo, state)));

                await _saveListItemsContactsAsync(list, state);

                hasError = false;
                try
                {
                    Task.WaitAll(arrayTask.ToArray(), TimeSpan.FromMinutes(15));
                }
                catch (Exception exception)
                {
                    hasError = true;
                    _log($"Error s5. {exception.Message}");
                }

                var i = 0;
                if (!hasError)
                {
                    foreach (var task in arrayTask)
                    {
                        bool itemData = task.Result;
                        if (itemData)
                        {
                            list[listToDo.Skip(i).Take(1).FirstOrDefault()] = true;
                            amount++;
                        }
                        else
                            hasError = true;

                        i++;
                    }
                }

                if (hasError)
                {
                    _log($"Error pass item-contacts {listToDo.Skip(i).Take(1).FirstOrDefault()}");
                    _log($"Pause {state.CountWaitSecondForFailRequest} sec");
                    Thread.Sleep(TimeSpan.FromSeconds(state.CountWaitSecondForFailRequest));
                }

                showStat();
            }

            _log($"List items-contacts completed");
            await _saveListItemsContactsAsync(list, state);
        }

        private async Task<bool> _downloadItemContactAsync(string item, ScraperYad2StateModel state)
        {
            var url = $"https://www.yad2.co.il/api/item/{item}/contactinfo";
            var filename = $"{state.PathItemsContacts}/item-contacts-{item}.json";

            var isDoneGetObject = await _downloadFilenameAsync(url, filename);

            return isDoneGetObject;
        }

        private async Task _scrapePhase5_GenerateListItemsContactsAsync(ScraperYad2StateModel state)
        {
            SetWorkPhaseBase($"GenerateListItemsContacts", state);
            var listItemsContacts = await _loadListItemsContactsAsync(state);

            if (listItemsContacts == null)
            {
                _log($"Generate new list items-contacts");

                listItemsContacts = new Dictionary<string, bool>();

                var files = Directory.GetFiles(state.PathPreLoads);
                var dublicates = new List<string>();

                foreach (var file in files)
                {
                    var fileData = File.ReadAllText(file);

                    var data = JsonConvert.DeserializeObject<PreloadDtoModel>(fileData);

                    var items = data.Feed.feed_items.Where(x => !string.IsNullOrEmpty(x.id)).Select(x => x.id).ToList();

                    foreach (var item in items)
                        if (listItemsContacts.ContainsKey(item))
                            dublicates.Add(item);
                        else
                            listItemsContacts.Add(item, false);
                }

                _log($"Generate new list items-contacts is completed");

                _log($"Total uniq:{listItemsContacts.Count}, Dublicate:{dublicates.Count}, Total:{listItemsContacts.Count + dublicates.Count}");

                File.WriteAllText($"{state.ListItemsContactsDublicatesFilename}", JsonConvert.SerializeObject(dublicates, Formatting.Indented));

                await _saveListItemsContactsAsync(listItemsContacts, state);
            }
            else
                _log($"Generate list items-contacts is missing");
        }

        private async Task _saveListItemsContactsAsync(Dictionary<string, bool> list, ScraperYad2StateModel state)
        {
            _log($"Save items-contacts {state.PathListItemsContacts}");

            await File.WriteAllTextAsync($"{state.PathListItemsContacts}", JsonConvert.SerializeObject(list, Formatting.Indented));
        }

        private async Task<Dictionary<string, bool>> _loadListItemsContactsAsync(ScraperYad2StateModel state)
        {
            Dictionary<string, bool> result = null;

            _log($"Load list-items-contacts ");

            try
            {
                var filename = state.PathListItemsContacts;
                if (File.Exists(filename))
                {
                    result = JsonConvert.DeserializeObject<Dictionary<string, bool>>(await File.ReadAllTextAsync(filename));
                    _log($"Load list-items-contacts done");
                }
                else
                    throw new Exception();
            }
            catch(Exception exception)
            {
                _log($"Load list-items-contacts fail");
                _log($"Error s2. {exception.Message}");
            }

            return result;
        }

        private async Task<bool> _scrapePhase4_GetItems(ScraperYad2StateModel state)
        {
            var result = true;

            SetWorkPhaseBase($"GetItems", state);

            var listItems = await _loadListItemsAsync(state);

            if (listItems != null)
            {
                await _scrapePhase4_DownloadsItemsAsync(listItems, state);
            }
            else
            {
                _log($"Fail load list-items");
                result = false;
            }

            return result;
        }

        private async Task _scrapePhase4_DownloadsItemsAsync(Dictionary<string, bool> list, ScraperYad2StateModel state)
        {
            var hasError = false;
            var amount = 0;

            Func<IEnumerable<string>> toDoAll = () => list.Where(x => x.Value == false).Select(x => x.Key);
            Func<IEnumerable<string>> toDone = () => list.Where(x => x.Value == true).Select(x => x.Key);
            Action showStat = () => _log($"Downloads: done: {toDone().Count()}, balance: {toDoAll().Count()}, total: {list.Count}");

            showStat();

            var amountGetters = state.CountScrapers;

            if (toDoAll().Count() > 0) _log($"Use {amountGetters} threads");

            while (toDoAll().Count() > 0)
            {
                List<Task<bool>> arrayTask = new List<Task<bool>>();
                var listToDo = toDoAll().Take(amountGetters).ToList();

                foreach (var toDo in listToDo)
                    arrayTask.Add(Task.Run(async() => await _downloadItemAsync(toDo, state)));

                await _saveListItemsAsync(list, state);

                hasError = false;
                try
                {
                    Task.WaitAll(arrayTask.ToArray(), TimeSpan.FromMinutes(15));
                }
                catch (Exception exception)
                {
                    hasError = true;
                    _log($"Error d1. {exception.Message}");
                }

                var i = 0;
                if (!hasError)
                {
                    foreach (var task in arrayTask)
                    {
                        bool itemData = task.Result;
                        if (itemData)
                        {
                            list[listToDo.Skip(i).Take(1).FirstOrDefault()] = true;
                            amount++;
                        }
                        else
                            hasError = true;

                        i++;
                    }
                }

                if (hasError)
                {
                    _log($"Error pass item {listToDo.Skip(i).Take(1).FirstOrDefault()}");
                    _log($"Pause {state.CountWaitSecondForFailRequest} sec");
                    Thread.Sleep(TimeSpan.FromSeconds(state.CountWaitSecondForFailRequest));
                }

                showStat();
            }

            _log($"Download items completed");
            await _saveListItemsAsync(list, state);
        }

        private async Task<bool> _downloadItemAsync(string item, ScraperYad2StateModel state)
        {
            var url = $"https://www.yad2.co.il/api/item/{item}";
            var filename = $"{state.ItemsPath}/{item}.json";

            var isDoneGetObject = await _downloadFilenameAsync(url, filename);

            return isDoneGetObject;
        }

        private async Task _scrapePhase3_GenerateListItems(ScraperYad2StateModel state)
        {
            SetWorkPhaseBase($"GenerateListItems", state);
            var listItems = await _loadListItemsAsync(state);

            if (listItems == null)
            {
                Console.Write($"Generate new list items ...");

                listItems = new Dictionary<string, bool>();

                var files = Directory.GetFiles(state.PathPreLoads);
                var dublicates = new List<string>();

                foreach (var file in files)
                {
                    var fileData = await File.ReadAllTextAsync(file);

                    var data = JsonConvert.DeserializeObject<PreloadDtoModel>(fileData);

                    var items = data.Feed.feed_items.Where(x => !string.IsNullOrEmpty(x.id)).Select(x => x.id).ToList();

                    foreach (var item in items)
                        if (listItems.ContainsKey(item))
                            dublicates.Add(item);
                        else
                            listItems.Add(item, false);
                }

                _log("Done");

                Console.WriteLine($"Total uniq:{listItems.Count}, Dublicate:{dublicates.Count}, Total:{listItems.Count + dublicates.Count}");

                await File.WriteAllTextAsync($"{state.ListItemsDublicatesFilename}", JsonConvert.SerializeObject(dublicates, Formatting.Indented));

                await _saveListItemsAsync(listItems, state);
            }
        }

        private async Task _saveListItemsAsync(Dictionary<string, bool> list, ScraperYad2StateModel state)
        {
            _log($"Save {state.PathListItems}");
            await File.WriteAllTextAsync($"{state.PathListItems}", JsonConvert.SerializeObject(list, Formatting.Indented));
        }

        private async Task<Dictionary<string, bool>> _loadListItemsAsync(ScraperYad2StateModel state)
        {
            Dictionary<string, bool> result = null;

            _log($"Load list-items");

            try
            {
                var filename = state.PathListItems;
                if (File.Exists(filename))
                {
                    result = JsonConvert.DeserializeObject<Dictionary<string, bool>>(await File.ReadAllTextAsync(filename));
                    _log($"Load list-items is completed");
                }
                else
                    throw new Exception();
            }
            catch (Exception exception)
            {
                _log($"Load list-items is fail");
                _log($"Error L1. {exception.Message}");
            }

            return result;
        }

        private async Task _scrapePhase1_GenerateListPreLoadsAsync(ScraperYad2StateModel state)
        {
            SetWorkPhaseBase($"GenerateListPreLoads", state);
            var listPreLoads = await _loadListPreLoadsAsync(state);

            if (listPreLoads == null)
            {
                _log($"Generate new list-pre-loads");
                var tryCount = 1;
                var tryCountMax = 10;
                var doNeedTry = false;

                do
                {
                    doNeedTry = false;
                    var isDoneGetPage1 = await _downloadPreLoadAsync(page: 1, state: state);

                    if (isDoneGetPage1)
                    {
                        var page1Dto = _getPreloadFromFilestore(page: 1, state: state);
                        var lastPage = int.Parse(page1Dto.Pagination.LastPage);

                        _log($"Detect {lastPage} pages");

                        var list = new Dictionary<int, bool>();

                        foreach (var i in Enumerable.Range(1, lastPage)) list.Add(i, false);
                        list[1] = true;

                        await _saveListPreLoadsAsync(list, state);
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
                            _log($"Tred {tryCount} download PreLoad Page1. Stop scrap process.");
                    }
                } while (doNeedTry);
            }
            else
                _log($"Generate list-pre-loads is missing");
        }

        private async Task _saveListPreLoadsAsync(Dictionary<int, bool> list, ScraperYad2StateModel state)
        {
            _log($"Save list-pre-loads: {state.PathListPreLoads}");

            await File.WriteAllTextAsync($"{state.PathListPreLoads}", JsonConvert.SerializeObject(list, Formatting.Indented));
        }

        private async Task<Dictionary<int,bool>> _loadListPreLoadsAsync(ScraperYad2StateModel state)
        {
            Dictionary<int, bool> result = null;

            _log($"Load list-pre-loads starting");
            var filename = $"{state.PathListPreLoads}";

            if (File.Exists(filename))
            {
                try
                {
                    result = JsonConvert.DeserializeObject<Dictionary<int, bool>>(await File.ReadAllTextAsync(filename));
                    _log($"Load list-pre-loads is complete");
                }
                catch (Exception exception)
                {
                    _log($"Load list-pre-loads is fail");
                    _log($"Error L2. {exception.Message}");
                }
            }
            else _log($"no file {filename}");

            return result;
        }

        private PreloadDtoModel _getPreloadFromFilestore(int page, ScraperYad2StateModel state)
        {
            var result = JsonConvert.DeserializeObject<PreloadDtoModel>(File.ReadAllText($"{state.PathPreLoads}/page-{page}.json"));

            return result;
        }

        private async Task<bool> _downloadPreLoadAsync(int page, ScraperYad2StateModel state)
        {
            var url = $"https://www.yad2.co.il/api/pre-load/getFeedIndex/realestate/rent?page={page}&compact-req=1";
            var filename = $"{state.PathPreLoads}/page-{page}.json";

            var isDoneGetPage = await _downloadFilenameAsync(url, filename);

            return isDoneGetPage;
        }
        private async Task<bool> _downloadFilenameAsync(string url, string filename)
        {
            bool result = true;
            var guid = Guid.NewGuid();

            _log($"Download start guild:{guid}, url:{url}, filename:{filename}");

            var client = new WebClient();

            ICredentials credentials = new NetworkCredential(_config.Proxy.Username, _config.Proxy.Password);
            client.Proxy = new WebProxy(new Uri(_config.Proxy.Server), true, null, credentials);
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");

            var response = "";
            try
            {
                response = await client.DownloadStringTaskAsync(new Uri(url));
                var json = JsonConvert.DeserializeObject<object>(response);
                await File.WriteAllTextAsync(filename, response);
                _log($"Download {guid} is complete");
            }
            catch (Exception exception)
            {
                client.CancelAsync();
                client.Dispose();
                result = false;
                _log($"Download {guid} is fail");
                _log($"Error D1. {exception.Message}");
            }

            return result;
        }
        private async Task<bool> _scrapePhase2_GetPreLoads(ScraperYad2StateModel state)
        {
            var result = true;

            SetWorkPhaseBase($"GetPreLoads", state);

            var listPreLoads = await _loadListPreLoadsAsync(state);

            if (listPreLoads != null)
            {
                await _scrapePhase2_DownloadsPreLoadsAsync(listPreLoads, state);
            }
            else
            {
                _log($"Fail load list-pre-loads");
                result = false;
            }

            return result;
        }

        private async Task _scrapePhase2_DownloadsPreLoadsAsync(Dictionary<int, bool> list, ScraperYad2StateModel state)
        {
            var hasError = false;
            var amount = 0;

            Func<IEnumerable<int>> toDoAll = () => list.Where(x => x.Value == false).Select(x => x.Key);
            Func<IEnumerable<int>> toDone = () => list.Where(x => x.Value == true).Select(x => x.Key);
            Action showStat = () => _log($"Downloads: done: {toDone().Count()}, balance: {toDoAll().Count()}, total: {list.Count}");

            showStat();

            var amountGetters = state.CountScrapers;

            if (toDoAll().Count() > 0) _log($"Use {amountGetters} threads");

            while (toDoAll().Count() > 0)
            {
                List<Task<bool>> arrayTask = new List<Task<bool>>();
                var listToDo = toDoAll().Take(amountGetters).ToList();

                foreach (var toDo in listToDo)
                    arrayTask.Add(Task.Run(async() => await _downloadPreLoadAsync(toDo, state)));

                await _saveListPreLoadsAsync(list, state);

                hasError = false;
                try
                {
                    Task.WaitAll(arrayTask.ToArray(), TimeSpan.FromMinutes(5));
                }
                catch (Exception exception)
                {
                    hasError = true;
                    _log($"Error D2. {exception.Message}");
                }

                var i = 0;
                if (!hasError)
                {
                    foreach (var task in arrayTask)
                    {
                        bool itemData = task.Result;
                        if (itemData)
                        {
                            list[listToDo.Skip(i).Take(1).FirstOrDefault()] = true;
                            amount++;
                        }
                        else
                            hasError = true;

                        i++;
                    }
                }

                if (hasError)
                {
                    _log($"Error pass page {listToDo.Skip(i).Take(1).FirstOrDefault()}");
                    _log($"Pause {state.CountWaitSecondForFailRequest} sec");
                    Thread.Sleep(TimeSpan.FromSeconds(state.CountWaitSecondForFailRequest));
                }

                showStat();
            }

            _log($"List pre-loads completed");
            await _saveListPreLoadsAsync(list, state);
        }

        protected override void _clearWorkspaceInner(IState state)
        {
            _cleanDirectory($"{state.RootPath}");

            _cleanFile($"{state.LogFilename}");

            _log($"Clean workspace done");
        }

        private void _cleanFile(string filename)
        {
            _log($"Clean/delete file {filename}");

            if (File.Exists(filename)) File.Delete(filename);
        }

        private void _cleanDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    var dirInfo = new DirectoryInfo(path);
                    dirInfo.Delete(recursive: true);
                }
            }catch(Exception exception)
            {
                _log($"Error-e1. {exception.Message}");
            }

            //if (Directory.Exists(dirInfo.FullName))
            //{
            //    _log($"Cleaning path: {dirInfo.FullName}");

            //    foreach (var dir in dirInfo.GetDirectories()) dir.Delete(recursive: true);

            //    foreach (var file in dirInfo.GetFiles()) file.Delete();

            //    _log("Cleaning path done");
            //}
        }

        protected override void _checkWorkspaceInner(IState state)
        {
            CheckDirectory(state.RootPath);

            CheckDirectory($"{(state as ScraperYad2StateModel).PathPreLoads}");

            CheckDirectory($"{(state as ScraperYad2StateModel).ItemsPath}");

            CheckDirectory($"{(state as ScraperYad2StateModel).PathItemsContacts}");
        }
        private void CheckDirectory(string path)
        {
            _log($"Checking directory {path}");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                _log($"Directory has created");
            }
            else
                _log($"Directory is exists");
        }

        public ScraperYad2StatusModel StatusWorkspace()
        {
            var state = (ScraperYad2StateModel)_state;

            state.WorkPhase = "StatusWorkspace";

            _log("Start");

            var scrapeDate = _statusWorkspace_ScrapeDateBase(state);
            var amountItemsFromPath = _statusWorkspace_AmountItemsFromPathBase(state);
            var amountItemsWithWrongDataFromPath = _statusWorkspace_AmountItemsWithWrongDataFromPath(state);
            var amountPages = _statusWorkspace_AmountPagesBase(state);
            var amountItemsFromPages = _statusWorkspace_AmountItemsFromPages(state);
            var amountItemUniquesFromPages = _statusWorkspace_AmountItemUniquesFromPages(state);

            var status = new ScraperYad2StatusModel()
            {
                ScrapeDate = scrapeDate,
                AmountItemsFromPath = amountItemsFromPath,
                AmountItemsWithWrongDataFromPath = amountItemsWithWrongDataFromPath,
                AmountPages = amountPages,
                AmountItemsFromPages = amountItemsFromPages,
                AmountItemUniquesFromPages = amountItemUniquesFromPages,
            };

            _log("End");

            return status;
        }

        private int _statusWorkspace_AmountItemsWithWrongDataFromPath(ScraperYad2StateModel state)
        {
            var listItems = _statusWorkspace_AmountItemsFromPath_GetFilesBase(state);
            var amountItemsWithWrongData = 0;
            
            foreach(var item in listItems)
            {
                try
                {
                    var itemObject = JsonConvert.DeserializeObject<Phase3ObjectDto>(File.ReadAllText(item.FullName));
                }
                catch (Exception exception)
                {
                    amountItemsWithWrongData++;
                    _log($"Error w1. {exception.Message}");
                }
            }

            return amountItemsWithWrongData;
        }

        private int _statusWorkspace_AmountItemUniquesFromPages(ScraperYad2StateModel state)
        {
            var list = _statusWorkspace_AmountItemsFromPages_GetItems(state);

            var dups = list.GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.First()?.Id ?? "NULL", x => x.First().Done);

            var result = dups.Count() - dups.Where(x=>x.Key == "NULL").Count();

            return result;
        }

        private int _statusWorkspace_AmountItemsFromPages(ScraperYad2StateModel state)
        {
            var list = _statusWorkspace_AmountItemsFromPages_GetItems(state);

            return list.Count();
        }

        private List<ItemTest> _statusWorkspace_AmountItemsFromPages_GetItems(ScraperYad2StateModel state)
        {
            var listPages = _statusWorkspace_AmountPages_GetFilesBase(state);

            var totalItems = 0;
            var list = new List<ItemTest>();

            foreach (var page in listPages)
            {
                var filename = page.FullName;
                var pageData = JsonConvert.DeserializeObject<PreloadDtoModel>(File.ReadAllText(filename));
                var listItems = pageData.Feed.feed_items.Select(x => new ItemTest() { Id = x.id, Done = false }).ToList();
                list.AddRange(listItems);
                totalItems += listItems.Count;
            }

            return list;
        }

        public void PrintStatus(ScraperYad2StatusModel status)
        {
            _log($"ScrapeDate: {status.ScrapeDate}");
            _log($"AmountItemsFromPath: {status.AmountItemsFromPath}");
            _log($"AmountItemsWithWrongDataFromPath: {status.AmountItemsWithWrongDataFromPath}");
            _log($"AmountPages: {status.AmountPages}");
            _log($"AmountItemsFromPages: {status.AmountItemsFromPages}");
            _log($"AmountItemDuplicatesFromPages: {status.AmountItemDuplicatesFromPages}");
            _log($"AmountItemUniquesFromPages: {status.AmountItemUniquesFromPages}");
        }

        public bool SaveStatus(object status)
        {
            var result = SaveStatusBase(status, _state);

            return result;
        }

        public Yad2Repository GetRepository()
        {
            var result = new Yad2Repository();

            return result;
        }
    }
}
