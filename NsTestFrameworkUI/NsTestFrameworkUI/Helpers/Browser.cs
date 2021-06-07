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

        public static void InitializeDriver(DriverOptions driverOptions)
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("no-sandbox");
            options.SetLoggingPreference(LogType.Browser, LogLevel.All);
            options.PageLoadStrategy = PageLoadStrategy.Normal;

            if (!string.IsNullOrEmpty(driverOptions.DownloadDirectoryPath))
            {
                options.AddUserProfilePreference("download.default_directory", driverOptions.DownloadDirectoryPath);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("disable-popup-blocking", "true");
            }

            if (driverOptions.IsHeadless)
            {
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--headless");
            }

            if (driverOptions.IsMobileLayout)
                options.EnableMobileEmulation("iPhone X");

            WebDriver = new ChromeDriver(options);
            WebDriver.Manage().Window.Maximize();
        }

        public static void GoTo(string url)
        {
            WebDriver.Navigate().GoToUrl(url);
            WaitHelpers.WaitUntilNoPendingAjaxRequests();
        }

        public static void Cleanup()
        {
            WebDriver.Close();
            WebDriver.Quit();
        }

        public static void SwitchToLastTab()
        {
            var windowHandles = WebDriver.WindowHandles;
            var lastTabIndex = windowHandles.Count - 1;
            var lastTab = windowHandles[lastTabIndex];
            WebDriver.SwitchTo().Window(lastTab);
        }

        public static void ClearCookies()
        {
            WebDriver.Manage().Cookies.DeleteAllCookies();
        }

        public static void AddCookie(string cookieName, string cookieValue)
        {
            var cookie = new Cookie(cookieName, cookieValue);
            WebDriver.Manage().Cookies.AddCookie(cookie);
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