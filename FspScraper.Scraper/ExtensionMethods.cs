using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System;

namespace FspScraper.Scraper
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns whether the element at the specific time was found.
        /// </summary>
        public static bool ElementExists(this IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Waits until the xpath exists, and then returns the count of matching elements
        /// </summary>
        public static int WaitThenCountElement(this IWebDriver driver, WebDriverWait wait, string xpath)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath(xpath)));

            return driver.FindElements(By.XPath(xpath)).Count;
        }

        /// <summary>
        /// Waits until the element is clickable, then clicks. If click is intercepted, retries are attempted with 100ms in between each.
        /// </summary>
        /// <exception cref="OpenQA.Selenium.ElementClickInterceptedException">After 10 tries if click is intercepted.</exception>
        public static void WaitThenClickElement(this IWebDriver driver, WebDriverWait wait, string xpath, int retryAttempts = 10)
        {
            int i = 0;
            bool clicked = false;
            while (!clicked && i < retryAttempts)
            {
                try
                {
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(xpath)));
                    driver.FindElement(By.XPath(xpath)).Click();
                    clicked = true;
                }
                catch (ElementClickInterceptedException)
                {
                    Thread.Sleep(100);
                    i++;
                }
            }
        }

        /// <summary>
        /// Checks if the first element matching the xpath is loaded and then grabs text from all matching elements
        /// Will not allow empty strings
        /// </summary>
        /// <returns>List of strings grabbed from all elements matching the xpath.</returns>
        public static List<string> WaitThenGrabElementsText(this IWebDriver driver, WebDriverWait wait, string xpath)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath(xpath)));
            WaitForAngularLoad(driver, wait);
            List<string> list = driver.FindElements(By.XPath(xpath)).Select(e => e.Text).ToList();
            return list;
        }

        /// <summary>
        /// Waits until loading is finished, then grabs the xpath selected. Throws exception if element doesn't exist.
        /// </summary>
        /// <returns>The first string by xpath selection, or null.</returns>
        public static string WaitThenGrabElementText(this IWebDriver driver, WebDriverWait wait, string xpath)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath(xpath)));
            driver.WaitForAngularLoad(wait);
            return driver.FindElement(By.XPath(xpath))?.Text;
        }
        /// <summary>
        /// Returns the text of the selected element, or null. No exception is thrown if element doesn't exist.
        /// </summary>
        /// <returns>List of strings grabbed from all elements matching the xpath, or null.</returns>
        public static string WaitThenGrabElementTextOrNull(this IWebDriver driver, WebDriverWait wait, string xpath)
        {
                if (driver.ElementExists(By.XPath(xpath)))
                {
                    WaitForAngularLoad(driver, wait);
                    return driver.FindElement(By.XPath(xpath))?.Text;
                }
                return String.Empty;
        }

        /// <summary>
        /// Waits until angular pendingRequests length is zero, which is when the DOM should be populated and loading finished.
        /// </summary>
        public static void WaitForAngularLoad(this IWebDriver driver, WebDriverWait wait)
        {
            var angReadyScript = "return angular.element(document.body).injector().get('$http').pendingRequests.length";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            wait.Until(wd => js.ExecuteScript(angReadyScript).ToString() == "0");
        }
    }
}