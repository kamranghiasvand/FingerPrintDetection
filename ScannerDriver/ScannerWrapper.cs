using System;
using System.Threading;
using Suprema;

namespace ScannerDriver
{
    public delegate void CaptureEventHandler(ScannerWrapper sender, byte[] template, string error);
    public class ScannerWrapper : IDisposable
    {
        private readonly UFScanner scanner;
        public IScannerManager Manager { get; }
        const int MaxTemplateSize = 1024;
        public event CaptureEventHandler CaptureEvent;
        private bool isCaptureSingleImage;
        public int ImageQuality { get; set; } = 40;

        public int Timeout
        {
            get { return scanner.Timeout; }
            set { scanner.Timeout = value; }
        }

        public bool IsCapturing => scanner.IsCapturing;
        public string Id => scanner.ID;
        public bool IsFingerOn => scanner.IsFingerOn;
        public bool IsSensorOn => scanner.IsSensorOn;

        public ScannerWrapper(UFScanner scanner, IScannerManager manager)
        {
            if (scanner == null)
                throw new ArgumentNullException(nameof(scanner));
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));
            Manager = manager;
            this.scanner = scanner;
            this.scanner.nTemplateType = 2002;
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
            isCaptureSingleImage = true;
            var res = CaptureSingleImage(out error);
            isCaptureSingleImage = false;
            return res;
        }

        private byte[] CaptureSingleImage(out string error)
        {
            var res = new byte[0];
            try
            {
                var iscapturing = IsCapturing;
                var timeout = Timeout;
                if (iscapturing)
                    if (!StopCapturing(out error))
                        return new byte[0];
                Timeout = 3000;
                if (!ClearCaptureImageBuffer(out error))
                    return new byte[0];
                while (IsCapturing)
                    Thread.Sleep(100);
                var counter = 0;
                while (true)
                {
                    if (counter >= 3)
                        break;
                    counter++;
                    var status = scanner.CaptureSingleImage();
                    if (status == UFS_STATUS.OK)
                    {
                        res = ExtractTemplate(out error);
                        break;
                    }
                    UFScanner.GetErrorString(status, out error);
                }
                Timeout = timeout;
                if (!iscapturing) return res;
                string x;
                StartCapturing(out x);
                return res;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return new byte[0];
            }
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
            scanner.nTemplateType = 2002;
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

        private DateTime lastEvent;
        private int Scanner_CaptureEvent(object sender, UFScannerCaptureEventArgs e)
        {
         
            if (!IsCapturing)
                return 1;
            if (isCaptureSingleImage)
                return 1;
            if (!e.FingerOn)
                return 1;
            if ((DateTime.Now.Subtract(lastEvent)).TotalSeconds < 2)
            {
                lastEvent = DateTime.Now;
                return 1;
            }
            lastEvent = DateTime.Now;
            string error;
            var template = ExtractTemplate(out error);
            if (template == null || template.Length == 0)
                return 1;
            if (CaptureEvent != null)
                (new Thread(() =>
                {
                    CaptureEvent(this, template, error);
                })).Start();
            
            return 1;
        }


        #region IDisposable Members

        private bool disposed;
        protected virtual void Dispose(bool disposing)
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
