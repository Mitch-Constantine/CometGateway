using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatiN.Core;

namespace CometGateway.Server.TelnetDemo.Tests.View
{
    [TestClass]
    public class JavascriptTest
    {
        [TestMethod]
        public void RunQUnitTests()
        {
            using (var ie = new IE("http://localhost/CometGateway.Server.TelnetDemo/Test"))
            {
                var failed = ie.Span(Find.ByClass("failed")).InnerHtml;
                Console.WriteLine(failed);
                Assert.AreEqual("0", failed);
            }
        }
    }
}
