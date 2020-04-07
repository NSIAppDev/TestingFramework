using System;
using System.IO;
using System.Threading;

namespace NsTestFrameworkUI.Helpers
{
    public class FileHelper
    {
        public static string FileDownloadsPath(string fileName)
        {
            var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var downloadPath = Path.Combine(userPath, "Downloads");
            return Path.Combine(downloadPath, fileName);
        }

        public static void DeleteFile(string fileName)
        {
            var exportFilePath = FileDownloadsPath(fileName);
            if (!File.Exists(exportFilePath)) return;
            try
            {
                File.Delete(exportFilePath);
            }
            catch
            {
                // ignored
            }
        }

        public static void ClearFolder(string folderPath)
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (var directory in directoryInfo.GetDirectories())
            {
                ClearFolder(directory.FullName);
                directory.Delete();
            }
        }

        public static void WaitUntilFileExists(string fileName)
        {
            var exportFilePath = FileDownloadsPath(fileName);
            while (!File.Exists(exportFilePath))
            {
                Thread.Sleep(WaitHelpers.WaitTime);
            }
        }
    }
}
