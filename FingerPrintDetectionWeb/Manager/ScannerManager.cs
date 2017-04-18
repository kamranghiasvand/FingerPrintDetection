using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FingerPrintDetectionModel;
using Newtonsoft.Json;
using Suprema;

namespace FingerPrintDetectionWeb.Manager
{
    public class ScannerManager : IDisposable
    {
        private readonly Thread worker;
        private readonly TcpListener listener;
        public bool IsRunning { get; private set; }
        private static ScannerManager instance;
        private ScannerManager()
        {
            worker = new Thread(ThreadWorker);
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1568);
        }

        public static ScannerManager Create()
        {
            if (instance != null)
                return instance;
            instance = new ScannerManager();
            return instance;
        }
        public void Start()
        {
            if (IsRunning)
                return;
            IsRunning = InitDriver();
            if (IsRunning)
                worker.Start();

        }

        private static bool InitDriver()
        {
            try
            {
                using (var client = new TcpClient())
                {
                    client.Connect(IPAddress.Parse("127.0.0.1"), 1468);
                    var reader = new StreamReader(client.GetStream());
                    var writer = new StreamWriter(client.GetStream());
                    var request = new CommandRequest { Name = CommandName.Start };
                    writer.WriteLine(JsonConvert.SerializeObject(request));
                    writer.Flush();
                    var response = JsonConvert.DeserializeObject<CommandResponse>(reader.ReadLine());
                    return response.Status;
                }

            }
            catch
            {

            }
            return false;
        }

        private static bool UninitDriver()
        {
            try
            {
                using (var client = new TcpClient())
                {
                    client.Connect(IPAddress.Parse("127.0.0.1"), 1468);
                    var reader = new StreamReader(client.GetStream());
                    var writer = new StreamWriter(client.GetStream());
                    var request = new CommandRequest { Name = CommandName.Stop };
                    writer.WriteLine(JsonConvert.SerializeObject(request));
                    writer.Flush();
                    var response = JsonConvert.DeserializeObject<CommandResponse>(reader.ReadLine());
                    return response.Status;
                }

            }
            catch
            {

            }
            return false;
        }
        public void Stop()
        {
            IsRunning = !UninitDriver();
            if (!IsRunning)
                worker.Join();
        }
        private void ThreadWorker()
        {
            listener.Start();
            while (IsRunning)
            {
                if (!listener.Pending())
                {
                    Thread.Sleep(250);
                    continue;
                }
                ProccessRequest(listener.AcceptTcpClient());
            }
        }

        private static void ProccessRequest(TcpClient client)
        {
            try
            {
                var reader = new StreamReader(client.GetStream());
                var response = JsonConvert.DeserializeObject<CommandResponse>(reader.ReadLine());
                if (!response.Status)
                    return;
                var userId = Verify(response.Template);
                if (userId < 0)
                    return;
                using (var dbContext = new ApplicationDbContext())
                {
                    var user = dbContext.RealUsers.FirstOrDefault(m => !m.Deleted && m.Id == userId);
                    if (user?.LogicalUser == null || user.LogicalUser.Deleted)
                        return;
                    if (user.LogicalUser.Plan == null || user.LogicalUser.Plan.Deleted)
                        return;
                    if (user.LogicalUser.Sound == null || user.LogicalUser.Sound.Deleted ||
                        string.IsNullOrEmpty(user.LogicalUser.Sound.Uri))
                        return;
                    var plan = user.LogicalUser.Plan;
                    var dtn = DateTime.Now;
                    if (TimeSpan.Compare(dtn.TimeOfDay, plan.StartTime.TimeOfDay) < 0 ||
                        TimeSpan.Compare(dtn.TimeOfDay, plan.EndTime.TimeOfDay) >= 0) return;
                    var sound = user.LogicalUser.Sound;
                    try
                    {
                        var player = new System.Media.SoundPlayer { SoundLocation = new Uri(sound.Uri).AbsolutePath };
                        for (var i = 0; i < user.LogicalUser.Plan.RepeatNumber; ++i)
                            player.Play();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        // ReSharper disable once RedundantAssignment
        public async Task<byte[]> CaptureSingleTemplate(string scannerId, string error)
        {
            var res = new byte[0];
            await Task.Run(() =>
            {
                try
                {


                    var client = new TcpClient();
                    client.Connect(IPAddress.Parse("127.0.0.1"), 1468);
                    var reader = new StreamReader(client.GetStream());
                    var writer = new StreamWriter(client.GetStream());
                    var request = new CommandRequest { Name = CommandName.CaptureSingleImage };
                    request.Arguments.Add(new Argument { Name = "ScannerId", Value = scannerId });
                    writer.WriteLine(JsonConvert.SerializeObject(request));
                    writer.Flush();
                    writer.Close();

                    var response = JsonConvert.DeserializeObject<CommandResponse>(reader.ReadLine());
                    error = response.Message;
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


                    using (var client = new TcpClient())
                    {
                        client.Connect(IPAddress.Parse("127.0.0.1"), 1468);
                        var reader = new StreamReader(client.GetStream());
                        var writer = new StreamWriter(client.GetStream());
                        var request = new CommandRequest { Name = CommandName.GetScannerState };
                        writer.WriteLine(JsonConvert.SerializeObject(request));
                        writer.Flush();

                        var response = JsonConvert.DeserializeObject<CommandResponse>(reader.ReadLine());
                        res = response.ScannersState;
                    }
                }
                catch (Exception ex)
                {
                    // ignored
                }
            });
            return res;
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
                    foreach (var realUser in dbContext.RealUsers)
                    {
                        var found = false;

                        if (realUser.FirstFinger != null)
                            matcher.Verify(template, template.Length, realUser.FirstFinger,
                                realUser.FirstFinger.Length, out found);
                        if (found)
                            return realUser.Id;

                        if (realUser.SecondFinger != null)
                            matcher.Verify(template, template.Length, realUser.SecondFinger,
                                realUser.SecondFinger.Length, out found);
                        if (found)
                            return realUser.Id;

                        if (realUser.ThirdFinger != null)
                            matcher.Verify(template, template.Length, realUser.ThirdFinger,
                                realUser.ThirdFinger.Length, out found);
                        if (found)
                            return realUser.Id;
                    }
                }

            }
            catch (Exception)
            {
                // ignored
            }
            return -1;
        }

        #region IDisposable Members

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                IsRunning = false;
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~ScannerManager()
        {
            Dispose(false);
        }
        #endregion


    }


    public class CommandRequest
    {
        public CommandName Name { get; set; }
        public List<Argument> Arguments { get; set; } = new List<Argument>();
    }

    public class CommandResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public byte[] Template { get; set; } = new byte[0];
        public List<ScannerState> ScannersState { get; set; } = new List<ScannerState>();
    }

    public class Argument
    {
        public string Name { get; set; }
        public object Value { get; set; }

    }
    public enum CommandName
    {
        Start,
        Stop,
        CaptureSingleImage,
        GetScannerState
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
