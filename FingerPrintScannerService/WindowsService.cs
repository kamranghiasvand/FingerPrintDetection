using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrintScannerService
{
    public class WindowsService : ServiceBase
    {
        private readonly System.ComponentModel.IContainer components;
        public WindowsService()
        {
            components = new System.ComponentModel.Container();
            ServiceName = "FingerPrint Detector Service";
        }

        protected override void OnStart(string[] args)
        {
         
        }

        protected override void OnStop()
        {
         
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
