using FingerPrintDetectionWeb.Manager;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FingerPrintDetectionWeb.Startup))]
namespace FingerPrintDetectionWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            var scanner=ScannerManager.Create();
            scanner.Start();
            

        }
    }
}
