using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScraperServices.Services
{
    public class SelenoidService
    {
        private RemoteWebDriver _windowMain { get; set; }
        private WebDriverWait _waitMain { get; set; }
        private string _selenoidProtocol { get; set; } = "http";
        private string _selenoidAddress { get; set; } = "selenoid.localnet";
        private Logger _logger { get; set; }
        public SelenoidService()
        {
            _logger = LogManager.GetLogger("selenoid");

            _logger.Debug("Create SelenoidService");
        }
        public ResponseParseUrlOnAirdna Airdna_ParseUrlForGetCity(string url)
        {
            var result = new ResponseParseUrlOnAirdna();
            var state = result.State;

            var options = new FirefoxOptions();

            try
            {
                var host = System.Net.Dns.GetHostName();
                var selenoidUrl = $"{_selenoidProtocol}://{_selenoidAddress}:4444/wd/hub";

                _windowMain = new RemoteWebDriver(new Uri(selenoidUrl), options);

                _logger.Debug($"Use selenoidUrl:{selenoidUrl}");

            } catch(Exception exception)
            {
                state.ErrorCode = EnumErrorCode.NoConnectToSolenoid;
                state.ExceptionMessage = exception.Message;

                _logger.Error($"Error create instance web-driver. {state.ExceptionMessage}");
            }

            if (!state.HasError)
            {
                _waitMain = new WebDriverWait(_windowMain, TimeSpan.FromSeconds(60));

                try
                {
                    _windowMain.Navigate().GoToUrl(url);
                    foreach (var i in Enumerable.Range(1, 7))
                    {
                        Thread.Sleep(1000);
                        var readyState = _windowMain.ExecuteScript("return document.readyState");
                        //if (readyState != null && (readyState as string) == "complete") break;
                    }

                } catch (Exception exception)
                {
                    state.ErrorCode = EnumErrorCode.Airdna_InvalidUrlForParsing;
                    state.ExceptionMessage = exception.Message;

                    _logger.Error($"Error navigate to url:{url}. {state.ExceptionMessage}");
                }

                if (!state.HasError)
                {
                    IReadOnlyCollection<object> netObjects = (IReadOnlyCollection<object>)_windowMain.ExecuteScript("return window.performance.getEntries();");

                    string cityId = "", cityName = "", cityOriginalName = "";

                    var splitUrl = url.Split("/");
                    if (splitUrl.Length > 2)
                    {
                        cityName = splitUrl[splitUrl.Length - 2].ToLower();
                    }

                    _logger.Debug($"netObject.count:{netObjects.Count}");

                    foreach (Dictionary<string, object> netObject in netObjects)
                    {
                        var nameObject = netObject.Where(x => x.Key == "name").Select(x => x.Value).FirstOrDefault();
                        if (nameObject != null)
                        {
                            string name = (string)nameObject;
                            var existsCityId = name.ToLower().Contains("city_id=");
                            if (existsCityId)
                            {
                                cityId = name.Split("city_id=")[1].Split("&")[0];
                                break;
                            }
                        }
                    }

                    _logger.Debug($"cityId:{cityId}, cityName={cityName}");

                    if (!string.IsNullOrEmpty(cityId) && !string.IsNullOrEmpty(cityName))
                    {
                        var elementCityOriginalName = _windowMain.FindElementByClassName("Select-placeholder");
                        var elementCityOriginalNameText = elementCityOriginalName.Text;

                        if (!string.IsNullOrWhiteSpace(elementCityOriginalNameText))
                        {
                            cityOriginalName = elementCityOriginalNameText;
                        }
                        else cityOriginalName = cityName;

                        result.City.CityName = cityName;
                        result.City.CityId = cityId;
                        result.City.CityOriginalName = cityOriginalName;
                    }
                    else
                    {
                        state.ErrorCode = EnumErrorCode.Airdna_CannotDetectCityId;
                        state.ExceptionMessage = "Cannot detect cityId";
                    }
                }

                _windowMain.Quit();
            }

            return result;
        }
        public SelenoidService UseSelenoidAddress(string selenoidAddress)
        {
            if (selenoidAddress != null) _selenoidAddress = selenoidAddress;

            _logger.Debug($"Setting selenoidAddress = {selenoidAddress}");

            return this;
        }
        public SelenoidService UseSelenoidProtocol(string selenoidProtocol)
        {
            if (selenoidProtocol != null) _selenoidProtocol = selenoidProtocol;

            _logger.Debug($"Setting selenoidProtocols = {selenoidProtocol}");

            return this;
        }
    }
}
