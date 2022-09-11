using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Text;

namespace Overlayer
{
    public static class SocketHelper
    {
        public static Socket New(ProtocolType protocolType = ProtocolType.Tcp, int bindPort = -1)
        {
            switch (protocolType)
            {
                case ProtocolType.Tcp:
                    return new Socket(AddressFamily.InterNetwork, SocketType.Stream, protocolType);
                case ProtocolType.Udp:
                    var soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, protocolType);
                    if (bindPort != -1)
                        soc.BindAny(bindPort);
                    return soc;
                default:
                    throw new NotSupportedException("SocketHelper.New() Method Only Supports Tcp and Udp.");
            }
        }
        public static string Request(this Socket socket, string header, Encoding encoding = null, int bufferSize = 1024)
        {
            socket.Send(header, encoding);
            return socket.Receive(encoding, bufferSize);
        }
        public static IPEndPoint GetIP(this Socket socket)
            => (IPEndPoint)socket.RemoteEndPoint;
        public static void BindAny(this Socket socket, int port = 0)
            => socket.Bind(new IPEndPoint(IPAddress.Any, port));
        public static int Send(this Socket socket, string content, Encoding encoding = null)
            => socket.Send((encoding ?? Encoding.UTF8).GetBytes(content));
        public static string Receive(this Socket socket, Encoding encoding = null, int bufferSize = 1024)
        {
            Receive(socket, out string str, encoding, bufferSize);
            return str;
        }
        public static int Receive(this Socket socket, out string data, Encoding encoding = null, int bufferSize = 1024)
        {
            byte[] buffer = new byte[bufferSize];
            int size = socket.Receive(buffer);
            data = (encoding ?? Encoding.UTF8).GetString(buffer);
            return size;
        }
        public static int SendTo(this Socket socket, string content, EndPoint endPoint, Encoding encoding = null)
            => socket.SendTo((encoding ?? Encoding.UTF8).GetBytes(content), endPoint);
        public static string ReceiveFrom(this Socket socket, ref EndPoint endPoint, Encoding encoding = null, int bufferSize = 1024)
        {
            ReceiveFrom(socket, out string str, ref endPoint, encoding, bufferSize);
            return str;
        }
        public static int ReceiveFrom(this Socket socket, out string data, ref EndPoint endPoint, Encoding encoding = null, int bufferSize = 1024)
        {
            byte[] buffer = new byte[bufferSize];
            int size = socket.ReceiveFrom(buffer, ref endPoint);
            data = (encoding ?? Encoding.UTF8).GetString(buffer);
            return size;
        }
        public static string ReceiveFrom(this Socket socket, EndPoint endPoint, Encoding encoding = null, int bufferSize = 1024)
        {
            ReceiveFrom(socket, out string str, ref endPoint, encoding, bufferSize);
            return str;
        }
        public static int ReceiveFrom(this Socket socket, out string data, EndPoint endPoint, Encoding encoding = null, int bufferSize = 1024)
        {
            byte[] buffer = new byte[bufferSize];
            int size = socket.ReceiveFrom(buffer, ref endPoint);
            data = (encoding ?? Encoding.UTF8).GetString(buffer);
            return size;
        }
        public static void OpenServer(this Socket socket, int port, int backlog = 20)
        {
            socket.BindAny(port);
            socket.Listen(backlog);
        }
        public static void CloseAll(this Socket socket, bool reuseSocket = true)
        {
            if (socket.Connected)
                socket.Disconnect(reuseSocket);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        public static void OpenServer(this Socket socket, int port, Action<Socket> onAccept, Action<Socket, byte[]> onReceive, int backlog = 20)
        {
            var acceptor = socket.GetTcpAcceptor();
            if (onAccept != null)
                acceptor.OnAccept += onAccept;
            if (onReceive != null)
                acceptor.OnReceive += onReceive;
            socket.OpenServer(port, backlog);
            acceptor.Start();
        }
        public static void Connect(this Socket socket, IPAddress ip, int port, Action<byte[]> onReceive)
        {
            var receiver = socket.GetTcpReceiver();
            if (onReceive != null)
                receiver.OnReceive += onReceive;
            socket.Connect(ip, port);
            receiver.Start();
        }
        public static NetworkStream GetStream(this Socket socket)
            => new NetworkStream(socket);
        public static TcpAcceptor GetTcpAcceptor(this Socket socket)
            => new TcpAcceptor(socket);
        public static TcpReceiver GetTcpReceiver(this Socket socket)
            => new TcpReceiver(socket);
        public static UdpReceiver GetUdpReceiver(this Socket socket)
           => new UdpReceiver(socket);
    }
    public class TcpReceiver
    {
        public Socket socket;
        public int receiveBuffer;
        public virtual event Action<byte[]> OnReceive = delegate { };
        private Thread thread;
        public TcpReceiver(Socket socket, int receiveBuffer = 1024)
        {
            this.socket = socket;
            this.receiveBuffer = receiveBuffer;
            thread = new Thread(ReceiveHandler);
        }
        public void Start() => thread.Start();
        public void Stop() => thread.Abort();
        protected virtual void ReceiveHandler()
        {
            while (socket.Connected)
            {
                byte[] buffer = new byte[receiveBuffer];
                try
                {
                    if (socket.Receive(buffer) > 0)
                        OnReceive(buffer);
                }
                catch (SocketException) { }
            }
        }
    }
    public class UdpReceiver
    {
        public Socket socket;
        public int receiveBuffer;
        public virtual event Action<EndPoint, byte[]> OnReceive = delegate { };
        private Thread thread;
        public UdpReceiver(Socket socket, int receiveBuffer = 1024)
        {
            this.socket = socket;
            this.receiveBuffer = receiveBuffer;
            thread = new Thread(ReceiveHandler);
        }
        public static EndPoint Any => new IPEndPoint(IPAddress.Any, 0);
        public void Start() => thread.Start();
        public void Stop() => thread.Abort();
        private void ReceiveHandler()
        {
            while (true)
            {
                byte[] buffer = new byte[receiveBuffer];
                try
                {
                    var any = Any;
                    if (socket.ReceiveFrom(buffer, ref any) > 0)
                        OnReceive(any, buffer);
                }
                catch (SocketException) { }
            }
        }
    }
    public class TcpAcceptor
    {
        public Socket socket;
        public int receiveBuffer;
        public event Action<Socket> OnAccept = delegate { };
        public event Action<Socket, byte[]> OnReceive = delegate { };
        private Thread thread;
        public TcpAcceptor(Socket socket, int receiveBuffer = 1024)
        {
            this.socket = socket;
            this.receiveBuffer = receiveBuffer;
            thread = new Thread(AcceptHandler);
        }
        public void Start() => thread.Start();
        public void Stop() => thread.Abort();
        private void AcceptHandler()
        {
            while (true)
            {
                Socket client = socket.Accept();
                OnAccept(client);
                new Task(() =>
                {
                    while (client.Connected)
                    {
                        byte[] buffer = new byte[receiveBuffer];
                        if (client.Receive(buffer) > 0)
                            OnReceive(client, buffer);
                    }
                }).Start();
            }
        }
    }
}