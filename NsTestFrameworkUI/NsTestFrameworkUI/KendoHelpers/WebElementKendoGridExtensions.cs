using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using NsTestFrameworkUI.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace NsTestFrameworkUI.KendoHelpers
{
    public static class WebElementKendoGridExtensions
    {
        private static readonly string _dataColumnCssSelector = "[ng-bind],[ng-bind-custom]";

        public static List<KendoGridHeaderModel> GetGridHeaders(this IWebElement grid)
        {
            var lockedHeaderElementsCount =
                grid.FindElements(By.CssSelector(".k-grid-header-locked table th[data-index]")).Count;

            var headerElements = grid.FindElements(By.CssSelector(".k-grid-header-wrap table th[data-index]"));
            var models = headerElements.Select(webElement =>
            {
                var dataField = webElement.GetAttribute("data-field");
                if (!string.IsNullOrEmpty(dataField))
                    return new KendoGridHeaderModel
                    {
                        Index = GetColumnIndexFromHeader(webElement) - lockedHeaderElementsCount,
                        PropertyName = dataField,
                        Text = webElement.Text,
                        HeaderWebElement = webElement
                    };
                ((IJavaScriptExecutor)Browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", webElement);
                dataField = webElement.Text.ConvertToValidCSharpPropertyName();

                return new KendoGridHeaderModel
                {
                    Index = GetColumnIndexFromHeader(webElement) - lockedHeaderElementsCount,
                    PropertyName = dataField,
                    Text = webElement.Text,
                    HeaderWebElement = webElement
                };
            }).OrderBy(_ => _.Index).ToList();
            return models;
        }


        public static TGridViewModel GetFirstKendoGridData<TGridViewModel>(this IWebElement grid,
            Expression<Func<TGridViewModel, bool>> condition = null)
            where TGridViewModel : class, new()
        {
            var headers = grid.GetGridHeaders();
            return grid.GetKendoGridRow(condition).ParseRowData<TGridViewModel>(headers);
        }

        public static List<TGridViewModel> GetKendoGridData<TGridViewModel>(this IWebElement grid)
            where TGridViewModel : class, new()
        {
            var headers = grid.GetGridHeaders();
            return grid
                .GetKendoGridRows()
                .Select(gr => gr.ParseRowData<TGridViewModel>(headers)).Where(m => m != null)
                .ToList();
        }

        public static IWebElement GetKendoGridRow<TGridViewModel>(this IWebElement grid,
            Expression<Func<TGridViewModel, bool>> condition)
            where TGridViewModel : class, new()
        {
            var headers = grid.GetGridHeaders();
            return grid
                .GetKendoGridRows()
                .FirstOrDefault(r =>
                    condition == null || condition.Compile().Invoke(r.ParseRowData<TGridViewModel>(headers)));
        }


        private static TGridViewModel ParseColumns<TGridViewModel>(ReadOnlyCollection<IWebElement> columns,
              TGridViewModel dataItem, List<KendoGridHeaderModel> headers)
        {
            foreach (var header in headers)
            {
                if (HeaderIsEmpty(header))
                    continue;

                var columnWebElement = columns[header.Index];
                ((IJavaScriptExecutor)Browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", columnWebElement);
                var value = columnWebElement.Text;
                SetProperty(dataItem, header.PropertyName.Capitalize(), value);
            }

            return dataItem;
        }

        private static bool HeaderIsEmpty(KendoGridHeaderModel header)
        {
            return string.IsNullOrEmpty(header.PropertyName);
        }

        public static void SetKendoGridInlineEditRowData(this IWebElement row, string property, string value,
            bool isJsonObject = false)
        {
            AssertIsKendoGridRow(row);

            var readOnlyControl = row
                .FindElements(By.CssSelector(_dataColumnCssSelector))
                .FirstOrDefault(c => GetPropertyName(c).Equals(property));

            if (!IsValidDataColumn(readOnlyControl)) return;
            var camelCasePropertyName = $"{char.ToLower(property[0])}{property.Substring(1)}";

            var executor = (IJavaScriptExecutor)Browser.WebDriver;
            executor.ExecuteScript("arguments[0].click();", readOnlyControl);
            SetValue(row, value, camelCasePropertyName, isJsonObject);
        }

        private static void SetValue(IWebElement row, string value, string camelCasePropertyName,
            bool isJsonObject = false)
        {
            var jsExecutor = (IJavaScriptExecutor)Browser.WebDriver;
            var editorCssSelector = $"input[name='{camelCasePropertyName}']";
            var gridElementSelector = $@"$($(""{editorCssSelector}"").parents('[kendo-grid]')[0])";

            row.FindElement(By.CssSelector(editorCssSelector));

            var changeInputEditValue = $@"$(""{editorCssSelector}"").val('{value}')";
            jsExecutor.ExecuteScript(changeInputEditValue);

            var triggerKendoEditDataChange = $@"
                var item = _.find({gridElementSelector}.data('kendoGrid').dataSource.data(), function(item){{
                    return item.uid == '{row.GetAttribute("data-uid")}';
                }}); 

                item.set('{camelCasePropertyName}', {(!isJsonObject ? "'" : string.Empty)}{value}{(!isJsonObject ? "'" : string.Empty)});
                item.trigger('change');";

            jsExecutor.ExecuteScript(triggerKendoEditDataChange);
        }

        public static void SetKendoGridInlineWithButtonsEditRowData(this IWebElement row, string property, string value)
        {
            AssertIsKendoGridRow(row);

            var camelCasePropertyName = $"{char.ToLower(property[0])}{property.Substring(1)}";

            var editControl = row.FindElement(By.CssSelector($"input[name='{camelCasePropertyName}']"));
            editControl.Clear();
            editControl.SendKeys(value);
        }

        private static void AssertIsKendoGridRow(IWebElement control)
        {
            var isKendoGridRow =
                Browser.WebDriver.ExecuteJavaScript<bool>(
                    $"return $(\"[data-uid='{control.GetAttribute("data-uid")}']\").parents('[kendo-grid]').length > 0");
            if (!isKendoGridRow)
                throw new InvalidOperationException("Element is not a kendo grid row");
        }

        private static ReadOnlyCollection<IWebElement> GetKendoGridRows(this IWebElement grid)
        {
            return grid.FindElements(By.CssSelector(".k-grid-content table tr"));
        }

        private static string GetPropertyName(IWebElement dataColumn)
        {
            var attributesToMatch = _dataColumnCssSelector.Split(',').Select(t => t.Replace("[", "").Replace("]", ""));
            return attributesToMatch
                .Select(attr => dataColumn.GetAttribute(attr)?.Replace("dataItem.", ""))
                .Select(prop => prop != null ? $"{prop.Capitalize()}" : string.Empty)
                .FirstOrDefault(prop => !string.IsNullOrEmpty(prop));
        }

        private static bool IsValidDataColumn(IWebElement dataColumn)
        {
            return dataColumn != null;
        }

        private static TGridViewModel ParseRowData<TGridViewModel>(this IWebElement gridRow,
             List<KendoGridHeaderModel> headers)
             where TGridViewModel : class, new()
        {
            var columns = gridRow.GetRowCells();
            if (IsGroupingRow(gridRow))
                return null;

            var dataItem = new TGridViewModel();
            return ParseColumns(columns, dataItem, headers);
        }

        private static bool IsGroupingRow(IWebElement gridRow)
        {
            return gridRow.GetAttribute("class").Contains("k-grouping-row");
        }

        private static ReadOnlyCollection<IWebElement> GetRowCells(this IWebElement gridRow)
        {
            return gridRow.FindElements(By.CssSelector("td[role=\"gridcell\"]"));
        }

        public static void SetProperty<T>(T obj, string propertyName, string value)
        {
            var type = typeof(T);

            if (!obj.HasProperty(propertyName)) return;

            if (DateTime.TryParseExact(value, "MM-dd-yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var date))
            {
                type.GetProperty(propertyName)?.SetValue(obj, date);
                return;
            }

            var propertyType = Nullable.GetUnderlyingType(type.GetProperty(propertyName).PropertyType) ??
                               type.GetProperty(propertyName).PropertyType;

            var safeValue = string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, propertyType);

            type.GetProperty(propertyName)?.SetValue(obj, safeValue);
        }

        private static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static int GetColumnIndexFromHeader(IWebElement column)
        {
            return int.Parse(column.GetAttribute("data-index"));
        }

        public static IWebElement GetColumnSpecifiedByIndex(this IWebElement grid, int rowIndex, int columnIndex)
        {
            var rows = grid.GetAllRowsFromGrid();
            var cells = rows[rowIndex].FindElements(By.TagName("td"));
            return cells[columnIndex];
        }

        public static IWebElement GetColumnSpecifiedByIndex(this IWebElement grid, By rowSelector, int rowIndex, int columnIndex)
        {
            var rows = grid.GetAllRowsFromGrid(rowSelector);
            var cells = rows[rowIndex].FindElements(By.TagName("td"));
            return cells[columnIndex];
        }

        public static IWebElement GetLinkForSpecifiedColumnIndex(this IWebElement grid, int rowIndex, int columnIndex)
        {
            var rows = grid.GetAllRowsFromGrid();
            var cells = rows[rowIndex].FindElements(By.TagName("td"));
            return cells[columnIndex].FindElement(By.TagName("a"));
        }

        public static ReadOnlyCollection<IWebElement> GetAllRowsFromGrid(this IWebElement grid)
        {
            return grid.FindElements(By.CssSelector(".k-grid-content table tr"));
        }

        public static ReadOnlyCollection<IWebElement> GetAllRowsFromGrid(this IWebElement grid, By rowSelector)
        {
            return grid.FindElements(rowSelector);
        }

        public static IWebElement GetButtonWithSpecifiedLinkTextFromSpecifiedRow(this IWebElement grid, string linkText,
            int rowIndex)
        {
            var rows = grid.GetAllRowsFromGrid();
            return rows[rowIndex].FindElement(By.LinkText(linkText));
        }

        public static IWebElement GetButtonWithSpecifiedLinkTextFromSpecifiedRow(this IWebElement grid, By gridSelector, string linkText, int rowIndex)
        {
            var rows = grid.GetAllRowsFromGrid(gridSelector);
            return rows[rowIndex].FindElement(By.LinkText(linkText));
        }
    }
}