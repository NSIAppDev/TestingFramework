namespace NsTestFrameworkUI.Helpers
{
    public class DriverOptions
    {
        public string ChromeDriverPath { get; set; }

        public bool IsHeadless { get; set; } = false;

        public string DownloadDirectoryPath { get; set; }

        public bool IsMobileLayout { get; set; } = false;
    }
}
