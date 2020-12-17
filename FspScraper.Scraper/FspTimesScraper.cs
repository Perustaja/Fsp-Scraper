using System.Collections.Generic;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using FspScraper.Common.Models;
using FspScraper.Common.Options;
using Microsoft.Extensions.Options;
using FspScraper.Common.Interfaces;
using FspScraper.Common;

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
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
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
            while (driver.Url != _loginOptions.AircraftUrl)
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
                var labels = new List<string>();
                var values = new List<string>();

                // Regnum
                labels.Add(LabelConstants.REGNUM);
                values.Add(driver.WaitThenGrabElementText(wait, "//div/*[@class='modal-body-title ng-binding']"));
                // Hobbs
                labels.Add(LabelConstants.HOBBS);
                values.Add(WaitThenGrabTimeValue(driver, wait, "times.hobbsMeter.enabled"));
                // AirTime - note airtime element exists even if aircraft does not track it
                labels.Add(LabelConstants.AIRTIME);
                values.Add(WaitThenGrabTimeValue(driver, wait, "times.airTimeMeter.enabled"));
                // Add in order Engine1, Prop1, Engine2, Prop2
                labels.AddRange(new List<string> { LabelConstants.ENG1, LabelConstants.PROP1, LabelConstants.ENG2, LabelConstants.PROP2 });
                // Add Engine1, Prop1
                values.Add(driver.WaitThenGrabElementTextOrNull(wait, "//div[@ng-repeat='engine in times.engines track by $index'][1]/div[@class='modal-section'][1]/div[@class='row']/div[@class='field-value-container ng-binding']/preceding-sibling::label[contains(text(),'Total Time in Service (TTIS)')]/following-sibling::div"));
                values.Add(driver.WaitThenGrabElementTextOrNull(wait, "//div[@ng-repeat='engine in times.engines track by $index'][1]/div[@class='modal-section'][2]/div[@class='row'][2]/div[@class='field-value-container ng-binding']/preceding-sibling::label[contains(text(), 'Total Time in Service (TTIS)')]/following-sibling::div"));
                // Add Engine2, Prop2 if necessary, elements do NOT exist for single engine aircraft
                values.Add(driver.WaitThenGrabElementTextOrNull(wait, "//div[@ng-repeat='engine in times.engines track by $index'][2]/div[@class='modal-section'][1]/div[@class='row']/div[@class='field-value-container ng-binding']/preceding-sibling::label[contains(text(),'Total Time in Service (TTIS)')]/following-sibling::div"));
                values.Add(driver.WaitThenGrabElementTextOrNull(wait, "//div[@ng-repeat='engine in times.engines track by $index'][2]/div[@class='modal-section'][2]/div[@class='row'][2]/div[@class='field-value-container ng-binding']/preceding-sibling::label[contains(text(), 'Total Time in Service (TTIS)')]/following-sibling::div"));

                // Parse values, add to set
                timesSet.Add(_parser.ParseTimes(labels, values));
                // Close modal
                driver.WaitThenClickElement(wait, "//button[@class='close']");
                // Invisibility check seems sporadic, add extra wait afterwards (sometimes click is intercepted).
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//button[@class='close']")));
            }
            return timesSet;
        }

        /// <summary>
        /// Grabs the time value of the aircraft based on the ng-show attribute associated with the value.
        /// e.g. the ng-show attribute of hobbs time is times.hobbsMeter.enabled.
        /// </summary>
        /// <param name="ngShowValue">ng-show attribute associated with the value.</param>
        /// <returns></returns>
        private string WaitThenGrabTimeValue(IWebDriver driver, WebDriverWait wait, string ngShowValue)
            => driver.WaitThenGrabElementText(wait, $"//div[@class='modal-section']/div[@ng-show='{ngShowValue}']/div[@class='field-value-container ng-binding']");
    }
}