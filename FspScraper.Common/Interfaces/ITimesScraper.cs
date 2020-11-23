using System.Collections.Generic;
using FspScraper.Common.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FspScraper.Common.Interfaces
{
    public interface ITimesScraper
    {
        ISet<FspTimes> Run();
        void Login(IWebDriver driver);
        ISet<FspTimes> ScrapeTimes(IWebDriver driver, WebDriverWait wait);
    }
}