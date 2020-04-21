using DeviceServer;
using Motor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using TestDevice;

namespace newdemoall
{
    public partial class Form1
    {
        BackgroundWorker bw = null;
        ITestBox testDevice = null;
        MotorDriver device = null;
        object deviceLock = new object();

        private void InitBW()
        {
            bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
            }
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            while (true)
            {
                if (bw.CancellationPending) break;
                Thread.Sleep(5);

                //如果没有指令，继续等待
                if (CommandsQueue.Count == 0)
                {
                    Thread.Sleep(50);
                    continue;
                }

                //从队列中将指令取出
                CommandInfo info = CommandsQueue.Dequeue() as CommandInfo;
                string value = info.Command.Trim('\0');
                //SetValue(value, info.Client.Client);

                //如果设备未连接，通知客户端
                if (testDevice == null || !testDevice.IsOpen)
                {
                    ServerResultMessage _ServerResultMessage = new ServerResultMessage()
                    {
                        CommandType = CommandType.None,
                        ResultCode = ResultCode.fail,
                        Data = "device is disconnect.",
                    };
                    SendData(info, JsonConvert.SerializeObject(_ServerResultMessage));
                    Thread.Sleep(1000);
                    continue;
                }

                ResetType result = ResetType.None;
                Start:
                try
                {
                    Command(value, info);
                }
                catch (MotorException ex)
                {
                    Loghelper.WriteLog("发送指令:",ex);
                    switch (result)
                    {
                        case ResetType.None:
                            lock (deviceLock)
                                result = device.ResetComUSB();
                            if (result == ResetType.USBSuccess || result == ResetType.Success)
                            {
                                goto Start;
                            }
                            break;
                        case ResetType.USBSuccess:
                            lock (deviceLock)
                                result = device.ResetCom();
                            if (result == ResetType.Success)
                            {
                                goto Start;
                            }
                            break;
                        case ResetType.Error:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Loghelper.WriteLog("下发错误",ex);
                    ServerResultMessage _ServerResultMessage = new ServerResultMessage()
                    {
                        CommandType = CommandType.None,
                        ResultCode = ResultCode.fail,
                        Data = ex.Message,
                    };
                    SendData(info, JsonConvert.SerializeObject(_ServerResultMessage));
                }
                //SetValue(string.Format("队列还剩【{0}】条指令未下发", CommandsQueue.Count), null);
            }
        }

        private void Command(string value, CommandInfo info)
        {
            ServerResultMessage _ServerResultMessage = new ServerResultMessage();
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(value);
            string messageId = string.Empty;
            if (dynamicObject.messageId != null)
                _ServerResultMessage.messageId = dynamicObject.messageId;
            CommandType _CommandType = (CommandType)dynamicObject.commandType;
            string resultMessageStr = "ok";
            double x = 0, y = 0, z = 0;
            switch (_CommandType)
            {
                case CommandType.Pressure:
                    //压力校准代码
                    this.testDevice.StylusMoveZ(0.2);
                    this.testDevice.Pressure();
                    break;
                case CommandType.Open:
                    //压力校准代码
                    //this.testDevice.StylusMoveZ(0.2);
                    //this.testDevice.Pressure();
                    break;
            }
            _ServerResultMessage.CommandType = _CommandType;
            _ServerResultMessage.ResultCode = ResultCode.Success;
            _ServerResultMessage.Data = resultMessageStr;
            SendData(info, JsonConvert.SerializeObject(_ServerResultMessage));
        }
    }
}
