using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NsTestFrameworkUI.Helpers
{
    internal class ScreenShot
    {
        private static string GetTimestamp(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss");
        }

        public static void TakeAndAttachScreenShot(TestContext testContext)
        {
            var screenShot = ((ITakesScreenshot)Browser.WebDriver).GetScreenshot();
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
            var imageName = $"{testContext.TestName}_{GetTimestamp(DateTime.Now)}.png";
            var path = Path.Combine(folderPath, imageName);
            Directory.CreateDirectory(folderPath);
            screenShot.SaveAsFile(path);
            testContext.AddResultFile(path);
        }
    }
}
