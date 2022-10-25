using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;


namespace NsTestFrameworkUI.Helpers
{
    public class FileHelper
    {
        public static void DeleteFileByPath(string path)
        {
            File.Delete(path);
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

        public static void WaitUntilFileExists(string filePath)
        {
            var fluentWait = new DefaultWait<IWebDriver>(Browser.WebDriver) { Timeout = TimeSpan.FromSeconds(40) };

            fluentWait.Until(x => File.Exists(filePath));
        }

        public static bool IsFilePresent(string path)
        {
            return File.Exists(path);
        }

        public static int GetNumberOfLinesFromExcel(string filePath, int worksheetIndex = 0)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var package = new ExcelPackage(new FileInfo(filePath));

            var worksheet = package.Workbook.Worksheets[worksheetIndex];

            return worksheet.Dimension.End.Row - 1;
        }

        public static List<string> GetColumnsNameFromExcel(string filePath, int worksheetIndex = 0)
        {
            var excelColumnsName = new List<string>();
            var file = new FileInfo(filePath);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excel = new ExcelPackage(file))
            {
                var sheet = excel.Workbook.Worksheets[worksheetIndex];

                excelColumnsName.AddRange(sheet.Cells[sheet.Dimension.Start.Row, sheet.Dimension.Start.Column, 1, sheet.Dimension.End.Column]
                    .TakeWhile(firstRowCell => firstRowCell.Text.Length != 0).Select(firstRowCell => firstRowCell.Text));
            }
            return excelColumnsName;
        }

        public static void EnableFileForEditing(string filePath, int worksheetIndex = 0)
        {
            WaitHelpers.ExplicitWait();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var xlWorkbook = new ExcelPackage(new FileInfo(filePath));

            var workSheet = xlWorkbook.Workbook.Worksheets[worksheetIndex];
            workSheet.Protection.IsProtected = true;
            xlWorkbook.Save();
        }

        public static DataTable GetRowsFromExcel(string filePath, int worksheetIndex = 0)
        {
            var dataTable = new DataTable();
            var file = new FileInfo(filePath);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excel = new ExcelPackage(file))
            {
                var workSheet = excel.Workbook.Worksheets[worksheetIndex];

                for (var rowIndex = workSheet.Dimension.Start.Row; rowIndex <= workSheet.Dimension.End.Row; rowIndex++)
                {
                    if (rowIndex == 1)
                        for (var columnIndex = workSheet.Dimension.Start.Column; columnIndex <= workSheet.Dimension.End.Column; columnIndex++)
                        {
                            var cellValue = workSheet.Cells[rowIndex, columnIndex].Value.ToString();
                            dataTable.Columns.Add(cellValue);
                        }

                    if (rowIndex == 1) continue;

                    var row = workSheet.Cells[rowIndex, 1, rowIndex, workSheet.Dimension.End.Column];
                    var newRow = dataTable.NewRow();

                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            return dataTable;
        }
    }
}
