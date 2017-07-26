using System;
using System.Diagnostics;
using System.Reflection;
using Common.Logging;
using log4net.Config;

namespace ScannerDriver
{
    class Program
    {
        static Program()
        {
            XmlConfigurator.Configure();
            Log = LogManager.GetLogger(typeof(Program));
        }
        public static ILog Log { get; set; }
        public static void Main()
        {
            try
            {
                Console.WriteLine("starting...");
                Log.Debug("Starting...");
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                {
                    Console.WriteLine("Another instance is running...");
                    Log.Debug("Another instance is running...");
                    Console.WriteLine("exiting...");
                    Log.Debug("exiting...");
                    return;

                }

                var cmd = CommandRunner.Create();
                cmd.Start();
                var dm = DriverManager.Create();
                var isRunning = true;
                while (isRunning)
                {
                    var c = Console.ReadLine()?.ToLower();
                    Log.Debug("read command from console: " + c);
                    var error = "";
                    switch (c)
                    {
                        case "start":
                            try
                            {
                                Log.Debug("Command is 'Start'");
                                dm?.Start(out error);
                                Console.WriteLine(string.IsNullOrEmpty(error) ? "Start Success" : error);
                                if (string.IsNullOrEmpty(error)) Log.Debug("Start Success");
                                else
                                    Log.Error("Start Failed: " + error);

                            }
                            catch(Exception ex)
                            {
                                Log.Error(ex);
                            }
                            break;
                        case "stop":
                            try
                            {
                                Log.Debug("Command is 'Stop'");
                                dm?.Stop(out error);
                                Console.WriteLine(string.IsNullOrEmpty(error) ? "Stop Success" : error);
                                if (string.IsNullOrEmpty(error)) Log.Debug("Stop Success");
                                else
                                    Log.Error("Stop Failed: " + error);
                            }
                            catch(Exception ex)
                            {
                                Log.Error(ex);
                            }
                            break;
                        case "capturesingleimage":
                            try
                            {
                                Log.Debug("Command is 'CaptureSingleImage");
                                dm?.CaptureSingleImage(dm.GetFirstScanner()?.Id, out error);
                                Console.WriteLine(string.IsNullOrEmpty(error) ? "Captured Success" : error);
                                if (string.IsNullOrEmpty(error)) Log.Debug("Captured Success");
                                else
                                    Log.Error("Captured Failed: " + error);
                            }
                            catch(Exception ex)
                            {
                                Log.Error(ex);
                            }
                            break;
                        case "getscannerstate":
                            try
                            {
                                Log.Debug("Command is 'GetScannerState'");
                                if (dm != null)
                                {
                                    var stat = dm.GetScannersState();
                                    Console.WriteLine("Id\t\tImageQuality\tIsCapturing\tIsSensorOn\tTimeout");
                                    Log.Debug("Id\t\tImageQuality\tIsCapturing\tIsSensorOn\tTimeout");
                                    foreach (var s in stat)
                                    {
                                        Console.WriteLine(
                                            $"{s.Id}\t\t{s.ImageQuality}\t{s.IsCapturing}\t{s.IsSensorOn}\t{s.Timeout}");
                                        Log.Debug($"{s.Id}\t\t{s.ImageQuality}\t{s.IsCapturing}\t{s.IsSensorOn}\t{s.Timeout}");

                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                Log.Error(ex);
                            }
                            break;

                        case "exit":
                            Log.Debug("Command is 'exit'");
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
            catch (Exception ex)
            {
                Log.Error(ex);
                Console.WriteLine(ex);
            }
        }
    }
}
