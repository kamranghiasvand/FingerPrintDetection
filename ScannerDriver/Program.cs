using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScannerDriver
{
    class Program
    {
        public static void Main()
        {
            string error = "";
            byte[] template;
            var manager = DriverManager.GetManager();
            manager.Start();
            var scanner = manager.GetFirstScanner();
            scanner.CaptureEvent += Scanner_CaptureEvent;
            while (true)
            {
                var cmd = Console.ReadLine();
                switch (cmd.ToLower().Trim())
                {
                    case "scan":
                        template=scanner.CaptureSingleTemplate(out error);
                        if (string.IsNullOrEmpty(error))
                            Console.WriteLine("Template is Captured");
                        else
                            Console.WriteLine(error);
                        break;
                    case "exit":
                        manager.Dispose();
                        return;
                    case "startenroll":
                        if (!scanner.StartCapturing(out error))
                            Console.WriteLine(error);
                        else
                            Console.WriteLine("Start Enrolling");
                        break;
                    case "stopenroll":
                        if (!scanner.StopCapturing(out error))
                            Console.WriteLine(error);
                        else
                            Console.WriteLine("Stop Enrolling");
                        break;

                }
            }
        }

        private static void Scanner_CaptureEvent(ScannerWrapper sender, byte[] template, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return;
            }
            for (var i = 0; i < template.Length; ++i)
                Console.Write(i + " ");
        }
    }
}
