using System;
using System.Threading;
using System.Web.Hosting;
using FingerPrintDetectionWeb.Manager;
using Microsoft.Owin;
using Microsoft.Owin.BuilderProperties;
using Owin;

[assembly: OwinStartupAttribute(typeof(FingerPrintDetectionWeb.Startup))]
namespace FingerPrintDetectionWeb
{
    public partial class Startup
    {
      
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.CreatePerOwinContext(ScannerManagerConnector.Create);
        }
        
    }
}
