using System;
using System.Threading;
using FingerPrintDetectionWeb.Manager;
using Microsoft.Owin;
using Microsoft.Owin.BuilderProperties;
using Owin;

[assembly: OwinStartupAttribute(typeof(FingerPrintDetectionWeb.Startup))]
namespace FingerPrintDetectionWeb
{
    public partial class Startup
    {
        private ScannerManager scanner;
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            scanner=ScannerManager.Create();
            scanner.Start();
            app.OnDisposing(() => { scanner.Dispose(); });

        }
        
    }
    static class AppBuilderExtensions
    {
        public static void OnDisposing(this IAppBuilder app, Action cleanup)
        {
            var properties = new AppProperties(app.Properties);
            var token = properties.OnAppDisposing;
            if (token != CancellationToken.None)
            {
                token.Register(cleanup);
            }
        }
    }
}
