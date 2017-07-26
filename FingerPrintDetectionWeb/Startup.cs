using FingerPrintDetectionWeb.Manager;
using log4net.Config;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FingerPrintDetectionWeb.Startup))]
[assembly: XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace FingerPrintDetectionWeb
{
    public partial class Startup
    {
      
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
           // app.CreatePerOwinContext(ScannerManagerConnector.Create);
        }
        
    }
}
