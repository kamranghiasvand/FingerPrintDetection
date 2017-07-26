using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;
using Newtonsoft.Json;

namespace ScannerDriver
{
    public class CommandRunner
    {
        private Thread worker;
        private readonly TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1468);
        private readonly IScannerManager scannerManager;
        private static CommandRunner instance;
        public readonly ILog Log = LogManager.GetLogger(typeof(CommandRunner));
        public bool IsRunning { get; private set; }

        private CommandRunner()
        {
            Log.Debug("New Instance Creating");
            scannerManager = DriverManager.Create();
            Log.Debug("New Instance Created");

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
            Log.Debug("Starting");
            worker = new Thread(ThreadWorker) { IsBackground = false };
            IsRunning = true;
            worker.Start();
            Log.Debug("Started");
        }
        public void Stop()
        {
            if (!IsRunning)
                return;
            Log.Debug("Stopping");
            IsRunning = false;
            string mess;
            scannerManager.Stop(out mess);
            if (!string.IsNullOrEmpty(mess))
                Log.Error(mess);
            worker.Join();
            Log.Debug("Stopped");
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
            Log.Debug("New Request Received From: " + client.Client.RemoteEndPoint);
            Log.Debug("Processing Request");
            if (!client.Connected)
            {
                Log.Debug("client Disconnected");
                return;
            }
            if (!client.Connected) return;
            try
            {
                while (client.Available <= 0)
                    Thread.Sleep(200);

                Log.Debug("Get Client Streams");
                var reader = new StreamReader(client.GetStream());
                var writer = new StreamWriter(client.GetStream());
                Log.Debug("De-serializing Request");
                var request = JsonConvert.DeserializeObject<CommandRequest>(reader.ReadLine());
                Log.Debug(request);
                byte[] template = null;
                var scannerState = new List<ScannerState>();
                var mess = "";
                switch (request.Name)
                {
                    case CommandName.Start:
                        Log.Debug("Command Name Is 'Start'");
                        Log.Debug("Starting Manager");
                        scannerManager.Start(out mess);
                        if (!string.IsNullOrEmpty(mess))
                            Log.Error(mess);
                        break;
                    case CommandName.Stop:
                        Log.Debug("Command Name Is 'Stop'");
                        Log.Debug("Stopping Manager");
                        scannerManager.Stop(out mess);
                        if (!string.IsNullOrEmpty(mess))
                            Log.Error(mess);
                        break;
                    case CommandName.CaptureSingleImage:
                        Log.Debug("Command Name Is 'CaptureSingleImage'");
                        Log.Debug("Capturing Single Image");
                        var scannerId = "";
                        Log.Debug("Finding Scanner Id");
                        foreach (var item in request.Arguments.Where(item => item.Name == "ScannerId"))
                        {
                            scannerId = (string)item.Value;
                            break;
                        }
                        if (scannerId == "")
                        {
                            Log.Info("Scanner Id is empty");
                            mess = "scannerId is empty";
                            break;
                        }
                        template = scannerManager.CaptureSingleImage(scannerId, out mess);
                        if (!string.IsNullOrEmpty(mess))
                            Log.Error(mess);
                        break;
                    case CommandName.GetScannerState:
                        Log.Debug("Command Name Is 'GetScannerState'");
                        Log.Debug("Getting Scanners State");
                        scannerState = scannerManager.GetScannersState();
                        Log.Debug("Getting Scanners State done");
                        break;
                    case CommandName.Exit:
                        Log.Debug("Command Name Is 'Exit'");
                        Stop();
                        break;
                }
                Log.Debug("Creating Response");
                var res = new CommandResponse { Status = string.IsNullOrEmpty(mess), Message = mess, Template = template, ScannersState = scannerState };
                Log.Debug(res);
                writer.WriteLine(JsonConvert.SerializeObject(res));
                writer.Flush();
                writer.Close();
                client.Close();
                Log.Debug("Processing Request Is Done");

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                try
                {
                    var res = new CommandResponse { Status = false, Message = ex.ToString(), Template = new byte[0] };
                    var writer = new StreamWriter(client.GetStream());
                    writer.WriteLine(JsonConvert.SerializeObject(res));
                    writer.Flush();
                    client.Close();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

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
}
