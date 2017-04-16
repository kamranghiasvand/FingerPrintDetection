using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Suprema;

namespace ScannerDriver
{
    public class DriverManager : Control, IScannerManager
    {
        private readonly UFScannerManager manager;
        private static DriverManager instance;
        public Dictionary<string, ScannerWrapper> Scanners { get; } = new Dictionary<string, ScannerWrapper>();

        public bool IsRunning { get; private set; }

        private DriverManager()
        {
            manager = new UFScannerManager(this);
            manager.ScannerEvent += Manager_ScannerEvent;
            manager.Init();
        }

        public static DriverManager Create()
        {
            if (instance != null) return instance;
            instance = new DriverManager();
            instance.CreateHandle();
            return instance;
        }

        public void Start()
        {
            if (IsRunning)
                return;
            manager.Init();
            if (manager.Scanners != null)
                for (var i = 0; i < manager.Scanners.Count; ++i)
                {
                    try
                    {
                        var scanner = new ScannerWrapper(manager.Scanners[i], this);
                        if (!Scanners.Keys.Contains(scanner.Id))
                            Scanners.Add(scanner.Id, scanner);
                        string mess;
                        scanner.StartCapturing(out mess);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
            if (manager.Scanners != null)
                for (var i = 0; i < manager.Scanners.Count; ++i)
                {
                    try
                    {
                        var scanner = new ScannerWrapper(manager.Scanners[i], this);
                        string mess;
                        scanner.StopCapturing(out mess);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            manager.Uninit();
        }

        public bool StartCapturing(string scannerId, out string error)
        {
            if (string.IsNullOrEmpty(scannerId))
            {
                error = "ScannerId is null";
                return false;
            }
            if (Scanners.ContainsKey(scannerId)) return Scanners[scannerId].StartCapturing(out error);
            error = "Scanner not found";
            return false;
        }

        public ScannerWrapper GetScanner(string scannerId, out string error)
        {
            error = "";
            if (string.IsNullOrEmpty(scannerId))
            {
                error = "ScannerId is null";
                return null;
            }
            if (Scanners.ContainsKey(scannerId)) return Scanners[scannerId];
            error = "Scanner not found";
            return null;
        }

        public ScannerWrapper GetFirstScanner()
        {
            if (Scanners.Count == 0)
                return null;
            return Scanners.First().Value;
        }

        public List<ScannerState> GetScannersState()
        {
            return Scanners.Select(item => new ScannerState
            {
                Id = item.Key,
                IsCapturing = item.Value.IsCapturing,
                IsSensorOn = item.Value.IsSensorOn,
                ImageQuality = item.Value.ImageQuality,
                Timeout = item.Value.Timeout
            }).ToList();

        }



        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (Scanners != null)
                    foreach (var item in Scanners)
                        item.Value.Dispose();
                manager.Uninit();
                IsRunning = false;
            }
            base.Dispose(isDisposing);
        }

        private void Manager_ScannerEvent(object sender, UFScannerManagerScannerEventArgs e)
        {
        }
    }

    public class ScannerState
    {
        public String Id { get; set; }
        public bool IsCapturing { get; set; }
        public bool IsSensorOn { get; set; }
        public int ImageQuality { get; set; }
        public int Timeout { get; set; }


    }
}
