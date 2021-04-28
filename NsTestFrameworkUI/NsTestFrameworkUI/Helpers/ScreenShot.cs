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

        public static void TakeAndAttachScreenShot(string testName)
        {
            var screenShot = ((ITakesScreenshot)Browser.WebDriver).GetScreenshot();

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
            var imageName = $"{testName}_{GetTimestamp(DateTime.Now)}.png";
            var imagePath = Path.Combine(folderPath, imageName);

            Directory.CreateDirectory(folderPath);
            screenShot.SaveAsFile(imagePath);
        }
    }
}
