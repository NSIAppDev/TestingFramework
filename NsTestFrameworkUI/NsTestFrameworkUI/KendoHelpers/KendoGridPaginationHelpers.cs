using NsTestFrameworkUI.Helpers;
using OpenQA.Selenium;

namespace NsTestFrameworkUI.KendoHelpers
{
    public static class KendoGridPaginationHelpers
    {
        public static void GoToLastPageOfGrid(this IWebElement grid)
        {
            grid.FindElement(By.CssSelector(".k-pager-last")).Click();
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }

        public static void GoToFirstPageOfGrid(this IWebElement grid)
        {
            grid.FindElement(By.CssSelector(".k-pager-first")).Click();
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }

        public static int NumberOfPages(this IWebElement grid)
        {
            grid.GoToLastPageOfGrid();
            var pageNumber = grid.FindElement(By.ClassName("k-state-selected")).Text;
            grid.GoToFirstPageOfGrid();
            return int.Parse(pageNumber);
        }

        public static void GoToPreviousPage(this IWebElement grid)
        {
            grid.FindElement(By.LinkText("Go to the previous page")).Click();
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }

        public static void GoToNextPage(this IWebElement grid)
        {
            grid.FindElement(By.LinkText("Go to the next page")).Click();
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }
        
        public static int GetNumberOfItemsListedOnPageBottom(this IWebElement grid)
        {
            var numberOfItemsString = grid.FindElement(By.CssSelector("span.k-pager-info.k-label")).Text;
            if (numberOfItemsString == "No items to display")
            {
                return 0;
            }
            var numberOfItems = int.Parse(numberOfItemsString.Split(' ')[4]);
            return numberOfItems;
        }
    }
}
