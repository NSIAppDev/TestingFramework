using NsTestFrameworkUI.Helpers;
using OpenQA.Selenium;

namespace NsTestFrameworkUI.KendoHelpers
{
    public static class KendoGridFilteringHelpers
        {
        public static void OpenFilteringDropdownForSpecifiedHeaderColumn(IWebElement columnDropdown)
        {
            columnDropdown.Click();
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }

        public static void SelectFilterOption()
        {
            Browser.WebDriver.FindElement(By.ClassName("k-filter")).Click();
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }

        public static void SendFilterCriteria(string criteria)
        {
            Browser.WebDriver.FindElement(By.ClassName("k-textbox")).SendKeys(criteria);
        }

        public static void SelectFilterButton()
        {
            Browser.WebDriver.FindElement(By.ClassName("k-primary")).Click();
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }

        public static void ClearPreviousFilteringCriteria()
        {
          Browser.WebDriver.FindElement(By.CssSelector("li form div button:nth-child(2)")).Click();
          WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }
    }
}
