using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

namespace ScannerDriver
{
    public interface ICommandRunner
    {
        void SendCapture(CommandResponse response);
    }
    public class CommandRunner : ICommandRunner
    {
        private Thread worker;
        private readonly TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1468);
        private readonly IScannerManager scannerManager;
        private static CommandRunner instance;
        public bool IsRunning { get; private set; }

        private CommandRunner()
        {
            scannerManager = DriverManager.Create(this);

        }

        public static CommandRunner Create()
        {
            if (instance != null)
                return instance;
            instance = new CommandRunner();
            return instance;
        }
        public void Start()
        {
            if (IsRunning)
                return;
            worker = new Thread(ThreadWorker) { IsBackground = false };
            IsRunning = true;
            worker.Start();
        }
        public void Stop()
        {
            if (!IsRunning)
                return;
            IsRunning = false;
            string mess;
            scannerManager.Stop(out mess);
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
                var client = listener.AcceptTcpClient();
                ProccessRequest(client);
            }
        }

        private void ProccessRequest(TcpClient client)
        {
            if (client == null)
                return;
            if (!client.Connected)
            {
                return;
            }
            while (client.Connected)
            {
                try
                {
                    if (client.Available <= 0)
                    {
                        Thread.Sleep(200);
                        continue;
                    }
                    var reader = new StreamReader(client.GetStream());
                    var writer = new StreamWriter(client.GetStream());
                    var request = JsonConvert.DeserializeObject<CommandRequest>(reader.ReadLine());
                    byte[] template = null;
                    List<ScannerState> scannerState = new List<ScannerState>();
                    var mess = "";
                    switch (request.Name)
                    {
                        case CommandName.Start:
                            scannerManager.Start(out mess);
                            break;
                        case CommandName.Stop:
                            scannerManager.Stop(out mess);
                            break;
                        case CommandName.CaptureSingleImage:
                            var scannerId = "";
                           foreach (var item in request.Arguments.Where(item => item.Name == "ScannerId"))
                           {
                               scannerId = (string)item.Value;
                               break;
                           }
                            if (scannerId == "")
                            {
                                mess = "scannerId is empty";
                                break;
                            }
                            template= scannerManager.CaptureSingleImage(scannerId,out mess);
                            break;
                        case CommandName.GetScannerState:
                            scannerState= scannerManager.GetScannersState();
                            break;
                        case CommandName.Exit:
                            Stop();
                            break;
                    }
                    var res = new CommandResponse { Status = string.IsNullOrEmpty(mess), Message = mess,Template=template,ScannersState = scannerState };
                    writer.WriteLine(JsonConvert.SerializeObject(res));
                    writer.Flush();
                    writer.Close();
                    client.Close();

                }
                catch (Exception ex)
                {
                    var res = new CommandResponse { Status = false, Message = ex.ToString(),Template=new byte[0] };
                    var writer = new StreamWriter(client.GetStream());
                    writer.WriteLine(JsonConvert.SerializeObject(res));
                    writer.Flush();
                    client.Close();
                }
            }

        }

        public void SendCapture(CommandResponse response)
        {
            
            try
            {
                var client = new TcpClient();
                client.Connect(IPAddress.Parse("127.0.0.1"), 1568);
                var writer = new StreamWriter(client.GetStream());
                writer.WriteLine(JsonConvert.SerializeObject(response));
                writer.Flush();
                writer.Close();
                client.Close();
            }
            catch
            {
                // ignored
            }
        }
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
        GetScannerState,
        Exit
    }
}
