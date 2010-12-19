using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CometGateway.Server.TelnetDemo.Controllers;
using CometGateway.Server.TelnetDemo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using CometGateway.Server.TelnetDemo.Configuration;

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
        public void NamedServerIsMatchedWithServerAction()
        {
            var mocks = new MockRepository();
            var controller = mocks.PartialMock<MainController>();
            var configurationParser = mocks.StrictMock<ITelnetServerConfigurationParser>();
            controller
                .Expect(c => c.GetConfigurationParser())
                .Return(configurationParser);
            TelnetServerConfiguration[] serverConfiguration =
                new[]
                {
                    new TelnetServerConfiguration
                    {
                        Name = "bogus",
                        Server = "bogus.org",
                        Port = 4000
                    },
                    new TelnetServerConfiguration
                    {
                        Name = "test",
                        Server = "aardwolf.org",
                        Port = 4000
                    }
                };
            configurationParser.Expect(c => c.Parse())
                .Return(serverConfiguration);

            mocks.ReplayAll();
            ActionResult result = controller.GetNamedServerActionResult("teSt");
            Assert.AreEqual( 
                "Index",
                (result as ViewResult).ViewName
            );
            Assert.AreEqual("aardwolf.org", controller.ViewData["server"]);
            Assert.AreEqual(4000, controller.ViewData["port"]);
            mocks.VerifyAll();
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
