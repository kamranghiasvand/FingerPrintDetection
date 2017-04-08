using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FingerPrintDetectionModel;
using FingerPrintDetectionWeb.Models;
using NAudio.Wave;

namespace FingerPrintDetectionWeb.Controllers
{
    [Authorize(Roles = "Admin")]
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
        #region User Management
        public ActionResult AddUser()
        {
            return View();
        }
        public async Task<JsonResult> GetUsersListAsync(DatatablesParam paramView)
        {

            var data = new List<object>();
            double recordsTotal = 0, recordsFiltered = 0;

            await Task.Run(() =>
            {
                if (paramView == null)
                    paramView = new DatatablesParam();
                recordsTotal = DbContext.Users.Count(m => !m.Deleted);
                var users = string.IsNullOrEmpty(paramView.customSearch) ?
                   DbContext.Users.Where(m => !m.Deleted).OrderBy(m => m.Id) :
                   DbContext.Users.Where(m => !m.Deleted && (m.FirstName.Contains(paramView.customSearch) || m.LastName.Contains(paramView.customSearch))).OrderBy(m => m.Id);
                recordsFiltered = users.Count();
                foreach (var user in users.Skip(paramView.start).Take(paramView.length))
                {
                    var row = new Dictionary<string, object>
                     {
                         {"Id", user.Id},
                         {"FirstName", user.FirstName},
                         {"LastName", user.LastName},
                         {"Email", user.Email},
                         {"UserName", user.UserName},
                     };
                    data.Add(row);
                }
            });
            return Json(new { paramView.draw, recordsTotal, recordsFiltered, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> AddUser(LogicalUserViewModel model, HttpPostedFileBase soundFile)
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
                if (soundFile == null)
                {
                    var res = new { status = "fail", errors = new List<string> { Resources.Global.PanelController_AddUser_SoundTrackNotSelected } };
                    return Json(res);
                }
                var fileName = Path.GetFileName(soundFile.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                soundFile.SaveAs(path);

                var reader = new Mp3FileReader(path);
                var duration = reader.TotalTime;
                DbContext.SoundTracks.Add(new SoundTrack
                {
                    Duration = duration.Ticks,
                    Name = fileName,
                    Type = SoundTrackType.Mp3,
                    Uri = new Uri(path).ToString()
                });
                DbContext.SaveChanges();
                model.SoundTrackId = DbContext.SoundTracks.First(m => m.Uri == (new Uri(path).ToString())).Id;
            }
            var plan = DbContext.Plans.First(m => m.Id == model.PlanId);
            var soundTrack = DbContext.SoundTracks.First(m => m.Id == model.SoundTrackId);
            var user = new LogicalUser
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Plan = plan,
                Sound = soundTrack,
                UserName = model.UserName
            };
            DbContext.Users.Add(user);
            DbContext.SaveChanges();
            await UserManager.AddToRoleAsync(user.Id, "User");
            return Json( new { status = "success", address=Url.Action("UserList","Panel") }) ;
        }
        #endregion

    }
}