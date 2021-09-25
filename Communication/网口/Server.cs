using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CommunicationLibrary
{
    public  class Server
    {
        public event Action<string> Receive;
        public event Action ClientRefresh;
        /// <summary>
        /// 获取已经连接的客户端集合
        /// </summary>
        public Dictionary<string, Socket> _dicSocket = new Dictionary<string, Socket>();
        public void Start(string Ip, string Port)
        {
            Socket socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(Ip), Convert.ToInt32(Port));
            socketWatch.Bind(point);
            socketWatch.Listen(10);
            ReceiveMes("连接成功");
            socketWatch.BeginAccept(new AsyncCallback(Listen), socketWatch);
        }

        private void Listen(IAsyncResult ar)
        {
            Socket socketWatch = (Socket)ar.AsyncState;
            try
            {
                Socket socketSend = socketWatch.EndAccept(ar);
                _dicSocket.Add(socketSend.RemoteEndPoint.ToString(), socketSend);
                Refresh();//刷新客户端列表
                State state = new State(socketSend);
                socketWatch.BeginAccept(new AsyncCallback(Listen), socketWatch);
                socketSend.BeginReceive(state.RecvBuffer, 0, state.RecvBuffer.Length, SocketFlags.None,
                                        new AsyncCallback(Recive), state);
            }
            catch (Exception ex)
            {
            }
        }
        void ReceiveMes(string str)
        {
            Receive?.Invoke(str);
        }
        private void Refresh()
        {
            ClientRefresh?.Invoke();
        }

        private void Recive(IAsyncResult ar)
        {
            try
            {
                int n = 0;
                State state = (State)ar.AsyncState;
                try
                {
                    n = state.Sock.EndReceive(ar);
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10054)
                    {
                        _dicSocket.Remove(state.Sock.RemoteEndPoint.ToString());
                        ReceiveMes(state.Sock.RemoteEndPoint.ToString() + " 下线");
                        state.Close();
                        Refresh();
                    }
                }
                if (n == 0)
                {
                    _dicSocket.Remove(state.Sock.RemoteEndPoint.ToString());
                    ReceiveMes(state.Sock.RemoteEndPoint.ToString() + " 下线");
                    state.Close();
                    Refresh();
                    return;
                }
                string str = Encoding.UTF8.GetString(state.RecvBuffer, 0, n);
                ReceiveMes(state.Sock.RemoteEndPoint.ToString() + " " + str);
                state.Sock.BeginReceive(state.RecvBuffer, 0, state.RecvBuffer.Length, SocketFlags.None,
                                            new AsyncCallback(Recive), state);
            }
            catch (Exception)
            {
            }
        }

        public void Send(Socket So, string str)
        {
            byte[] data = Encoding.Default.GetBytes(str);
            So.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendDataEnd), So);
        }
        private void SendDataEnd(IAsyncResult ar)
        {
            int r = ((Socket)ar.AsyncState).EndSend(ar);
        }
    }

    public class State
    {

        private byte[] _recvBuffer = new byte[1024 * 1024 * 2];
        public byte[] RecvBuffer
        {
            get { return _recvBuffer; }
        }

        private Socket _sock;
        public Socket Sock
        {
            get { return _sock; }
        }

        public State(Socket Sockt)
        {
            _sock = Sockt;
        }

        public void Close()
        {
            //关闭数据的接受和发送
            _sock.Shutdown(SocketShutdown.Both);
            //清理资源
            _sock.Close();
        }
    }
}
