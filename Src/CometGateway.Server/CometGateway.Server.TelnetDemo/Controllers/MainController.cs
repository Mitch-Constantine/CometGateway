using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CometGateway.Server.TelnetDemo.Models;

namespace CometGateway.Server.TelnetDemo.Controllers
{
    [HandleError]
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            ViewData[TestModeConstants.ENABLE_TEST_MODE] = true;
            return View("Index");
        }
    }
}
