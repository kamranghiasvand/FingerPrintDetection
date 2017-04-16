using System.Web;
using System.Web.Mvc;
using FingerPrintDetectionModel;
using FingerPrintDetectionWeb.Manager;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ScannerDriver;

namespace FingerPrintDetectionWeb.Controllers
{
    public class BaseController : Controller
    {
        public ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        public IScannerManager FingerPrintManager => HttpContext.GetOwinContext().GetUserManager<DriverManager>();
        public ApplicationDbContext DbContext => HttpContext.GetOwinContext().Get<ApplicationDbContext>();
        public LoginUser GetCurrentUser() => UserManager.FindById(User.Identity.GetUserId<long>());

       
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
