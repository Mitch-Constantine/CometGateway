using System;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CometGateway.Server.TelnetDemo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using CometGateway.Server.TelnetDemo.Common;

namespace CometGateway.Server.TelnetDemo.Tests.Common
{
    [TestClass]
    public class EnableQUnitTest
    {
        [TestMethod]
        public void EnableQUnitEmptyIfNotTestMode()
        {
            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            var viewContext = MockRepository.GenerateStub<ViewContext>();

            viewContext.HttpContext = httpContext;
            viewContext.RequestContext = new RequestContext( httpContext, new RouteData() );
            var html = new HtmlHelper( viewContext, new ViewPage() );
            Assert.AreEqual( "", html.EnableQUnitTesting() );
        }

        [TestMethod]
        public void EnableQUnitIfTestMode()
        {
            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            var viewContext = MockRepository.GenerateStub<ViewContext>();
            var httpRequest = MockRepository.GenerateStub<HttpRequestBase>();
            httpContext.Stub(c => c.Request).Return(httpRequest).Repeat.Any();
            httpRequest.Stub(c => c.ApplicationPath).Return("http://localhost/CometGateway.Server.TelnetDemo");
            viewContext.HttpContext = httpContext;
            viewContext.RequestContext = new RequestContext( httpContext, new RouteData() );

            var viewData = new ViewDataDictionary() {{TestModeConstants.ENABLE_TEST_MODE, true}};
            viewContext.ViewData = viewData;
            var view = new WebFormView("~/Views/Main/Index.aspx");
            viewContext.View = view;

            var html = new HtmlHelper( viewContext, new ViewPage() );
            html.ViewDataContainer.ViewData = viewData;
            
            const string expectedSnippet =
  @"
        <link href=""http://localhost/CometGateway.Server.TelnetDemo/Content/QUnit.css"" rel=""stylesheet"" type=""text/css""></link>
        <script src=""http://localhost/CometGateway.Server.TelnetDemo/Scripts/qunit.js"" type=""text/javascript""></script>
        <script src=""http://localhost/CometGateway.Server.TelnetDemo/Scripts/Views/Main/IndexTest.js"" type=""text/javascript""></script>
        <div id='testResults'>
        <h1 id='qunit-header'></h1>
        <h2 id='qunit-banner'></h2>
        <h2 id='qunit-userAgent'></h2>
        <ol id='qunit-tests'></ol>
        <div id='qunit-fixture'></div>
    </div>
    <script type='text/javascript'>var testmode = true;</script>
    <script type='text/javascript'>
        $('body').contents().hide();
        $('#testResults').show();
    </script>";
            TestUtilities.AssertStringsMatch( expectedSnippet, html.EnableQUnitTesting());
        }
    }
}
