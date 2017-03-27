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

        public static DriverManager GetManager()
        {
            if (instance != null) return instance;
            instance = new DriverManager();
            instance.CreateHandle();
            return instance;
        }

        public void Start()
        {

            if (manager.Scanners != null)
                for (var i = 0; i < manager.Scanners.Count; ++i)
                {
                    var scanner = new ScannerWrapper(manager.Scanners[i], this);
                    Scanners.Add(scanner.Id,scanner);
                }
            IsRunning = true;
        }

        public bool StartCapturing(string scannerId,out string error)
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
}
