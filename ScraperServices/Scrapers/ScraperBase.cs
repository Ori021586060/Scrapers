using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using ScraperCore;
using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScraperServices.Scrapers
{
    public abstract class ScraperBase
    {
        protected IState _state { get; set; }
        public bool Scrape()
        {
            var scrape = ScrapeAsync();
            var result = scrape.Result;

            return result;
        }

        public async Task<bool> ScrapeAsync()
        {
            var result = await ScrapeInnerAsync();

            return result;
        }

        protected abstract Task<bool> ScrapeInnerAsync();

        protected void _prepareToWorkBase(IState state)
        {
            _setWorkPhaseBase("prepare", state);

            _logBase($"Start scraper win-win (isNew:{state.IsNew})", state);

            if (state.IsNew) _clearWorkspace(state);

            _checkWorkspace(state);
        }

        protected void _setWorkPhaseBase(string workPhase, IState state)
        {
            state.WorkPhase = workPhase;
        }

        protected void _checkWorkspace(IState state)
        {
            SetWorkPhaseBase("CheckWorkspace", state);

            _checkWorkspaceInner(state);

            _logBase($"Done", state);
        }

        protected abstract void _checkWorkspaceInner(IState state);

        protected void _clearWorkspace(IState state)
        {
            SetWorkPhaseBase("ClearWorkspace", state);

            _clearWorkspaceInner(state);

            _logBase($"Done", state);
        }

        protected abstract void _clearWorkspaceInner(IState state);

        protected void _checkDirectory(string directory)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }

        protected void _checkDirectory(string directory, IState state)
        {
            _checkDirectory(directory);
        }

        protected void _cleanFile(string filename, IState state)
        {
            _logBase($"Clean/delete file {filename}", state);

            if (File.Exists(filename)) File.Delete(filename);
        }

        protected void _cleanDirectory(string path, IState state)
        {
            if (Directory.Exists(path))
            {
                var dirInfo = new DirectoryInfo(path);

                _logBase($"Cleaning directory: {dirInfo.FullName}", state);

                foreach (var dir in dirInfo.GetDirectories()) dir.Delete(recursive: true);

                foreach (var file in dirInfo.GetFiles()) file.Delete();

                _logBase("Cleaning directory done", state);
            }
            else
            {
                Directory.CreateDirectory(path);
                _logBase($"Created directory {path}", state);
            }
        }

        protected void _logBase(string message, IState state)
        {
            var logMessage = Formats.LogRow(DateTime.UtcNow, state.SpentTime, state.TypeScraper.ToString(), state.WorkPhase, message);

            Console.WriteLine(logMessage);
        }

        protected void _initSelenoidBase(SelenoidStateModel selenoidState, IState state)
        {
            if (selenoidState is null) selenoidState = new SelenoidStateModel();

            var options = new FirefoxOptions();

            if (!selenoidState.ShowPictures)
                options.SetPreference("permissions.default.image", 2);

            options.SetPreference("javascript.enabled", selenoidState.JavaScriptEnable);

            bool doNeedRepeatRequest = false;
            int countRepeat = 0;
            int countRepeatMax = 10;

            do
            {
                var selenoidService = _selectSelenoidServiceBase(state);
                var selenoidUrl = $"{selenoidService.Protocol}://{selenoidService.Address}:4444/wd/hub";
                _logBase($"\tInit Selenoid Service-{state.UsedSelenoidService}:{selenoidUrl}", state);

                try
                {
                    if (selenoidState.WindowMain != null)
                    {
                        _logBase($"SelenoidService isnt null", state);
                        selenoidState.WindowMain.Quit();
                        _logBase($"Quit SelenoidService done", state);
                    }

                    _printSelenoidServiceParams(selenoidService, state);

                    doNeedRepeatRequest = false;

                    selenoidState.WindowMain = new RemoteWebDriver(new Uri(selenoidUrl), options.ToCapabilities(), TimeSpan.FromMinutes(5));

                    selenoidState.WaitMain = new WebDriverWait(selenoidState.WindowMain, TimeSpan.FromSeconds(120));
                }
                catch
                {
                    countRepeat++;
                    if (countRepeat < countRepeatMax)
                    {
                        doNeedRepeatRequest = true;
                        _logBase($"\t!!! Need change Selenoid Service !!!", state);
                        _logBase($"\tBad config Selenoid Service-{state.UsedSelenoidService}:", state);
                        _printSelenoidServiceParams(selenoidService, state);
                    }
                    else
                        _logBase($"Try is out. Error", state);
                }

                if (doNeedRepeatRequest)
                {
                    _logBase($"\tcountRepeat:{countRepeat}", state);
                    state.UsedSelenoidService++;
                    Thread.Sleep(TimeSpan.FromMinutes(10));
                }

            } while (doNeedRepeatRequest);
        }

        private SelenoidConfigModel _selectSelenoidServiceBase(IState state)
        {
            var config = _loadConfigSelenoidSectionBase(state);

            Func<SelenoidConfigModel> GetSelenoidService = () => config.Selenoid
                        .Where(x => x.Enabled)
                        .Skip(state.UsedSelenoidService)
                        .FirstOrDefault();

            var selenoidService = GetSelenoidService();

            if (selenoidService is null)
            {
                state.UsedSelenoidService = 0;
                selenoidService = GetSelenoidService();
            }

            return selenoidService;
        }

        private ParamsSelenoidConfigModel _loadConfigSelenoidSectionBase(IState state)
        {
            ParamsSelenoidConfigModel result = null;
            var filename = state.ConfigFilename;
            _logBase($"Load selenoid config: {filename}", state);

            if (File.Exists(filename))
            {
                result = JsonConvert.DeserializeObject<ParamsSelenoidConfigModel>(File.ReadAllText(filename));
                _logBase($"file exists", state);
            }
            else
                _logBase($"no config file", state);

            return result;
        }

        private void _printSelenoidServiceParams(SelenoidConfigModel service, IState state)
        {
            _logBase($"\tParams Selenoid:", state);
            _logBase($"\t\tDns:{service.Address}", state);
            _logBase($"\t\tIp:{Dns.GetHostAddresses(service.Address).FirstOrDefault()}", state);
            _logBase($"\t\tProtocol:{service.Protocol}", state);
            _logBase($"\t\tIndex:{state.UsedSelenoidService}", state);
        }

        protected bool SaveStatusBase(object status, IState state)
        {
            var result = false;

            try
            {
                var filename = $"{state.StatusFilename}";

                File.WriteAllText(filename, JsonConvert.SerializeObject(status, Newtonsoft.Json.Formatting.Indented));

                result = true;
            }
            catch { }

            return result;
        }

        protected DateTime _statusWorkspace_ScrapeDateBase(IState state)
        {
            var path = state.ItemsPath;
            DateTime result = default;

            try
            {
                var listItems = new DirectoryInfo($"{path}").GetFiles();
                result = listItems[listItems.Length - 1].LastWriteTime;
            }catch(Exception exception)
            {
                _logBase($"No data. {exception.Message}", state);
            }

            return result;
        }

        protected int _statusWorkspace_AmountItemsFromPathBase(IState state)
        {
            var listItems = _statusWorkspace_AmountItemsFromPath_GetFilesBase(state);
            var result = listItems.Length;

            return result;
        }

        protected FileInfo[] _statusWorkspace_AmountItemsFromPath_GetFilesBase(IState state)
        {
            var path = state.ItemsPath;
            var listItems = new DirectoryInfo($"{path}").GetFiles();

            return listItems;
        }

        protected int _statusWorkspace_AmountPagesBase(IState state)
        {
            var listPages = _statusWorkspace_AmountPages_GetFilesBase(state);

            return listPages.Length;
        }

        protected FileInfo[] _statusWorkspace_AmountPages_GetFilesBase(IState state)
        {
            var path = state.PagesPath;
            var listPages = new DirectoryInfo($"{path}").GetFiles();

            return listPages;
        }

        protected void SetWorkPhaseBase(string phaseText, IState state, string text = "")
        {
            state.WorkPhase = $"{phaseText}";
            if (string.IsNullOrEmpty(text)) text = "Start";
            _logBase(text, state);

            PrintShortState(state);

            UpdateStateFile($"{phaseText}", state);
        }

        private void PrintShortState(IState state)
        {
            _logBase($"Is new: {state.IsNew}", state);
        }

        protected void UpdateStateFile(string text, IState state)
        {
            //var filename = state.LogStatFilename;

            //File.WriteAllText(filename, text);
        }

        protected async Task<T> LoadItemDtoFromStoreAsync<T>(FileInfo file, IState state)
        {
            var result = DeserializeFileAsync<T>(file);
            _logBase($"Load ItemDto: {file.Name}", state);

            return await result;
        }

        protected async Task<T> DeserializeFileAsync<T>(FileInfo file)
        {
            T result = default;

            if (File.Exists(file.FullName))
                result = JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(file.FullName));

            return result;
        }

        protected async Task<HtmlDocument> GetPageData_WebClient(string url, IState state)
        {
            var webGet = new HtmlWeb();
            HtmlDocument page = null;

            try
            {
                page = await webGet.LoadFromWebAsync(url);
            }
            catch (Exception exception)
            {
                _logBase($"Error-t1. {exception.Message}", state);
            }

            return page;
        }

        protected void LogDone(IState state)
        {
            _logBase($"Done", state);
        }

        protected FileInfo[] GetListItemFiles(IState state)
        {
            var path = $"{state.ItemsPath}";
            var itemFiles = new DirectoryInfo(path).GetFiles();

            return itemFiles;
        }

        public DataScrapeModel GetDomainModel()
        {
            var model = GetDomainModelAsync();
            var result = model.Result;

            return result;
        }

        public abstract Task<DataScrapeModel> GetDomainModelAsync();
    }
}
