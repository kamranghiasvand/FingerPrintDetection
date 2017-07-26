using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FingerPrintDetectionWeb.Models;

namespace FingerPrintDetectionWeb.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        public async Task<JsonResult> GetLogListAsync(DatatablesParam paramView)
        {

            var data = new List<object>();
            double recordsTotal = 0, recordsFiltered = 0;

            await Task.Run(() =>
            {
                try { 
                if (paramView == null)
                    paramView = new DatatablesParam();
                recordsTotal = DbContext.Logs.Count(m => !m.Deleted);
                var dt = DateTime.Now.Date;
                try
                {
                    if (!string.IsNullOrEmpty(paramView.customSearch))
                    {
                        dt = DateTime.Parse(paramView.customSearch);
                    }
                }
                catch
                {
                    dt = DateTime.Now.Date;
                }

                var logs =
                         DbContext.Logs.Where(m => !m.Deleted && DbFunctions.DiffDays(m.Time, dt) == 0).OrderBy(m => m.Id);
                recordsFiltered = logs.Count();

                foreach (var log in logs.Skip(paramView.start).Take(paramView.length))
                {
                    var row = new Dictionary<string, object>
                    {
                        {"Id", log.Id},
                        {"Income", log.Income},
                        {"Time",(PanelController.ConvertGregorianToPersian(log.Time.ToString(CultureInfo.InvariantCulture),"hh:mm:ss tt"))}
                    };

                    var logicaluser = DbContext.LogicalUsers.FirstOrDefault(m => !m.Deleted && m.Id == log.LogicalUserId);
                    row.Add("LogicalUser", logicaluser != null ? logicaluser.FirstName + " " + logicaluser.LastName : "");

                    var realuser = DbContext.RealUsers.FirstOrDefault(m => !m.Deleted && m.Id == log.RealUserId);
                    row.Add("RealUser", realuser != null ? realuser.FirstName + " " + realuser.LastName : "");

                    var plan = DbContext.Plans.FirstOrDefault(m => !m.Deleted && m.Id == log.PlanId);
                    row.Add("Plan", plan != null ? plan.Name : "");

                    data.Add(row);
                }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });
            return Json(new { paramView.draw, recordsTotal, recordsFiltered, data }, JsonRequestBehavior.AllowGet);
        }
    }
}