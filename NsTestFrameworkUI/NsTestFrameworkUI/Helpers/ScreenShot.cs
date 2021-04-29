using System;
using System.IO;
using OpenQA.Selenium;

namespace NsTestFrameworkUI.Helpers
{
    public class ScreenShot
    {
        private static string GetTimestamp(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss");
        }

        public static string GetScreenShotPath(string testName)
        {
            var screenShot = ((ITakesScreenshot)Browser.WebDriver).GetScreenshot();

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
            var imagePath = GetImagePath(folderPath, testName);

            Directory.CreateDirectory(folderPath);
            screenShot.SaveAsFile(imagePath);
            return imagePath;
        }

        private static string GetImagePath(string folderPath, string testName)
        {
            var imageName = $"{testName}_{GetTimestamp(DateTime.Now)}.png";
            return Path.Combine(folderPath, imageName);
        }
    }
}
