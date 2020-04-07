using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NsTestFrameworkUI.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

namespace NsTestFrameworkUI.Pages
{
    public static class PageHelpers
    {
        public static T InitPage<T>(T page)
        {
            PageFactory.InitElements(Browser.WebDriver, page);
            return page;
        }

        public static IWebElement Enable(this IWebElement webElement)
        {
            Browser.WebDriver.ExecuteJavaScript("arguments[0].removeAttribute('disabled')", webElement);
            return webElement;
        }

        public static void Hover(this IWebElement webElement)
        {
            const string hoverJs = "var evObj = document.createEvent('MouseEvents');" +
                                   "evObj.initMouseEvent(\"mouseover\",true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);" +
                                   "arguments[0].dispatchEvent(evObj);";
            ((IJavaScriptExecutor)Browser.WebDriver).ExecuteScript(hoverJs, webElement);
        }

        public static void HoverTheMouse(this IWebElement element)
        {
            var menuHover = new Actions(Browser.WebDriver);
            menuHover.MoveToElement(element).Perform();
        }

        public static void SwitchToLastTab()
        {
            var windowHandles = Browser.WebDriver.WindowHandles;
            var lastTabIndex = windowHandles.Count - 1;
            var lastTab = windowHandles[lastTabIndex];
            Browser.WebDriver.SwitchTo().Window(lastTab);
        }

        public static void ScrollPageToTop()
        {
            try
            {
                ((IJavaScriptExecutor)Browser.WebDriver).ExecuteScript(
                    "document.body.scrollTop = document.documentElement.scrollTop = 0;");
                ((IJavaScriptExecutor)Browser.WebDriver).ExecuteScript("window.scrollTo(0, 0);");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Trace.WriteLine(ex.InnerException.Message);
                }
            }
        }

        public static void ScrollDownToView(int value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            ((IJavaScriptExecutor)Browser.WebDriver).ExecuteScript($"window.scrollTo(0, document.body.scrollHeight - {value})");
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }

        public static void ScrollUpToView()
        {
            ((IJavaScriptExecutor)Browser.WebDriver).ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
        }

        public static ReadOnlyCollection<IWebElement> GetElements(this By selector)
        {
            return Browser.WebDriver.FindElements(selector);
        }

        public static void SelectOptionFromDropdown(this By dropdownElement, string option)
        {
            var dropdown = Browser.WebDriver.FindElement(dropdownElement);
            var selectElement = new SelectElement(dropdown);
            selectElement.SelectByText(option);
        }

        public static void SelectFromDropdown(this By dropdown, By optionsList, string option)
        {
            var element = Browser.WebDriver.FindElement(dropdown);
            element.Click();
            IList<IWebElement> dropdownList = element.FindElements(optionsList);
            dropdownList.First(x => string.Equals(x.Text, option)).Click();
        }

        public static string GetCssValue(this IWebElement element, string cssValue)
        {
            return element.GetCssValue(cssValue);
        }

        private static bool Exists(this By selector)
        {
            var count = Browser.WebDriver.FindElements(selector).Count;
            return count > 0;
        }

        public static void ClearField(this By selector)
        {
            selector.WaitForElement();
            if (!selector.Exists()) return;

            Browser.WebDriver.FindElement(selector).Clear();
        }

        public static void ActionClick(this By selector)
        {
            selector.WaitForElementToBeClickable();
            if (!selector.Exists()) return;

            Browser.WebDriver.FindElement(selector).Click();
        }

        public static void ActionSendKeys(this By selector, string text)
        {
            selector.WaitForElement();
            if (!selector.Exists()) return;

            selector.ClearField();
            Browser.WebDriver.FindElement(selector).SendKeys(text);
        }

        public static string GetText(this By selector)
        {
            try
            {
                return Browser.WebDriver.FindElement(selector).Text;
            }
            catch (WebDriverException)
            {
                return string.Empty;
            }
        }

        public static bool IsElementEnabled(this By selector) => Browser.WebDriver.FindElement(selector).Enabled;

        public static bool IsElementPresent(this By selector)
        {
            try
            {
                return Browser.WebDriver.FindElement(selector).Displayed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool AreElementsPresent(this By selector)
        {
            var errorMessages = Browser.WebDriver.FindElements(selector);
            try
            {
                return errorMessages.All(x => x.Displayed);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsAlertPresent()
        {
            try
            {
                AcceptAlert();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void AcceptAlert()
        {
            Thread.Sleep(WaitHelpers.SleepIntervalInMilliseconds);
            Browser.WebDriver.SwitchTo().Alert().Accept();
        }

        public static bool IsElementSelected(this By cssSelector) => Browser.WebDriver.FindElement(cssSelector).Selected;

        public static bool IsElementEnabled(this By cssSelector, string attributeName)
        {
            var platform = Browser.WebDriver.FindElement(cssSelector);
            return !platform.GetAttribute(attributeName).Contains("disabled");
        }

        public static string GetAttribute(this By selector, string attributeName)
        {
            return Browser.WebDriver.FindElement(selector).GetAttribute(attributeName);
        }

    }
}
