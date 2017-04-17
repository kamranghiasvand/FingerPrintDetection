using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerDriver
{
    public interface IScannerManager
    {
        bool Start(out string error);
        bool Stop(out string error);
        bool StartCapturing(string scannerId, out string error);
        ScannerWrapper GetScanner(string scannerId, out string error);
        ScannerWrapper GetFirstScanner();
        List<ScannerState> GetScannersState();
        byte[] CaptureSingleImage(string scannerId,out string error);

    }
}
