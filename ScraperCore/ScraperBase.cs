using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ScraperCore
{
    public abstract class ScraperBase
    {
        public virtual DataScrapeModel Scrape()
        {
            return ScrapeInner();
        }

        protected abstract DataScrapeModel ScrapeInner();

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

        protected void _endToWorkBase(IState state)
        {
            _setWorkPhaseBase("finish", state);

            _logBase($"Scraper win-win done (isNew:{state.IsNew})", state);
        }

        protected void _checkWorkspace(IState state)
        {
            _logBase($"Start check workspace", state);

            _checkWorkspaceInner(state);

            _logBase($"Check workspace done", state);
        }

        protected abstract void _checkWorkspaceInner(IState state);

        protected void _clearWorkspace(IState state)
        {
            _logBase($"Cleaning workspace", state);

            _clearWorkspaceInner(state);

            _logBase($"Clear workspace done", state);
        }

        protected abstract void _clearWorkspaceInner(IState state);

        protected void _checkDirectory(string directory, IState state)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
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
        }

        protected void _logBase(string message, IState state)
        {
            var logMessage = $"{DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss")} | {state.TypeScraper.ToString(),-10} | {state.WorkPhase,-10} | {message}";

            Console.WriteLine(logMessage);

            //File.AppendAllText(_logFile, $"{logMessage}\r\n");
        }

        protected void _initSelenoidBase(SelenoidStateModel selenoidState, IState state)
        {
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
                    //Thread.Sleep(TimeSpan.FromMinutes(10));
                }

            } while (doNeedRepeatRequest);
        }

        private SelenoidConfigModel _selectSelenoidServiceBase(IState state)
        {
            var config = _loadConfigBaseSelenoidSection(state);

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

        private ParamsSelenoidConfigModel _loadConfigBaseSelenoidSection(IState state)
        {
            ParamsSelenoidConfigModel result = null;
            var filename = state.ConfigFilename;

            if (File.Exists(filename))
                result = JsonConvert.DeserializeObject<ParamsSelenoidConfigModel>(File.ReadAllText(filename));
            //else
            //{
            //    result = new ParamsSelenoidConfigModel();
            //    _saveConfig(result, state);
            //}

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
    }
}
