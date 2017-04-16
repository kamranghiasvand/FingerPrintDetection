using Microsoft.Owin;
using Owin;
using ScannerDriver;

[assembly: OwinStartupAttribute(typeof(FingerPrintDetectionWeb.Startup))]
namespace FingerPrintDetectionWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.CreatePerOwinContext(DriverManager.Create);
        }
    }
}
