﻿using System;
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
        private readonly ICommandRunner cmdRunner;
        public Dictionary<string, ScannerWrapper> Scanners { get; } = new Dictionary<string, ScannerWrapper>();
        
        public bool IsRunning { get; private set; }

        private DriverManager(ICommandRunner cmdRunner)
        {
            manager = new UFScannerManager(this);
            manager.ScannerEvent += Manager_ScannerEvent;
            this.cmdRunner = cmdRunner;
            manager.Init();
            
        }

        public static DriverManager Create(ICommandRunner commmandRunner)
        {
            if (instance != null) return instance;
            instance = new DriverManager(commmandRunner);
            instance.CreateHandle();
            return instance;
        }

        public bool Start(out string error)
        {
            error = "";

            if (IsRunning)
                return true;
            try
            {
                manager.Init();
                if (manager.Scanners != null)
                    for (var i = 0; i < manager.Scanners.Count; ++i)
                    {
                        try
                        {
                            if (Scanners.Keys.Contains(manager.Scanners[i].CID))
                            {
                                string m;
                                Scanners[(manager.Scanners[i].CID)].StartCapturing(out m);
                            continue;
                            }
                            var scanner = new ScannerWrapper(manager.Scanners[i], this);
                            Scanners.Add(scanner.Id, scanner);
                            scanner.CaptureEvent += Scanner_CaptureEvent;
                            string mess;
                            scanner.StartCapturing(out mess);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                IsRunning = true;
                return true;

            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
        }

        private void Scanner_CaptureEvent(ScannerWrapper sender, byte[] template, string error)
        {
            cmdRunner.SendCapture(new CommandResponse {Template=template,Status=string.IsNullOrEmpty(error),Message= error });
        }
       

        public bool Stop(out string error)
        {
            error = "";
            IsRunning = false;
            try
            {
                if (manager.Scanners != null)
                    for (var i = 0; i < manager.Scanners.Count; ++i)
                    {
                        try
                        {
                            var scanner = new ScannerWrapper(manager.Scanners[i], this);
                            Scanners.Remove(scanner.Id);
                            string mess;
                            scanner.StopCapturing(out mess);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                manager.Uninit();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
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

        public byte[] CaptureSingleImage(string scannerId, out string error)
        {
            if (string.IsNullOrEmpty(scannerId))
            {
                error = "ScannerId is empty";
                return new byte[0];
            }
            var scanner = Scanners.FirstOrDefault(m=>m.Key==scannerId);
            if (scanner.Value != null)
            {
                return scanner.Value.CaptureSingleTemplate(out error);

            }
            error = "Scanner not found";
            return new byte[0];
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
            try
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
            catch
            {
                
            }
            return new List<ScannerState>();

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
        public string Id { get; set; }
        public bool IsCapturing { get; set; }
        public bool IsSensorOn { get; set; }
        public int ImageQuality { get; set; }
        public int Timeout { get; set; }

    }
}
