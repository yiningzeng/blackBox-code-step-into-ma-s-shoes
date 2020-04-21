using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DeviceServer
{
    public class TcpServer
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int bufferSize = 1024 * 1024;
        private TcpListener listner;
        private volatile bool IStop;
        //接收数据
        public delegate void ReceiveHandler(CommandInfo info);
        public event ReceiveHandler ReceiveEvent;
        //错误事件
        public delegate void ErrorHandler(string msg);
        public event ErrorHandler ErrorEvent;
        public event EventHandler ConnectEvent;

        public void Start(string host, int port)
        {
            IStop = false;
            Action<string, int> action = Server;
            action.BeginInvoke(host, port, null, null);
        }
        public void Stop()
        {
            if (listner != null)
                listner.Stop();
            IStop = true;
        }
        /// <summary>
        /// 服务器端
        /// </summary>
        private void Server(string host, int port)
        {
            try
            {
                //1.监听端口
                listner = new TcpListener(IPAddress.Parse(host), port);
                listner.Start();
                log.ErrorFormat("监听端口{0}...", port);

                //2.等待请求
                while (!IStop)
                {
                    try
                    {
                        //2.1 收到请求
                        TcpClient client = listner.AcceptTcpClient(); //停在这等待连接请求
                        Action<TcpClient> action = Recive;
                        action.BeginInvoke(client, null, null);
                    }
                    catch (Exception ex)
                    {
                        Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
        void ReadBack(IAsyncResult iar)
        {
            try
            {
                BufferState state = (BufferState)iar.AsyncState;
                NetworkStream stream = state.Client.GetStream();
                int length = stream.EndRead(iar);
                if (length > 0)
                {
                    string data = Encoding.UTF8.GetString(state.Buffer, 0, length);
                    OnReceive(new CommandInfo(state.Client, data));
                }
                else
                {
                    Thread.Sleep(10);
                    return;
                }
                if (state.Client.Connected)
                {
                    state.Buffer = new byte[bufferSize];
                    stream.BeginRead(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(ReadBack), state);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void Recive(TcpClient client)
        {
            IPEndPoint ipendpoint = client.Client.RemoteEndPoint as IPEndPoint;
            NetworkStream stream = client.GetStream();

            if (ConnectEvent != null)
            {
                ConnectEvent(client, EventArgs.Empty);
            }
            //2.2 解析数据,长度<bufferSize字节
            byte[] buffer = new byte[bufferSize];
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadBack), new BufferState(client, buffer));

            //2.3 返回状态
            //byte[] messages = Encoding.Default.GetBytes("ok.");
            //stream.Write(messages, 0, messages.Length);

            //2.4 关闭客户端
            //stream.Close();
            //client.Close();
        }
        private void Error(Exception ex)
        {
            if (IStop) return;
            log.Error(ex);
            if (ex is SocketException)
            {
                SocketException socketEx = ex as SocketException;
                OnError(string.Format("{0}\r\n{1}", ErrorCode(socketEx.ErrorCode), socketEx.Message));
            }
            else
            {
                OnError(ex.Message);
            }
        }
        private void OnReceive(CommandInfo info)
        {
            try
            {
                if (ReceiveEvent != null)
                    ReceiveEvent(info);
            }
            catch { }
        }
        private void OnError(string msg)
        {
            try
            {
                if (ErrorEvent != null)
                    ErrorEvent(msg);
            }
            catch { }
        }

        /// <summary>
        /// 客户端
        /// </summary>
        public void Client(string ip, int port, string message)
        {
            try
            {
                //1.发送数据                
                TcpClient client = new TcpClient(ip, port);
                IPEndPoint ipendpoint = client.Client.RemoteEndPoint as IPEndPoint;
                NetworkStream stream = client.GetStream();
                byte[] messages = Encoding.UTF8.GetBytes(message);
                stream.Write(messages, 0, messages.Length);

                //2.接收状态,长度<1024字节
                byte[] bytes = new Byte[bufferSize];
                string data = string.Empty;
                int length = stream.Read(bytes, 0, bytes.Length);
                if (length > 0)
                {
                    data = Encoding.UTF8.GetString(bytes, 0, length);
                    Console.WriteLine("{0:HH:mm:ss}->接收数据(from {1}:{2})：{3}", DateTime.Now, ipendpoint.Address, ipendpoint.Port, data);
                }

                //3.关闭对象
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
        public string ErrorCode(int code)
        {
            switch (code)
            {
                case 10004: return "The operation is canceled.";
                case 10013: return "The requested address is a broadcast address, but flag is not set.";
                case 10014: return "Invalid argument.";
                case 10022: return "Socket not bound, invalid address or listen is not invoked prior to accept.";
                case 10024: return "No more file descriptors are available, accept queue is empty.";
                case 10035: return "Socket is non-blocking and the specified operation will block.";
                case 10036: return "A blocking Winsock operation is in progress.";
                case 10037: return "The operation is completed.No blocking operation is in progress.";
                case 10038: return "The descriptor is not a socket.";
                case 10039: return "Destination address is required.";
                case 10040: return "The datagram is too large to fit into the buffer and is truncated.";
                case 10041: return "The specified port is the wrong type for this socket.";
                case 10042: return "Option unknown, or unsupported.";
                case 10043: return "The specified port is not supported.";
                case 10044: return "Socket type not supported in this address family.";
                case 10045: return "Socket is not a type that supports connection oriented service.";
                case 10047: return "Address Family is not supported.";
                case 10048: return "Address in use.";
                case 10049: return "Address is not available from the local machine.";
                case 10050: return "Network subsystem failed.";
                case 10051: return "The network cannot be reached from this host at this time.";
                case 10052: return "Connection has timed out when SO_KEEPALIVE is set.";
                case 10053: return "Connection is aborted due to timeout or other failure.";
                case 10054: return "The connection is reset by remote side.";
                case 10055: return "No buffer space is available.";
                case 10056: return "Socket is already connected.";
                case 10057: return "Socket is not connected.";
                case 10058: return "Socket has been shut down.";
                case 10060: return "The attempt to connect timed out.";
                case 10061: return "Connection is forcefully rejected.";
                case 10201: return "Socket already created for this object.";
                case 10202: return "Socket has not been created for this object.";
                case 11001: return "Authoritative answer: Host not found.";
                case 11002: return "Non-Authoritative answer: Host not found.";
                case 11003: return "Non-recoverable errors.";
                case 11004: return "Valid name, no data record of requested type.";
                default: return null;
            }
        }
    }
    public class BufferState
    {
        public TcpClient Client { get; set; }
        public byte[] Buffer { get; set; }

        public BufferState(TcpClient client, byte[] buffer)
        {
            this.Client = client;
            this.Buffer = buffer;
        }
    }
    public class CommandInfo
    {
        public TcpClient Client { get; set; }
        public string Command { get; set; }

        public void SendData(string value)
        {
            try
            {
                if (Client.Connected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(value);
                    Client.GetStream().Write(buffer, 0, buffer.Length);
                    System.Threading.Thread.Sleep(20);
                }
            }
            catch { }
        }

        public CommandInfo(TcpClient client, string command)
        {
            this.Client = client;
            this.Command = command;
        }
    }
}
