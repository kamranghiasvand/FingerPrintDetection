using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common.Logging;
using FingerPrintDetectionModel;
using Newtonsoft.Json;

namespace FingerPrintDetectionWeb.Manager
{
    public sealed class ScannerManagerConnector : IDisposable
    {
        public bool IsRunning { get; private set; }
        public readonly ILog Log = LogManager.GetLogger(typeof(ScannerManagerConnector));
        public ScannerManagerConnector()
        {
            try
            {
                if (Process.GetProcessesByName("ScannerDriver").Length != 0 ||
                    Process.GetProcessesByName("ScannerDriver.vshost").Length != 0) return;
                Log.Debug("Starting ScannerDriver Process");
                var info = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = AppDomain.CurrentDomain.BaseDirectory + "bin\\ScannerDriver.exe"

                };
                Process.Start(info);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        public static ScannerManagerConnector Create()
        {
            var instance = new ScannerManagerConnector();
            using (var dbContext = new ApplicationDbContext())
            {
                var state = dbContext.ScannerManagerStates.FirstOrDefault();
                if (state == null)
                {
                    dbContext.ScannerManagerStates.Add(new ScannerManagerState { Started = false });
                    dbContext.SaveChanges();
                }
                else if (state.Started)
                    instance.Start();
                else instance.Stop();
            }
            return instance;
        }
        public void Start()
        {
            Log.Debug("Scanner Manager Starting");
            if (IsRunning)
                return;
            IsRunning = InitDriver();
            using (var dbContext = new ApplicationDbContext())
            {
                var state = dbContext.ScannerManagerStates.FirstOrDefault();
                if (state == null)
                {
                    dbContext.ScannerManagerStates.Add(new ScannerManagerState { Started = IsRunning });
                    dbContext.SaveChanges();
                }
                else
                {
                    state.Started = IsRunning;
                    dbContext.Entry(state).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            if (IsRunning)
                Log.Debug("Scanner Manager Started");
            else
                Log.Info("Scanner Manager Failed To Start");
        }

        private bool InitDriver()
        {
            try
            {
                Log.Debug("Scanner Manager InitDriver");
                using (var client = new TcpClient())
                {
                    client.Connect(IPAddress.Parse("127.0.0.1"), 1468);
                    var reader = new StreamReader(client.GetStream());
                    var writer = new StreamWriter(client.GetStream());
                    var request = new CommandRequest { Name = CommandName.Start };
                    writer.WriteLine(JsonConvert.SerializeObject(request));
                    writer.Flush();
                    var response = JsonConvert.DeserializeObject<CommandResponse>(reader.ReadLine());
                    Log.Debug(response);
                    return response.Status;
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return false;
        }

        private bool UninitDriver()
        {
            try
            {
                Log.Debug("Scanner Manager UninitDriver");
                using (var client = new TcpClient())
                {
                    client.Connect(IPAddress.Parse("127.0.0.1"), 1468);
                    var reader = new StreamReader(client.GetStream());
                    var writer = new StreamWriter(client.GetStream());
                    var request = new CommandRequest { Name = CommandName.Stop };
                    writer.WriteLine(JsonConvert.SerializeObject(request));
                    writer.Flush();
                    var response = JsonConvert.DeserializeObject<CommandResponse>(reader.ReadLine());
                    Log.Debug(response);
                    return response.Status;
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return false;
        }
        public void Stop()
        {
            Log.Debug("Scanner Manager Stopping");
            IsRunning = !UninitDriver();
            using (var dbContext = new ApplicationDbContext())
            {
                var state = dbContext.ScannerManagerStates.FirstOrDefault();
                if (state == null)
                {
                    dbContext.ScannerManagerStates.Add(new ScannerManagerState { Started = IsRunning });
                    dbContext.SaveChanges();
                }
                else
                {
                    state.Started = IsRunning;
                    dbContext.Entry(state).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            if (!IsRunning)
                Log.Debug("Scanner Manager Stopped");
            else
                Log.Info("Scanner Manager Failed To Stopped");
        }

        // ReSharper disable once RedundantAssignment
        public async Task<byte[]> CaptureSingleTemplate(string scannerId, string error)
        {
            var res = new byte[0];
            await Task.Run(() =>
            {
                try
                {
                    Log.Debug("Capturing Single Image");
                    var client = new TcpClient();
                    client.Connect(IPAddress.Parse("127.0.0.1"), 1468);
                    var reader = new StreamReader(client.GetStream());
                    var writer = new StreamWriter(client.GetStream());
                    var request = new CommandRequest { Name = CommandName.CaptureSingleImage };
                    request.Arguments.Add(new Argument { Name = "ScannerId", Value = scannerId });
                    writer.WriteLine(JsonConvert.SerializeObject(request));
                    writer.Flush();
                    var response = JsonConvert.DeserializeObject<CommandResponse>(reader.ReadLine());
                    error = response.Message;
                    if (!response.Status)
                        Log.Info(error);
                    else
                        Log.Debug(error);
                    if (response.Status)
                        res = response.Template;
                }
                catch (Exception ex)
                {
                    error = ex.ToString();
                }

            });
            return res;
        }

        public async Task<List<ScannerState>> GetScannersState()
        {
            var res = new List<ScannerState>();
            await Task.Run(() =>
            {
                try
                {
                    Log.Debug("Getting Scanners State");

                    using (var client = new TcpClient())
                    {
                        client.Connect(IPAddress.Parse("127.0.0.1"), 1468);
                        var reader = new StreamReader(client.GetStream());
                        var writer = new StreamWriter(client.GetStream());
                        var request = new CommandRequest { Name = CommandName.GetScannerState };
                        writer.WriteLine(JsonConvert.SerializeObject(request));
                        writer.Flush();

                        var response = JsonConvert.DeserializeObject<CommandResponse>(reader.ReadLine());
                        Log.Debug(response);
                        res = response.ScannersState;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });
            return res;
        }



        #region IDisposable Members

        private bool disposed;

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~ScannerManagerConnector()
        {
            Dispose(false);
        }
        #endregion


    }


    public class CommandRequest
    {
        public CommandName Name { get; set; }
        public List<Argument> Arguments { get; set; } = new List<Argument>();
        public override string ToString()
        {
            var res = $"Type: CommandRequest  {{ CommandName:{Name} ,ArgList:[";
            if (Arguments == null)
                res += ",";
            else
                res = Arguments.Aggregate(res, (current, item) => current + (item + ","));
            res = res.Substring(0, res.Length - 1);
            res += "]}";
            return res;
        }
    }

    public class CommandResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public byte[] Template { get; set; } = new byte[0];
        public List<ScannerState> ScannersState { get; set; } = new List<ScannerState>();
        public override string ToString()
        {

            return
                $"Type: CommandResponse  {{ Status:{{{Status}}}\tMessage:{{{Message}}}\tTemplateLen:{{{Template?.Length}}}\tScannerStateLen:{{{ScannersState?.Count}}} }}";
        }
    }

    public class Argument
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public override string ToString()
        {
            return $"Type: Argument  {{ Name:{Name}, Value:{Value} }}";
        }


    }
    public enum CommandName
    {
        Start,
        Stop,
        CaptureSingleImage,
        GetScannerState,
        Exit
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
