using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CometGateway.Server.TelnetDemo.Models;
using log4net;

namespace CometGateway.Server.TelnetDemo.Controllers
{
    [HandleError]
    public class MainController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainController).Name);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            ViewData[TestModeConstants.ENABLE_TEST_MODE] = true;
            return View("Index");
        }

        public ActionResult Aardwolf()
        {
            ViewData["server"] = "aardwolf.org";
            ViewData["port"] = "4000";
            return View("Index");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            log.Error("Unexpected exception", filterContext.Exception);

            base.OnException(filterContext);
        }
    }
}
