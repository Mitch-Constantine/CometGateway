using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CometGateway.Server.TelnetDemo.Controllers
{
    [HandleError]
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
