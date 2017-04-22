using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScannerDriver
{
    class Program
    {
        public static void Main()
        {
            try
            {
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                    return;
                var cmd = CommandRunner.Create();
                cmd.Start(); 
                var dm=DriverManager.Create(cmd);
                var isRunning = true;
                while (isRunning)
                {
                    var c=Console.ReadLine()?.ToLower();
                    string error;
                    switch (c)
                    {
                        case "start":
                            dm.Start(out error);
                            Console.WriteLine(string.IsNullOrEmpty(error) ? "Start Success" : error);
                            break;
                        case "stop":
                            dm.Stop(out error);
                            Console.WriteLine(string.IsNullOrEmpty(error) ? "Stop Success" : error);
                            break;
                        case "capturesingleimage":
                            var tem= dm.CaptureSingleImage(dm.GetFirstScanner().Id, out error);
                            Console.WriteLine(string.IsNullOrEmpty(error) ? "Captured Success" : error);
                            break;
                        case "getscannerstate":
                            var stat = dm.GetScannersState();
                            Console.WriteLine("Id\t\tImageQuality\tIsCapturing\tIsSensorOn\tTimeout");
                            foreach (var s in stat)
                            {
                                Console.WriteLine(
                                    $"{s.Id}\t\t{s.ImageQuality}\t{s.IsCapturing}\t{s.IsSensorOn}\t{s.Timeout}");

                            }
                            break;
                           
                        case "exit":
                            isRunning = false;
                            break;
                        default:
                            Console.WriteLine("-------------------Scanner Driver Version(" +
                                              Assembly.GetExecutingAssembly().ImageRuntimeVersion +
                                              ")-------------------");
                            Console.WriteLine(
                                "Command Names: 'start' , 'stop' , 'capturesingleimage' , 'getscannerstate' , 'exit'");
                            break;
                    }
                }
                    
                cmd.Stop();
            }
            catch
            {
            }
        }
    }
}
