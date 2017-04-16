using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerDriver
{
    public interface IScannerManager
    {
        void Start();
        void Stop();
        bool StartCapturing(string scannerId, out string error);
        ScannerWrapper GetScanner(string scannerId, out string error);
        ScannerWrapper GetFirstScanner();
        List<ScannerState> GetScannersState();

    }
}
