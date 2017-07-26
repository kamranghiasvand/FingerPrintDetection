using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Common.Logging;
using FingerPrintDetectionModel;
using NAudio.Wave;
using Suprema;

namespace ScannerDriver
{
    public class DriverManager : Control, IScannerManager
    {
        private readonly UFScannerManager manager;
        private static DriverManager instance;
        public Dictionary<string, ScannerWrapper> Scanners { get; } = new Dictionary<string, ScannerWrapper>();
        public readonly ILog Log = LogManager.GetLogger(typeof(DriverManager));
        public bool IsRunning { get; private set; }

        private DriverManager()
        {
            Log.Debug("Creating New Instance");
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

        public bool Start(out string error)
        {
            error = "";

            if (IsRunning)
                return true;
            Log.Debug("Driver Manager Starting");
            try
            {
                manager.Init();
                Log.Debug("UFScannerManager Initiated");
                if (manager.Scanners != null)
                {
                    Log.Debug("Available Scanner Number: " + manager.Scanners.Count);
                    for (var i = 0; i < manager.Scanners.Count; ++i)
                    {
                        try
                        {
                            string mess;
                            if (Scanners.Keys.Contains(manager.Scanners[i].CID))
                            {
                                Log.Debug("Scanner With CID '" + manager.Scanners[i].CID + "' Initiated Before.");
                                Log.Debug("Start Capturing Scanner With CID '" + manager.Scanners[i].CID + "'");
                                Scanners[manager.Scanners[i].CID].StartCapturing(out mess);
                                if (!string.IsNullOrEmpty(mess))
                                    Log.Error(mess);
                                continue;
                            }
                            Log.Debug("Scanner With CID '" + manager.Scanners[i].CID + "' Is New");
                            var scanner = new ScannerWrapper(manager.Scanners[i], this);
                            Scanners.Add(scanner.Id, scanner);
                            scanner.CaptureEvent += Scanner_CaptureEvent;
                           
                            Log.Debug("Start Capturing Scanner With CID '" + manager.Scanners[i].CID + "'");
                            scanner.StartCapturing(out mess);
                            if (!string.IsNullOrEmpty(mess))
                                Log.Error(mess);
                        }
                        catch(Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }
                }
                IsRunning = true;
                Log.Debug("Driver Manager Started");
                return true;

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                error = ex.ToString();
                return false;
            }
        }

        private void Scanner_CaptureEvent(ScannerWrapper sender, byte[] template, string error)
        {
            Log.Debug($"Scanner With CID {sender.Id} Detect New Finger. error:'{error}' templateLen:'{template?.Length}'");

            if (!string.IsNullOrEmpty(error))
            {
                Log.Info(error);
                return;
            }
            if (template == null || template.Length == 0)
            {
                Log.Info("Template is Null Or Empty");
                return;
            }
            try
            {
                Log.Debug("Verifying Template");
                var userId = Verify(template);
                Log.Debug("User ID: " + userId);
                if (userId < 0)
                    return;
                using (var dbContext = new ApplicationDbContext())
                {
                    var dtn = DateTime.Now;
                    var user = dbContext.RealUsers.FirstOrDefault(m => !m.Deleted && m.Id == userId);
                    if (user?.LogicalUser == null || user.LogicalUser.Deleted)
                    {
                        Log.Info($"No User Found With ID {userId}");
                        return;
                    }
                    try
                    {
                        var log = new Log
                        {
                            RealUserId = user.Id,
                            Time = dtn,
                            Income = true,
                            LogicalUserId = (long)user.LogicalUser?.Id,
                            PlanId = user.LogicalUser?.Plan?.Id ?? -1

                        };
                        var find =
                            dbContext.Logs.Where(
                                m =>
                                    !m.Deleted &&
                                    DbFunctions.CreateDateTime(dtn.Year, dtn.Month, dtn.Day, 0, 0, 0) <= m.Time);
                        if (find.Count() != 0)
                        {
                            var lastState = find.OrderByDescending(m => m.Time).First();
                            if (lastState.Income)
                                log.Income = false;
                        }
                        Log.Debug(log);
                        dbContext.Logs.Add(log);
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                    if (user.LogicalUser.Plan == null || user.LogicalUser.Plan.Deleted)
                    {
                        Log.Info($"There Is No Plan For User : {userId}");
                        return;
                    }

                    if (user.LogicalUser.Sound == null || user.LogicalUser.Sound.Deleted ||
                        string.IsNullOrEmpty(user.LogicalUser.Sound.Uri))
                    {
                        Log.Info($"There Is No Sound For User : {userId}");
                        return;
                    }
                    var plan = user.LogicalUser.Plan;

                    if (TimeSpan.Compare(dtn.TimeOfDay, plan.StartTime.TimeOfDay) < 0 ||
                        TimeSpan.Compare(dtn.TimeOfDay, plan.EndTime.TimeOfDay) >= 0)
                    {
                        Log.Info("Not In Active Zone Of Plan");
                        return;
                    }
                    var sound = user.LogicalUser.Sound;
                    Log.Debug("Sound To Play: " + sound);
                    try
                    {
                        Log.Debug("Sound Playing");
                        using (IWavePlayer waveOutDevice = new WaveOut())
                        {
                            var audioFileReader = new AudioFileReader(new Uri(sound.Uri).AbsolutePath);
                            waveOutDevice.Init(audioFileReader);
                            for (var i = 0; i < user.LogicalUser.Plan.RepeatNumber; ++i)
                            {
                                waveOutDevice.Play();
                                while (waveOutDevice.PlaybackState == PlaybackState.Playing)
                                {
                                    Thread.Sleep(100);
                                }
                                audioFileReader.Seek(0, SeekOrigin.Begin);

                            }
                        }
                        Log.Debug("Sound Played");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }





            
        }

        private static long Verify(byte[] template)
        {
            try
            {
                if (template == null || template.Length == 0)
                    return -1;
                var matcher = new UFMatcher { FastMode = true, nTemplateType = 2002, SecurityLevel = 4 };
                using (var dbContext = new ApplicationDbContext())
                {
                    foreach (var realUser in dbContext.RealUsers.Where(m => !m.Deleted))
                    {
                        var found = false;
                        var stat = UFM_STATUS.OK;
                        if (realUser.FirstFinger != null)
                            stat = matcher.Verify(template, template.Length, realUser.FirstFinger,
                                realUser.FirstFinger.Length, out found);
                        if (found)
                            return realUser.Id;

                        if (realUser.SecondFinger != null)
                            stat = matcher.Verify(template, template.Length, realUser.SecondFinger,
                                realUser.SecondFinger.Length, out found);

                        if (found)
                            return realUser.Id;

                        if (realUser.ThirdFinger != null)
                            stat = matcher.Verify(template, template.Length, realUser.ThirdFinger,
                                realUser.ThirdFinger.Length, out found);
                        if (found)
                            return realUser.Id;
                        string error;
                        UFMatcher.GetErrorString(stat, out error);
                    }
                }

            }
            catch (Exception)
            {
                // ignored
            }
            return -1;
        }
        public bool Stop(out string error)
        {
            Log.Debug("Stopping Driver Manager");
            error = "";
            IsRunning = false;
            try
            {
                if (manager.Scanners != null)
                    for (var i = 0; i < manager.Scanners.Count; ++i)
                    {
                        try
                        {
                            Log.Debug($"Stopping Scanner With CID '{manager.Scanners[i].CID}'");
                            var scanner = new ScannerWrapper(manager.Scanners[i], this);
                            Scanners.Remove(scanner.Id);
                            string mess;
                            scanner.StopCapturing(out mess);
                            if (!string.IsNullOrEmpty(mess))
                                Log.Error(mess);
                        }
                        catch(Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }
                Log.Debug("UnInitiating UFScanner");
               var status= manager.Uninit();
                if (status == UFS_STATUS.OK)
                    Log.Debug(status.ToString());
                else 
                    Log.Error(status.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                error = ex.ToString();
                return false;
            }
        }

        public bool StartCapturing(string scannerId, out string error)
        {
            try
            {
                Log.Debug($"Driver Manager Start Capturing Scanner With CID '{scannerId}'");
                if (string.IsNullOrEmpty(scannerId))
                {
                    error = "ScannerId is null";
                    return false;
                }
                if (Scanners.ContainsKey(scannerId)) return Scanners[scannerId].StartCapturing(out error);
                error = "Scanner not found";
                return false;
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                error = "System Failure";
                return false;
            }
        }

        public byte[] CaptureSingleImage(string scannerId, out string error)
        {
            try
            {
                Log.Debug($"Capturing Single Image From Scanner With CID '{scannerId}'");

                if (string.IsNullOrEmpty(scannerId))
                {
                    error = "ScannerId is empty";
                    return new byte[0];
                }
                var scanner = Scanners.FirstOrDefault(m => m.Key == scannerId);
                if (scanner.Value != null)
                    return scanner.Value.CaptureSingleTemplate(out error);
                
                error = "Scanner not found";
                return new byte[0];
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                error = "System Failure";
                return new byte[0];
            }
        }

        public ScannerWrapper GetScanner(string scannerId, out string error)
        {
            try
            {
                Log.Debug($"Getting Scanner With CID '{scannerId}'");
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
            catch(Exception ex)
            {
                Log.Error(ex);
                error = "System Failure";
                return null;
            }
        }

        public ScannerWrapper GetFirstScanner()
        {
            return Scanners.Count == 0 ? null : Scanners.First().Value;
        }

        public List<ScannerState> GetScannersState()
        {
            try
            {
                Log.Debug("Getting Scanner State");
                return Scanners.Select(item => new ScannerState
                {
                    Id = item.Key,
                    IsCapturing = item.Value.IsCapturing,
                    IsSensorOn = item.Value.IsSensorOn,
                    ImageQuality = item.Value.ImageQuality,
                    Timeout = item.Value.Timeout
                }).ToList();
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
            return new List<ScannerState>();

        }



        protected override void Dispose(bool isDisposing)
        {
            Log.Debug("Disposing Driver Manager");
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
            Log.Debug("UFScannerManager Event Raised: ");
            Log.Debug(e);
        }
    }

    public class ScannerState
    {
        public string Id { get; set; }
        public bool IsCapturing { get; set; }
        public bool IsSensorOn { get; set; }
        public int ImageQuality { get; set; }
        public int Timeout { get; set; }
        public override string ToString()
        {
            return
                $"Type:ScannerState {{Id:{Id}, IsCapturing:{IsCapturing}, IsSensorOn:{IsSensorOn}, ImageQuality:{ImageQuality}, Timeout:{Timeout} }}";
        }

    }
}
