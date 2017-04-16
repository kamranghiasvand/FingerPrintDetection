using System.ServiceProcess;

namespace FingerPrintScannerService
{
    static class Program
    {
        static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new WindowsService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
