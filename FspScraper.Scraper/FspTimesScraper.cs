using System.Collections.Generic;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using FspScraper.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FspScraper.Common.Options;
using Microsoft.Extensions.Options;
using FspScraper.Common.Interfaces;
using OpenQA.Selenium.Interactions;

namespace FspScraper.Scraper
{
    public class FspTimesScraper : ITimesScraper
    {
        private readonly LoginOptions _loginOptions;
        private readonly ITimesParser _parser;
        public FspTimesScraper(IOptions<LoginOptions> loginOptionsAccessor, ITimesParser parser) 
        {
            _loginOptions = loginOptionsAccessor.Value ?? throw new ArgumentNullException(nameof(loginOptionsAccessor));
            if (_loginOptions.FspPass == null || _loginOptions.FspUser == null)
                throw new ArgumentNullException("Login options not configured. Please set via user secrets.");
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }
        public ISet<FspTimes> Run()
        {
            IWebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            Login(driver);
            var times = ScrapeTimes(driver, wait);
            driver.Quit();
            return times;
        }
        public void Login(IWebDriver driver)
        {
            // navigate to login page and login
            driver.Navigate().GoToUrl(_loginOptions.LoginUrl);
            driver.FindElement(By.Id("username")).SendKeys(_loginOptions.FspUser);
            driver.FindElement(By.Id("password")).SendKeys(_loginOptions.FspPass);
            driver.FindElement(By.Id("password")).SendKeys(Keys.Enter);
        }

        public ISet<FspTimes> ScrapeTimes(IWebDriver driver, WebDriverWait wait)
        {
            driver.Navigate().GoToUrl(_loginOptions.AircraftUrl);
            // will hold our parsed time models
            var timesSet = new HashSet<FspTimes>();

            // Finds out how many planes are present on the page
            var numPlanes = driver.WaitThenCountElement(wait, "//ul[@class='list-inline gallery ng-scope']/li");
            // Click on the aircraft options dropdown, and then aircraft times
            for (var i = 1; i <= numPlanes; i++)
            {
                driver.WaitThenClickElement(wait, $"//*[@id='main']/ul/li[{i}]/div[1]/a[2]/span");
                driver.WaitThenClickElement(wait, $"//*[@id='main']/ul/li[{i}]/div[1]/ul/li[4]/a");
                
                List<string> labels = driver.WaitThenGrabElementsText(wait, "//form[not (@aria-hidden='true')]/fieldset/div[not (@aria-hidden='true')and not(@class='ng-hide')]//label[(parent::div[not (@aria-hidden='true')])]");
                List<string> values = driver.WaitThenGrabElementsText(wait, "//form[not (@aria-hidden='true')]/fieldset/div[not (@aria-hidden='true')and not(@class='ng-hide')]//div[@class='input-holder' and (parent::div[not (@aria-hidden='true')])]/div");
                timesSet.Add(_parser.ParseTimes(labels, values));

                driver.WaitThenClickElement(wait, "//button[@class='close']");
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[@class='close']")));
            }
            return timesSet;
        }
    }
}