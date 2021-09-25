using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CommunicationLibrary
{
    public class TCPSocketClient
    {
        #region Field
        public Socket _clientSocket;
        private byte[] _dataBuffer = new byte[1024 * 10];
        /// <summary>
        /// 获取已经连接的服务器
        /// </summary>
        public string RemotePoint
        {
            get
            {
                if (_clientSocket != null)
                { return _clientSocket.RemoteEndPoint.ToString(); }
                else
                { return ""; }
            }
        }

        #endregion

        public int TimeOut { get { return _timeOut; } set { _timeOut = value; } }
        private int _timeOut = 0;

        public bool Connected
        {
            get
            {
                if (_clientSocket == null) return false;
                return _clientSocket.Connected;
            }
        }

        public bool Connect(string ip,int port)
        {
            try
            {           
                if (_clientSocket != null) { if (_clientSocket.Connected) return true; }//如果上一次已经连接上了
                IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip),port);
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _clientSocket.SendTimeout = _timeOut;
                _clientSocket.ReceiveBufferSize = 1024 * 10;
                _clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                _clientSocket.BeginConnect(point, new AsyncCallback(CallBackConnect), _clientSocket);
            }
            catch (Exception ex)
            {
            }
            return true;
        }

        /// <summary>
        /// 当连接服务器之后的回调函数
        /// </summary>
        /// <param name="asy"></param>
        private void CallBackConnect(IAsyncResult asy)
        {
            try
            {
                if (!_clientSocket.Connected) return;
                _clientSocket.EndConnect(asy);
                _clientSocket.BeginReceive(_dataBuffer, 0, _dataBuffer.Length, SocketFlags.None, new
                    AsyncCallback(CallBackRecieve), null);
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 当接收到数据之后的回调函数
        /// </summary>
        /// <param name="iar"></param>
        private void CallBackRecieve(IAsyncResult iar)
        {
            try
            {
                int count = _clientSocket.EndReceive(iar);//count为已经接收到的字节数
                string receiveString = Encoding.Default.GetString(_dataBuffer, 0, count);
                //将处理的数据发送出去
                this.OnReceiveData(receiveString);
                //继续监听下一次的数据接收
                if (_clientSocket.Connected)
                {
                    _clientSocket.BeginReceive(_dataBuffer, 0, _dataBuffer.Length, SocketFlags.None, new AsyncCallback(CallBackRecieve), _clientSocket);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public event Action<string>  EventHandlerReceive;

        private void OnReceiveData(string msg)
        {
            if (EventHandlerReceive != null)
            {
                EventHandlerReceive(msg);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="str"></param>
        public bool Send(string str)
        {
            try
            {
                if (_clientSocket == null) {return false; }
                if (!_clientSocket.Connected) {  return false; }
                byte[] buffer = Encoding.Default.GetBytes(str + "\r\n");
                _clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(CallBackSend), _clientSocket);
            }
            catch (Exception ex)
            {
                this.Close();
                return false;
            }
            return false;
        }

        private void CallBackSend(IAsyncResult iar)
        {
            try
            {
                int t = ((Socket)iar.AsyncState).EndSend(iar);//t为已经发送的字节数
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            try
            {
                if (_clientSocket != null)
                {
                    if (_clientSocket.Connected)
                    {
                        _clientSocket.Shutdown(SocketShutdown.Both);
                    }                  
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                _clientSocket.Close();
            }
        }
    }
}
