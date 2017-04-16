using System;
using System.Threading;
using Suprema;

namespace ScannerDriver
{
    public delegate void CaptureEventHandler(ScannerWrapper sender, byte[] template, string error);
    public class ScannerWrapper : IDisposable
    {
        private readonly UFScanner scanner;
        private IScannerManager manager;
        const int MaxTemplateSize = 1024;
        public event CaptureEventHandler CaptureEvent;
        public int ImageQuality { get; set; } = 40;

        public int Timeout
        {
            get { return scanner.Timeout; }
            set { scanner.Timeout = value; }
        } 

        public bool IsCapturing => scanner.IsCapturing;
        public string Id => scanner.CID;
        public bool IsFingerOn => scanner.IsFingerOn;
        public bool IsSensorOn => scanner.IsSensorOn;

        public ScannerWrapper(UFScanner scanner, IScannerManager manager)
        {
            if (scanner == null)
                throw new ArgumentNullException(nameof(scanner));
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));
            this.manager = manager;
            this.scanner = scanner;
            this.scanner.CaptureEvent += Scanner_CaptureEvent;
            Timeout = -1;
        }


        public bool ClearCaptureImageBuffer(out string error)
        {
            error = "";
            var status = scanner.ClearCaptureImageBuffer();
            if (status == UFS_STATUS.OK) return true;
            UFScanner.GetErrorString(status, out error);
            return false;
        }

        public byte[] CaptureSingleTemplate(out string error)
        {
            var status = scanner.CaptureSingleImage();
            if (status == UFS_STATUS.OK) return ExtractTemplate(out error);
            UFScanner.GetErrorString(status, out error);
            return new byte[0];
        }
        public bool StartCapturing(out string error)
        {
            error = "";
            scanner.ClearCaptureImageBuffer();
            var status = scanner.StartCapturing();
            if (status == UFS_STATUS.OK)
                return true;

            UFScanner.GetErrorString(status, out error);
            return false;
        }

        public bool StopCapturing(out string error)
        {
            error = "";
            var status = scanner.AbortCapturing();
            if (status == UFS_STATUS.OK)
                return true;
            
            UFScanner.GetErrorString(status, out error);
            return false;
        }

        private byte[] ExtractTemplate(out string error)
        {
            var bytes = new byte[MaxTemplateSize];
            int templateSize;
            int enrollQuality;
            var status = scanner.ExtractEx(MaxTemplateSize, bytes, out templateSize, out enrollQuality);
            if (status != UFS_STATUS.OK)
            {
                UFScanner.GetErrorString(status, out error);
                return new byte[0];
            }
            if (enrollQuality < ImageQuality)
            {
                error = "Quality is not Enough";
                return new byte[0];
            }
            var template = new byte[templateSize];
            for (var i = 0; i < templateSize; i++)
                template[i] = bytes[i];
            error = "";
            return template;
        }

        private int Scanner_CaptureEvent(object sender, UFScannerCaptureEventArgs e)
        {
            if (!IsCapturing)
                return 1;
            if (!e.FingerOn)
                return 1;
            string error;
            var template = ExtractTemplate(out error);
            if (CaptureEvent != null)
                (new Thread(() =>
                {
                    CaptureEvent(this, template, error);
                })).Start();
            return 1;
        }


        #region IDisposable Members

        private Boolean disposed;
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                scanner.CaptureEvent -= Scanner_CaptureEvent;
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~ScannerWrapper()
        {
            Dispose(false);
        }

        #endregion

    }
}
