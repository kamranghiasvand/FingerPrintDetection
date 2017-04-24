﻿using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Mvc;
using FingerPrintDetectionModel;
using FingerPrintDetectionWeb.Manager;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace FingerPrintDetectionWeb.Controllers
{
    public class BaseController : Controller
    {
        public ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        public ScannerManager FingerPrintManager => ScannerManager.Create();
        public ApplicationDbContext DbContext => HttpContext.GetOwinContext().Get<ApplicationDbContext>();
        public LoginUser GetCurrentUser() => UserManager.FindById(User.Identity.GetUserId<long>());


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public BaseController()
        {
            if (Process.GetProcessesByName("ScannerDriver").Length == 0 && Process.GetProcessesByName("ScannerDriver.vshost").Length == 0)
            {
                string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~/bin"), "ScannerDriver.exe");

                var info = new ProcessStartInfo
                {
                    CreateNoWindow=false,
                    WindowStyle=ProcessWindowStyle.Normal,
                    FileName = path

                };
                Process.Start(info);
            }
        }
    }
}
