using System;
using System.Diagnostics;
using System.Reflection;

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
                    var error = "";
                    switch (c)
                    {
                        case "start":
                            try
                            {
                                dm?.Start(out error);
                                Console.WriteLine(string.IsNullOrEmpty(error) ? "Start Success" : error);
                            }
                            catch
                            {
                                // ignored
                            }
                            break;
                        case "stop":
                            try
                            {
                                dm?.Stop(out error);
                                Console.WriteLine(string.IsNullOrEmpty(error) ? "Stop Success" : error);
                            }
                            catch
                            {
                                // ignored
                            }
                            break;
                        case "capturesingleimage":
                            try
                            {
                                dm?.CaptureSingleImage(dm.GetFirstScanner()?.Id, out error);
                                Console.WriteLine(string.IsNullOrEmpty(error) ? "Captured Success" : error);
                            }
                            catch
                            {
                                // ignored
                            }
                            break;
                        case "getscannerstate":
                            try {
                                if (dm != null)
                                {
                                    var stat = dm.GetScannersState();
                                    Console.WriteLine("Id\t\tImageQuality\tIsCapturing\tIsSensorOn\tTimeout");
                                    foreach (var s in stat)
                                    {
                                        Console.WriteLine(
                                            $"{s.Id}\t\t{s.ImageQuality}\t{s.IsCapturing}\t{s.IsSensorOn}\t{s.Timeout}");

                                    }
                                }
                            }
                            catch
                            {
                                // ignored
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
                // ignored
            }
        }
    }
}
