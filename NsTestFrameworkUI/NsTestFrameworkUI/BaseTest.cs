using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsTestFrameworkUI.Helpers;

namespace NsTestFrameworkUI
{
    public class BaseTest
    {
        public TestContext TestContext { get; set; }

        [TestCleanup]
        public virtual void After()
        {
            if (TestContext.CurrentTestOutcome.Equals(UnitTestOutcome.Failed))
                ScreenShot.TakeAndAttachScreenShot(TestContext);
            Browser.Cleanup();
        }
    }
}
