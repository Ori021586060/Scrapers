using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperCore
{
    public class SelenoidStateModel
    {
        public  RemoteWebDriver WindowMain { get; set; }
        public WebDriverWait WaitMain { get; set; }
        public bool JavaScriptEnable { get; set; } = true;
        public bool ShowPictures { get; set; } = false;
    }
}
