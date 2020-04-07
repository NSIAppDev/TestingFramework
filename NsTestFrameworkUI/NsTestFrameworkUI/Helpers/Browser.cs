using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace NsTestFrameworkUI.Helpers
{
    public static class Browser
    {
        [ThreadStatic]
        public static ChromeDriver WebDriver;
        public static ISearchContext Driver => WebDriver;

        public static void InitializeDriver(string chromeDriverPath, bool useHeadless = false, string downloadDirectoryPath = null)
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");
            options.PageLoadStrategy = PageLoadStrategy.Normal;

            if (!string.IsNullOrEmpty(downloadDirectoryPath))
            {
                options.AddUserProfilePreference("download.default_directory", downloadDirectoryPath);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("disable-popup-blocking", "true");
            }

            if (useHeadless)
            {
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--headless");
            }

            WebDriver = new ChromeDriver(chromeDriverPath, options);
        }

        public static void Goto(string url)
        {
            WebDriver.Navigate().GoToUrl(url);
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }

        public static void Cleanup()
        {
            WebDriver.Close();
            WebDriver.Quit();
        }

        public static void DeleteCookie(string cookieName)
        {
            WebDriver.Manage().Cookies.DeleteCookieNamed(cookieName);
        }

        public static void RefreshPage()
        {
            WebDriver.Navigate().Refresh();
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }
    }
}