using DeviceServer;
using newdemoall.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace newdemoall
{
    public partial class Form1
    {
        private Queue CommandsQueue = new Queue();
        private TcpServer server;

        private void InitServer()
        {
            string host = Settings.Default.IP;
            if (string.IsNullOrEmpty(host))
                host = Methoded.GetIpAddress();
            if (server != null)
                server.Stop();
            //连接网络
            server = new TcpServer();
            server.ErrorEvent += server_ErrorEvent;
            server.ReceiveEvent += server_ReceiveEvent;
            server.ConnectEvent += server_ConnectEvent;
            server.Start(host, Settings.Default.Port);
            //SetValue2("监听端口 {0}:{1}...", host, Settings.Default.Port);
        }
        void server_ErrorEvent(string msg)
        {
            if (!this.Visible) return;
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new TcpServer.ErrorHandler(server_ErrorEvent), msg);
                    return;
                }
                MessageBox.Show(this, msg, Config.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Loghelper.WriteLog("",ex);
            }
        }
        void server_ReceiveEvent(CommandInfo info)
        {
            CommandsQueue.Enqueue(info);
            //SetValue(info.Command, null);
        }
        void server_ConnectEvent(object sender, EventArgs e)
        {
        }

        private void SendData(CommandInfo info, string desc)
        {
            //SetValue(desc, info.Client.Client);
            info.SendData(desc);
        }

    }
}
