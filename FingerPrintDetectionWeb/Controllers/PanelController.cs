using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FingerPrintDetectionModel;
using FingerPrintDetectionWeb.Models;

namespace FingerPrintDetectionWeb.Controllers
{
    [Authorize(Roles ="Admin")]
    public class PanelController : BaseController
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

        [HttpPost]
        public JsonResult AddUser(LogicalUserViewModel model)
        {
            if (!model.IsValid())
            {
                var res = new { status = "fail", errors = new List<string>() };
                foreach (var err in model.Errors.Take(3))
                    res.errors.Add(err);
                return Json(res);
            }
            if (model.SoundTrackId < 0)
            {
                
            }
            var user = new LogicalUser {Email = model.Email,FirstName=model.FirstName,LastName=model.LastName,};
            return null;
        }
    }
}