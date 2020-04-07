using OpenQA.Selenium;

namespace NsTestFrameworkUI.KendoHelpers
{
    public class KendoGridHeaderModel
    {
        public int Index { get; set; }
        public string PropertyName { get; set; }
        public string Text { get; set; }
        public IWebElement HeaderWebElement { get; set; }
    }
}