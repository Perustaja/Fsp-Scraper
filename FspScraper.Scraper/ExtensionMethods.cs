using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection.Emit;
using System.Linq;
using System.Collections.Generic;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using FspScraper.Common;
using OpenQA.Selenium.Support.UI;
using FspScraper.Common.Models;
using SeleniumExtras.WaitHelpers;

namespace FspScraper.Scraper
{
    public static class ExtensionMethods
    {
        // Waits until the xpath exists, and then returns the count of matching elements
        public static int WaitThenCountElement(this IWebDriver driver, WebDriverWait wait, string xpath)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath(xpath)));

            return driver.FindElements(By.XPath(xpath)).Count;
        }
        // Waits until the element is clickable, then clicks
        public static void WaitThenClickElement(this IWebDriver driver, WebDriverWait wait, string xpath)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(xpath)));
            driver.FindElement(By.XPath(xpath)).Click();
        }
        // Checks if the first element matching the xpath is loaded and then grabs text from all matching elements
        // Will not allow empty strings
        public static List<string> WaitThenGrabElementsText(this IWebDriver driver, WebDriverWait wait, string xpath)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath(xpath)));
            WaitForAngularLoad(driver, wait);
            List<string> list = driver.FindElements(By.XPath(xpath)).Select(e => e.Text).ToList();
            return list;
        }
        public static void WaitForAngularLoad(IWebDriver driver, WebDriverWait wait)
        {
            var angReadyScript = "return angular.element(document.body).injector().get('$http').pendingRequests.length";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            wait.Until(wd => js.ExecuteScript(angReadyScript).ToString() == "0");
        }
    }
}