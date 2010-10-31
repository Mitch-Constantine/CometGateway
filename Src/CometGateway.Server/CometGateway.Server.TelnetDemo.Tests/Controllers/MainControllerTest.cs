using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CometGateway.Server.TelnetDemo.Controllers;
using CometGateway.Server.TelnetDemo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CometGateway.Server.TelnetDemo.Tests.Controllers
{
    [TestClass]
    public class MainControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            var result = new MainController().Index() as ViewResult;
            Assert.AreEqual("", result.ViewName);
        }
        [TestMethod]
        public void Test_TestMode()
        {
            var result = new MainController().Test() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
            Assert.AreEqual(true, result.ViewData[TestModeConstants.ENABLE_TEST_MODE]);
        }
    }
}
