using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FingerPrintScannerService
{
    public class CommandRunner
    {
        private Thread worker;
        private readonly TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1368);

        public bool IsRunning { get; private set; }

        public void Start()
        {

            worker = new Thread(ThreadWorker) { IsBackground = false };
            IsRunning = true;
            worker.Start();
        }
        public void Stop()
        {
            IsRunning = false;
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
                new Thread(() => { ProccessRequest(listener.AcceptTcpClient()); }).Start();
            }
        }

        private void ProccessRequest(TcpClient client)
        {
            if (client == null)
                return;
            if (!client.Connected)
            {
                client.Close();
                return;
            }
            try
            {
                var reader = new StreamReader(client.GetStream());
                var writer = new StreamWriter(client.GetStream());
                var request = JsonConvert.DeserializeObject<CommandRequest>(reader.ReadLine());
                var result = true;
                var errorCode = ErrorCode.NoError;
                switch (request.Name)
                {
                    case CommandName.ForceActivationOnTarget:
                        #region ForceActivationOnTarget
                        if (request.Arguments.Count < 2)
                        {
                            result = false;
                            errorCode = ErrorCode.SuppliedArgumentIsNotEnough;
                        }
                        else
                        {
                            var ruleDefId = request.Arguments.FirstOrDefault(m => m.Name == "ruleDefId");
                            if (ruleDefId == null)
                            {
                                result = false;
                                errorCode = ErrorCode.RuleDefIdArgumentIsMissing;
                            }
                            else
                            {
                                var targetId = request.Arguments.FirstOrDefault(m => m.Name == "targetId");
                                if (targetId == null)
                                {
                                    result = false;
                                    errorCode = ErrorCode.TargetIdArgumentIsMissing;
                                }
                                else
                                {
                                    result = SchedulerService.ForceActivationOnTarget((long)ruleDefId.Value, (long)targetId.Value);
                                    if (!result)
                                        errorCode = ErrorCode.FailToForceActivationOnTarget;
                                }
                            }
                        }
                        break;
                    #endregion
                    case CommandName.ForceDeactivationOnTarget:
                        #region ForceDeactivationOnTarget
                        if (request.Arguments.Count < 2)
                        {
                            result = false;
                            errorCode = ErrorCode.SuppliedArgumentIsNotEnough;
                        }
                        else
                        {
                            var ruleDefId = request.Arguments.FirstOrDefault(m => m.Name == "ruleDefId");
                            if (ruleDefId == null)
                            {
                                result = false;
                                errorCode = ErrorCode.RuleDefIdArgumentIsMissing;
                            }
                            else
                            {
                                var targetId = request.Arguments.FirstOrDefault(m => m.Name == "targetId");
                                if (targetId == null)
                                {
                                    result = false;
                                    errorCode = ErrorCode.TargetIdArgumentIsMissing;
                                }
                                else
                                {
                                    result = SchedulerService.ForceDeactivationOnTarget((long)ruleDefId.Value, (long)targetId.Value);
                                    if (!result)
                                        errorCode = ErrorCode.FailToForceDeactivationOnTarget;
                                }
                            }
                        }
                        break;
                    #endregion
                    case CommandName.ForceSchedule:
                        #region ForceSchedule
                        if (request.Arguments.Count < 1)
                        {
                            result = false;
                            errorCode = ErrorCode.SuppliedArgumentIsNotEnough;
                        }
                        else
                        {
                            var ruleDefId = request.Arguments.FirstOrDefault(m => m.Name == "ruleDefId");
                            if (ruleDefId == null)
                            {
                                result = false;
                                errorCode = ErrorCode.RuleDefIdArgumentIsMissing;
                            }
                            else
                            {
                                result = SchedulerService.ForceSchedule((long)ruleDefId.Value);
                                if (!result)
                                    errorCode = ErrorCode.FailToForceSchedule;
                            }
                        }
                        break;
                    #endregion
                    case CommandName.IsTargetReachable:
                        #region IsTargetReachable
                        if (request.Arguments.Count < 1)
                        {
                            result = false;
                            errorCode = ErrorCode.SuppliedArgumentIsNotEnough;
                        }
                        else
                        {
                            var targetId = request.Arguments.FirstOrDefault(m => m.Name == "targetId");
                            if (targetId == null)
                            {
                                result = false;
                                errorCode = ErrorCode.TargetIdArgumentIsMissing;
                            }
                            else
                            {
                                result = SchedulerService.RuleExecuter.IsTargetReachable((long)targetId.Value);
                                if (!result)
                                    errorCode = ErrorCode.TargetIsNotReachable;
                            }
                        }
                        break;
                    #endregion
                    case CommandName.ConnectCore:
                        #region ConnectCore
                        if (request.Arguments.Count < 1)
                        {
                            result = false;
                            errorCode = ErrorCode.SuppliedArgumentIsNotEnough;
                        }
                        else
                        {
                            var targetId = request.Arguments.FirstOrDefault(m => m.Name == "targetId");
                            if (targetId == null)
                            {
                                result = false;
                                errorCode = ErrorCode.TargetIdArgumentIsMissing;
                            }
                            else
                            {
                                result = SchedulerService.RuleExecuter.ConnectCore((long)targetId.Value);
                                if (!result)
                                    errorCode = ErrorCode.FailToConnectCore;
                            }
                        }
                        break;
                    #endregion
                    case CommandName.DisconnectCore:
                        #region DisconnectCore
                        if (request.Arguments.Count < 1)
                        {
                            result = false;
                            errorCode = ErrorCode.SuppliedArgumentIsNotEnough;
                        }
                        else
                        {
                            var targetId = request.Arguments.FirstOrDefault(m => m.Name == "targetId");
                            if (targetId == null)
                            {
                                result = false;
                                errorCode = ErrorCode.TargetIdArgumentIsMissing;
                            }
                            else
                            {
                                result = SchedulerService.RuleExecuter.DisconnectCore((long)targetId.Value);
                                if (!result)
                                    errorCode = ErrorCode.FailToDisconnectCore;
                            }
                        }
                        break;
                        #endregion
                }
                var res = new CommandResponse { Status = (result ? ExecutionStatus.Success : ExecutionStatus.Fail), ErrorCode = errorCode, Message = ErrorCode.Description(errorCode) };
                writer.WriteLine(JsonConvert.SerializeObject(res));
                writer.Flush();
                reader.Close();
                writer.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Log.Error("an error occurred when command runner process new command", ex);
                var res = new CommandResponse { Status = ExecutionStatus.Fail, ErrorCode = ErrorCode.Unknown, Message = ErrorCode.Description(ErrorCode.Unknown) };
                var writer = new StreamWriter(client.GetStream());
                writer.WriteLine(JsonConvert.SerializeObject(res));
                writer.Flush();
                writer.Close();
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
        public ExecutionStatus Status { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }

    public class Argument
    {
        public string Name { get; set; }
        public object Value { get; set; }

    }
    public enum ExecutionStatus
    {
        Success,
        Fail
    }
    public enum CommandName
    {
       StartCapturing
    }
}
