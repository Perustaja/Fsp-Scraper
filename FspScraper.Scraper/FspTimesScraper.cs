using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Collections.Generic;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using FspScraper.Common;
using FspScraper.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FspScraper.Scraper
{
    public class FspTimesScraper
    {
        public ISet<FspTimes> Run()
        {
            IWebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            Login(driver, Constants.FSP_AIRCRAFT_URL);
            var times = ScrapeTimes(driver, wait);
            driver.Quit();
            return times;
        }
        public static void Login(IWebDriver driver, string selectedUrl)
        {
            // navigate to login page and login
            driver.Navigate().GoToUrl(selectedUrl);
            driver.FindElement(By.Id("username")).SendKeys(Constants.FSP_LOGIN_USER);
            driver.FindElement(By.Id("password")).SendKeys(Constants.FSP_LOGIN_PASS + Keys.Enter);
            var url = driver.Url;
            Assert.AreEqual(selectedUrl, url);
        }

        public static ISet<FspTimes> ScrapeTimes(IWebDriver driver, WebDriverWait wait)
        {
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
                timesSet.Add(FspTimesParser.ParseTimes(labels, values));

                driver.WaitThenClickElement(wait, "//button[@class='close']");
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[@class='close']")));
            }
            return timesSet;
        }
    }
}