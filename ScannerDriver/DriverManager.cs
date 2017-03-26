using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Suprema;

namespace ScannerDriver
{
    public class DriverManager
    {

        public static void Main()
        {
            var driver = new Driver();
            driver.Init();
            while (true)
            {
                var cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "scan":
                        driver.Scan();
                        break;
                    case "exit":
                        return;
                    case "enroll":
                        driver.ExtractTemplate();
                        break;

                }
            }
        }
    }

    public class Driver : Control
    {
        const int MAX_TEMPLATE_SIZE = 1024;
        int m_quality = 40;

        UFS_STATUS status;
        UFScannerManager scannerManager;
        int nScannerNumber;
        
        private UFMatcher matcher;

        public void Init()
        {
            CreateHandle();
            scannerManager = new UFScannerManager(this);
            status = scannerManager.Init();
            if (status != UFS_STATUS.OK)
            {
                string error;
                UFScanner.GetErrorString(status, out error);
                throw new Exception(error);
            }
            if (scannerManager.Scanners.Count > 0)
            {
                scanner = scannerManager.Scanners[0];
                scanner.Timeout = 5000;
            }
            scannerManager.ScannerEvent += ScannerEvent;
            matcher = new UFMatcher();
            if (matcher.InitResult != UFM_STATUS.OK)
            {
                string error;
                UFMatcher.GetErrorString(matcher.InitResult, out error);
                throw new Exception(error);
            }
        }

        public void Scan()
        {
            if (scanner == null)
                return;

            status = scanner.ClearCaptureImageBuffer();
            if (status != UFS_STATUS.OK)
            {
                string error;
                UFScanner.GetErrorString(status, out error);
                Console.WriteLine(error);
                return;
            }
            scanner.CaptureEvent += Scanner_CaptureEvent;
            status = scanner.StartCapturing();

            if (status != UFS_STATUS.OK)
            {
                string error;
                UFScanner.GetErrorString(status, out error);
                Console.WriteLine(error);
                return;
            }

        }

        private int Scanner_CaptureEvent(object sender, UFScannerCaptureEventArgs e)
        {
            var template = new byte[MAX_TEMPLATE_SIZE];
            int templateSize;
            int enrollQuality;
            status = ((UFScanner)sender).ExtractEx(MAX_TEMPLATE_SIZE, template, out templateSize, out enrollQuality);
            if (status != UFS_STATUS.OK)
            {
                string error;
                UFScanner.GetErrorString(status, out error);
                Console.WriteLine(error);
                return 1;
            }
            Console.WriteLine("Template:");
            for (var i = 0; i < templateSize; ++i)
                Console.Write(template[i] + " ");
            return 1;
        }

        public void ScannerEvent(object sender, UFScannerManagerScannerEventArgs e)
        {

            if (scannerManager.Scanners.Count > 0)
            {
                scanner = scannerManager.Scanners[0];
                scanner.Timeout = 5000;
            }
            else
                scanner = null;

        }

        public  bool ExtractTemplate(out byte[] template, out string error)
        {
            template = new byte[MAX_TEMPLATE_SIZE];
            try
            {
                scanner.ClearCaptureImageBuffer();
                while (true)
                {
                    status = scanner.CaptureSingleImage();
                    if (status != UFS_STATUS.OK)
                    {
                        UFScanner.GetErrorString(status, out error);
                        return false;
                    }

                    int size;
                    int enrollQuality;
                    status = scanner.ExtractEx(MAX_TEMPLATE_SIZE, template, out size, out enrollQuality);

                    if (status == UFS_STATUS.OK)
                    {
                        if (enrollQuality < m_quality)
                        {
                            continue;
                        }
                        else
                        {
                            return template;
                        }
                    }
                    else
                    {
                        UFScanner.GetErrorString(status, out error);
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
        }
    }
}
