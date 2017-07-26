using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Common.Logging;
using FingerPrintDetectionModel;
using FingerPrintDetectionWeb.Manager;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace FingerPrintDetectionWeb.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        protected ScannerManagerConnector ManagerConnector => ScannerManagerConnector.Create();
        protected ApplicationDbContext DbContext => HttpContext.GetOwinContext().Get<ApplicationDbContext>();
        public LoginUser GetCurrentUser() => UserManager.FindById(User.Identity.GetUserId<long>());

        protected readonly ILog Log = LogManager.GetLogger(typeof(BaseController));


        public BaseController()
        {
            try
            {
                if (Process.GetProcessesByName("ScannerDriver").Length != 0 ||
                    Process.GetProcessesByName("ScannerDriver.vshost").Length != 0) return;
                Log.Debug("Starting Scanner Driver");
                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~/bin"), "ScannerDriver.exe");

                var info = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = path

                };
                Process.Start(info);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
