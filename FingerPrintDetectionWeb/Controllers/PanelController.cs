﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FingerPrintDetectionModel;
using FingerPrintDetectionWeb.Models;
using FingerPrintDetectionWeb.Resources;
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


        #region User Management

        #region Logical User
        public ActionResult LogicalUserList()
        {
            return View();
        }
        public ActionResult AddLogicalUser()
        {
            return View();
        }
        public async Task<JsonResult> GetLogicalUsersListAsync(DatatablesParam paramView)
        {

            var data = new List<object>();
            double recordsTotal = 0, recordsFiltered = 0;

            await Task.Run(() =>
            {
                if (paramView == null)
                    paramView = new DatatablesParam();
                recordsTotal = DbContext.LogicalUsers.Count(m => !m.Deleted);
                var users = string.IsNullOrEmpty(paramView.customSearch) ?
                   DbContext.LogicalUsers.Where(m => !m.Deleted).OrderBy(m => m.Id) :
                   DbContext.LogicalUsers.Where(m => !m.Deleted && (m.FirstName.Contains(paramView.customSearch) || m.LastName.Contains(paramView.customSearch))).OrderBy(m => m.Id);
                recordsFiltered = users.Count();

                foreach (var user in users.Skip(paramView.start).Take(paramView.length))
                {
                    var row = new Dictionary<string, object>
                     {
                         {"Id", user.Id},
                         {"FirstName", user.FirstName},
                         {"LastName", user.LastName}
                     };
                    data.Add(row);
                }
            });
            return Json(new { paramView.draw, recordsTotal, recordsFiltered, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddLogicalUser(LogicalUserViewModel model, HttpPostedFileBase soundFile)
        {
            try
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
                        var res =
                            new
                            {
                                status = "fail",
                                errors = new List<string> { Global.PanelController_AddUser_SoundTrackNotSelected }
                            };
                        return Json(res);
                    }
                    var fileName = Path.GetFileName(soundFile.FileName);
                    if (fileName == null)
                    {
                        var res =
                            new
                            {
                                status = "fail",
                                errors = new List<string> { Global.PanelController_AddUser_SoundTrackNotSelected }
                            };
                        return Json(res);
                    }
                    var path = Server.MapPath("~/App_Data/uploads/");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path = Path.Combine(path, fileName);
                    try
                    {
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        soundFile.SaveAs(path);
                    }
                    catch (Exception)
                    {
                        var res =
                            new
                            {
                                status = "fail",
                                errors = new List<string> { Global.PanelController_AddUser_FailToSaveSoundTrack }
                            };
                        return Json(res);
                    }

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
                    path = (new Uri(path).ToString());
                    model.SoundTrackId = DbContext.SoundTracks.First(m => m.Uri == path).Id;
                }
                var plan = DbContext.Plans.First(m => m.Id == model.PlanId);
                var soundTrack = DbContext.SoundTracks.First(m => m.Id == model.SoundTrackId);
                var user = new LogicalUser
                {
                    Plan = plan,
                    Sound = soundTrack,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                DbContext.LogicalUsers.Add(user);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                var res = new { status = "fail", errors = new List<string> { "خطایی در سرور رخ داده است" } };
                return Json(res);
            }
            return Json(new { status = "success", address = Url.Action("LogicalUserList", "Panel") });
        }
        #endregion

        #region Real User
        public ActionResult RealUserList()
        {
            return View();
        }

        public ActionResult AddRealUser()
        {
            return View();

        }

        public async Task<JsonResult> GetRealUsersListAsync(DatatablesParam paramView)
        {
            var data = new List<object>();
            double recordsTotal = 0, recordsFiltered = 0;

            await Task.Run(() =>
            {
                if (paramView == null)
                    paramView = new DatatablesParam();
                recordsTotal = DbContext.RealUsers.Count(m => !m.Deleted);
                var users = string.IsNullOrEmpty(paramView.customSearch) ?
                   DbContext.RealUsers.Where(m => !m.Deleted).OrderBy(m => m.Id) :
                   DbContext.RealUsers.Where(m => !m.Deleted && (m.FirstName.Contains(paramView.customSearch) || m.LastName.Contains(paramView.customSearch))).OrderBy(m => m.Id);
                recordsFiltered = users.Count();

                foreach (var user in users.Skip(paramView.start).Take(paramView.length))
                {
                    var fingers = new bool[3];
                    fingers[0] = user.FirstFinger != null && user.FirstFinger.Length > 0;
                    fingers[1] = user.SecondFinger != null && user.SecondFinger.Length > 0;
                    fingers[2] = user.ThirdFinger != null && user.ThirdFinger.Length > 0;
                    var row = new Dictionary<string, object>
                     {
                         {"Id", user.Id},
                         {"FirstName", user.FirstName},
                         {"LastName", user.LastName},
                        {"Fingers",fingers }
                     };
                    data.Add(row);
                }
            });
            return Json(new { paramView.draw, recordsTotal, recordsFiltered, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> AddRealUser(RealUserViewModel model)
        {
            return await Task.Run<JsonResult>(() =>
            {
                try
                {
                    if (!model.IsValid())
                    {
                        var res = new { status = "fail", errors = new List<string>() };
                        foreach (var err in model.Errors.Take(3))
                            res.errors.Add(err);
                        return Json(res);
                    }
                    var logicaluser = DbContext.LogicalUsers.FirstOrDefault(m => m.Id == model.LogicalUserId);
                    if (logicaluser == null)
                    {
                        var res = new { status = "fail", errors = new [] { "کابر منطقی را انتحاب کنید" } };
                        return Json(res);
                    }
                    var realUser = new RealUser
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        LogicalUser = logicaluser,
                        Birthday = ConvertPersianToEnglish(model.Birthday)
                    };
                    DbContext.RealUsers.Add(realUser);
                    DbContext.SaveChanges();
                }
                catch (Exception)
                {
                    var res = new { status = "fail", errors = new [] { "خطایی در سرور رخ داده است" } };
                    return Json(res);
                }
                return Json(new { status = "success", address = Url.Action("LogicalUserList", "Panel") });
            });
        }

        [HttpPost]
        public async Task<JsonResult> AddFinger(int witchFinger, long userId)
        {
            try
            {
                if (!(new[] {0,1, 2}.Contains(witchFinger)))
                    return Json(new {status = "fail", errors = new[] {"انگشت مورد نظر را انتخاب کنید"}});
                var user = DbContext.RealUsers.FirstOrDefault(m => m.Id == userId);
                if (user == null)
                    return Json(new {status = "fail", errors = new[] {"کاربر مورد نظر را انتخاب کنید"}});
                var status = await FingerPrintManager.GetScannersState();
                if(status.Count==0)
                    return Json(new { status = "fail", errors = new[] { "اسکنری پیدا نشد" } });
                var error = "";
                var template = await FingerPrintManager.CaptureSingleTemplate(status.First().Id, error);
                if (template == null || template.Length == 0)
                    return Json(new {status = "fail", errors = new[] {"انگشت کاربر تشخیص داده نشد"}});
                if(witchFinger==0)
                user.FirstFinger = template;
                if (witchFinger == 1)
                    user.SecondFinger = template;
                if (witchFinger == 2)
                    user.ThirdFinger = template;
                DbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                DbContext.SaveChanges();
                return Json(new {status = "success"});
            }
            catch
            {
                var res = new { status = "fail", errors = new []{ "خطایی در سرور رخ داده است" } };
                return Json(res);
            }


        }

        [HttpPost]
        public JsonResult DeleteRealUser(long id)
        {
            return null;

        }
        #endregion
        #endregion

        #region Plan Management
        public ActionResult PlanList()
        {
            return View();
        }
        public ActionResult AddPlan()
        {
            return View();
        }
        public async Task<JsonResult> GetPlansListAsync(DatatablesParam paramView)
        {

            var data = new List<object>();
            double recordsTotal = 0, recordsFiltered = 0;

            await Task.Run(() =>
            {
                if (paramView == null)
                    paramView = new DatatablesParam();
                recordsTotal = DbContext.Plans.Count(m => !m.Deleted);
                var plans = string.IsNullOrEmpty(paramView.customSearch) ?
                   DbContext.Plans.Where(m => !m.Deleted).OrderBy(m => m.Id) :
                   DbContext.Plans.Where(m => !m.Deleted && (m.Name.Contains(paramView.customSearch))).OrderBy(m => m.Id);
                recordsFiltered = plans.Count();

                foreach (var plan in plans.Skip(paramView.start).Take(paramView.length))
                {
                    var row = new Dictionary<string, object>
                    {
                        {"Id", plan.Id},
                        {"Name", plan.Name},
                        {"RepeatNumber", plan.RepeatNumber},
                        {"MaxNumberOfUse", plan.MaxNumberOfUse},
                        {"Description", plan.Description},
                        {"Users", new List<object>()}
                    };
                    if (plan.Users != null)
                        foreach (var user in plan.Users)
                        {
                            ((List<object>)row["Users"]).Add(new { user.FirstName, user.LastName });
                        }
                    data.Add(row);
                }
            });
            return Json(new { paramView.draw, recordsTotal, recordsFiltered, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddPlan(PlanViewModel model)
        {
            try
            {
                if (!model.IsValid())
                {
                    var res = new { status = "fail", errors = new List<string>() };
                    foreach (var err in model.Errors.Take(3))
                        res.errors.Add(err);
                    return Json(res);
                }
                var plan = new Plan
                {
                    Description = model.Description,
                    MaxNumberOfUse = model.MaxUserCount,
                    Name = model.Name,
                    RepeatNumber = model.RepeatNumber,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime
                };
                DbContext.Plans.Add(plan);
                DbContext.SaveChanges();

                return Json(new { status = "success", address = Url.Action("PlanList", "Panel") });
            }
            catch (Exception)
            {
                var res = new { status = "fail", errors = new List<string> { "خطایی در سرور رخ داده است" } };
                return Json(res);
            }
        }


        #endregion

        #region Scanner Manager

        public ActionResult ScannerManager()
        {
            return View();
        }

        public async Task<JsonResult> GetScannersListAsync(DatatablesParam paramView)
        {
            try
            {
                var data = new List<object>();
                var total = await FingerPrintManager.GetScannersState();
                if (paramView == null)
                    paramView = new DatatablesParam();

                double recordsTotal = total.Count;
                var filtered = string.IsNullOrEmpty(paramView.customSearch)
                    ? total.OrderBy(m => m.Id)
                    : total.Where(m => (m.Id.Contains(paramView.customSearch))).OrderBy(m => m.Id);
                double recordsFiltered = filtered.Count();

                data.AddRange(
                    filtered.Skip(paramView.start)
                        .Take(paramView.length)
                        .Select(state => new Dictionary<string, object>
                        {
                                {"Id", state.Id},
                                {"ImageQuality", state.ImageQuality},
                                {"IsCapturing", state.IsCapturing},
                                {"IsSensorOn", state.IsSensorOn},
                                {"Timeout", state.Timeout}
                        }));

                return Json(new { paramView.draw, recordsTotal, recordsFiltered, data }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var res = new { status = "fail", errors = new List<string> { "خطایی در سرور رخ داده است" } };
                return Json(res);
            }
        }

        private JsonResult StopScannerManager()
        {
            try
            {
                FingerPrintManager.Stop();
                return Json(new { status = "success", address = Url.Action("ScannerManager", "Panel"), state = FingerPrintManager.IsRunning });
            }
            catch
            {
                var res = new { status = "fail", errors = new List<string> { "خطایی در سرور رخ داده است" }, state = FingerPrintManager.IsRunning };
                return Json(res);
            }
        }
        private JsonResult StartScannerManager()
        {
            try
            {
                FingerPrintManager.Start();
                return Json(new { status = "success", address = Url.Action("ScannerManager", "Panel"), state = FingerPrintManager.IsRunning });
            }
            catch
            {
                var res = new { status = "fail", errors = new List<string> { "خطایی در سرور رخ داده است" }, state = FingerPrintManager.IsRunning };
                return Json(res);
            }
        }

        [HttpPost]
        public JsonResult ToggleScannerManager()
        {
            return FingerPrintManager.IsRunning ? StopScannerManager() : StartScannerManager();
        }

        #endregion
        public static DateTime ConvertPersianToEnglish(string persianDate)
        {
            string[] formats = { "yyyy/MM/dd", "yyyy/M/d", "yyyy/MM/d", "yyyy/M/dd" };
            var d1 = DateTime.ParseExact(persianDate, formats,
                                              CultureInfo.CurrentCulture, DateTimeStyles.None);
            var date = new PersianCalendar();
            var dt = date.ToDateTime(d1.Year, d1.Month, d1.Day, 0, 0, 0, 0, 0);
            return dt;
        }
    }

}