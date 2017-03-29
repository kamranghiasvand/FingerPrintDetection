using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FingerPrintDetectionWeb.Controllers
{
    [Authorize(Roles ="Admin")]
    public class PanelController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserList()
        {
            return View();
        }

        public ActionResult AddUser()
        {
            return View();
        }
    }
}