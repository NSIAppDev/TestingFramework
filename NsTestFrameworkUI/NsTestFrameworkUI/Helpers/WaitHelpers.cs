using System;
using System.Threading;
using NsTestFrameworkUI.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace NsTestFrameworkUI.Helpers
{
    public static class WaitHelpers
    {
        public const int WaitTime = 30;
        private const int Milliseconds = 1500;
        public const int SleepIntervalInMilliseconds = 500;
        private const string AngularJs = "angular";
        private const string JQuery = "jQuery";

        public static void WaitUntilNoPendingAjaxRequests()
        {
            if (PageHelpers.IsAlertPresent()) return;
            WaitForDocumentReadyState();
            WaitUntilAngularIsReady();
            WaitUntilJqueryIsReady();
            ExplicitWait();
        }

        public static void WaitForDocumentReadyState()
        {
            IWait<IWebDriver> wait = new WebDriverWait(Browser.WebDriver, TimeSpan.FromSeconds(WaitTime));
            wait.Until(
                d => ((IJavaScriptExecutor)Browser.WebDriver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public static void WaitUntilAngularIsReady()
        {
            var angularIsDefined = WaitForLibraryToBeDefined(AngularJs);
            if (!angularIsDefined) return;

            var wait = new WebDriverWait(Browser.WebDriver, TimeSpan.FromSeconds(WaitTime));
            wait.Until(driver =>
            {
                var isAngularFinished = (bool)((IJavaScriptExecutor)driver).
                    ExecuteScript("return angular.element(document.body).injector().get('$http').pendingRequests.length == 0");
                return isAngularFinished;
            });
        }

        public static void WaitUntilJqueryIsReady()
        {
            var angularIsDefined = WaitForLibraryToBeDefined(JQuery);
            if (!angularIsDefined) return;

            var wait = new WebDriverWait(Browser.WebDriver, TimeSpan.FromSeconds(WaitTime));
            wait.Until(driver =>
            {
                var isJQueryFinished = (bool)((IJavaScriptExecutor)driver).ExecuteScript("return typeof jQuery == \"function\"");
                return isJQueryFinished;
            });
        }

        private static bool WaitForLibraryToBeDefined(string library)
        {
            var javaScriptExecutor = (IJavaScriptExecutor)Browser.WebDriver;

            var isLibraryDefined = (bool)javaScriptExecutor.ExecuteScript($"return window.{library} !== undefined;");

            return isLibraryDefined;
        }

        public static void WaitUntilElementIsVisible(this By selector)
        {
            new WebDriverWait(Browser.WebDriver, TimeSpan.FromSeconds(WaitTime))
                .Until(ExpectedConditions.ElementIsVisible(selector));
        }

        public static void WaitForElement(this By selector)
        {
            var wait = new WebDriverWait(Browser.WebDriver, TimeSpan.FromSeconds(WaitTime));
            wait.Until(d => d.FindElement(selector));
        }

        public static void WaitForSpinner(this By selector)
        {
            while (selector.IsSpinnerPresent())
            {
                Thread.Sleep(SleepIntervalInMilliseconds);
            }
        }

        private static bool IsSpinnerPresent(this By selector)
        {
            try
            {
                Browser.WebDriver.FindElement(selector);
                return true;
            }
            catch (Exception)
            {
                Thread.Sleep(Milliseconds);
                return false;
            }
        }

        public static void ExplicitWait()
        {
            var startingTime = DateTime.Now;
            var wait = new WebDriverWait(Browser.WebDriver, TimeSpan.FromMilliseconds(Milliseconds))
            {
                PollingInterval = TimeSpan.FromMilliseconds(Milliseconds)
            };
            wait.Until(d => DateTime.Now - startingTime - TimeSpan.FromMilliseconds(Milliseconds) > TimeSpan.Zero);
        }

        public static void ExplicitWait(int milliseconds)
        {
            var startingTime = DateTime.Now;
            var wait = new WebDriverWait(Browser.WebDriver, TimeSpan.FromMilliseconds(milliseconds))
            {
                PollingInterval = TimeSpan.FromMilliseconds(milliseconds)
            };
            wait.Until(d => DateTime.Now - startingTime - TimeSpan.FromMilliseconds(milliseconds) > TimeSpan.Zero);
        }

        public static void WaitForElementToBeClickable(this By selector)
        {
            var wait = new WebDriverWait(Browser.WebDriver, TimeSpan.FromSeconds(WaitTime));
            wait.Until(ExpectedConditions.ElementToBeClickable(selector));
        }

        public static void WaitUntilElementIsEnabled(By selector)
        {
            var wait = new WebDriverWait(Browser.WebDriver, TimeSpan.FromSeconds(WaitTime));
            wait.Until(element => element.FindElement(selector).Enabled);
        }

        public static void WaitUntilElementIsNotVisible(this By selector)
        {
            var wait = new WebDriverWait(Browser.WebDriver, TimeSpan.FromSeconds(WaitTime));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(selector));
        }

        public static void WaitUntilTextIsPresentInElementLocated(this By selector, string text)
        {
            var wait = new WebDriverWait(Browser.WebDriver, TimeSpan.FromSeconds(WaitTime));
            wait.Until(ExpectedConditions.TextToBePresentInElementLocated(selector, text));
        }
    }
}
