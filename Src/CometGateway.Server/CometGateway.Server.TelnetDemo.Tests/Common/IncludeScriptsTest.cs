using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Rhino.Mocks;
using System.Web;

using CometGateway.Server.TelnetDemo.Common;

namespace CometGateway.Server.TelnetDemo.Tests.Common
{
    [TestClass]
    public class IncludeScriptsTest
    {
        [TestMethod]
        public void IncludeScriptsReturnsScriptsWithFullPath()
        {

            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            var viewContext = MockRepository.GenerateStub<ViewContext>();
            var httpRequest = MockRepository.GenerateStub<HttpRequestBase>();

            httpContext.Stub( c => c.Request ).Return( httpRequest ).Repeat.Any();

            httpRequest.Stub(c => c.ApplicationPath).Return("http://localhost/TestComet");

            viewContext.HttpContext = httpContext;
            viewContext.RequestContext = new RequestContext( httpContext, new RouteData() );            

            var html = new HtmlHelper( viewContext, new ViewPage() );
            Assert.AreEqual(@"<script src=""http://localhost/TestComet/Scripts/Cometd.js"" type=""text/javascript""></script>" +
                            @"<script src=""http://localhost/TestComet/Scripts/OtherScript.js"" type=""text/javascript""></script>",
                            html.IncludeScript("Cometd.js", "OtherScript.js"));
        }

        [TestMethod]
        public void IncludeScriptsReturnsScriptsWithFullPathIfPathHasTrailingSlash()
        {

            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            var viewContext = MockRepository.GenerateStub<ViewContext>();
            var httpRequest = MockRepository.GenerateStub<HttpRequestBase>();

            httpContext.Stub(c => c.Request).Return(httpRequest).Repeat.Any();

            httpRequest.Stub(c => c.ApplicationPath).Return("http://localhost/TestComet/");

            viewContext.HttpContext = httpContext;
            viewContext.RequestContext = new RequestContext(httpContext, new RouteData());

            var html = new HtmlHelper(viewContext, new ViewPage());
            Assert.AreEqual(@"<script src=""http://localhost/TestComet/Scripts/Cometd.js"" type=""text/javascript""></script>" +
                            @"<script src=""http://localhost/TestComet/Scripts/OtherScript.js"" type=""text/javascript""></script>",
                            html.IncludeScript("Cometd.js", "OtherScript.js"));
        }

        [TestMethod]
        public void IncludePageScriptReturnsIncludeForPageScript()
        {
            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            var viewContext = MockRepository.GenerateStub<ViewContext>();
            var httpRequest = MockRepository.GenerateStub<HttpRequestBase>();

            httpContext.Stub(c => c.Request).Return(httpRequest).Repeat.Any();
            httpRequest.Stub(c => c.ApplicationPath).Return("http://localhost/TestComet/");

            var view = new WebFormView("~/Views/Home/Index.aspx");

            viewContext.View = view;
            viewContext.HttpContext = httpContext;
            viewContext.RequestContext = new RequestContext(httpContext, new RouteData());

            var html = new HtmlHelper(viewContext, new ViewPage());
            Assert.AreEqual(@"<script src=""http://localhost/TestComet/Scripts/Views/Home/Index.js"" type=""text/javascript""></script>",
                            html.IncludePageScript() 
                          );
        }

    }
} 
