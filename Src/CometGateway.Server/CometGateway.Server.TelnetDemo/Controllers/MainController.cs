using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CometGateway.Server.TelnetDemo.Models;
using log4net;
using CometGateway.Server.TelnetDemo.Configuration;

namespace CometGateway.Server.TelnetDemo.Controllers
{
    [HandleError]
    public class MainController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainController).Name);

        protected override void HandleUnknownAction(string actionName)
        {
            ActionResult actionResult = GetNamedServerActionResult(actionName);
            if (actionResult != null)
            {
                actionResult.ExecuteResult(ControllerContext);
            }
            else
                base.HandleUnknownAction(actionName);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test() 
        {
            ViewData[TestModeConstants.ENABLE_TEST_MODE] = true;
            return View("Index");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            log.Error("Unexpected exception", filterContext.Exception);

            base.OnException(filterContext);
        }

        public virtual ITelnetServerConfigurationParser GetConfigurationParser()
        {
            return new TelnetServerConfigurationParser();
        }

        internal ActionResult GetNamedServerActionResult(string actionName)
        {
            TelnetServerConfiguration[] allConfigs = GetConfigurationParser().Parse();
            TelnetServerConfiguration configuration = 
                allConfigs
                    .FirstOrDefault(
                            config => config.Name.ToLower() == actionName.ToLower()
                         );

            if (configuration != null)
            {
                ViewData["server"] = configuration.Server;
                ViewData["port"] = configuration.Port;
                return View("Index");
            }
            else
            {
                return null;
            }
        }
    }
}
