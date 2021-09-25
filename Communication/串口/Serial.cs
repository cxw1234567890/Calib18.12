using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;

namespace Communication
{
    public class Serial
    {
        #region//字段和属性
        /// <summary>
        /// 串口对象
        /// </summary>
        private SerialPort _sP = new SerialPort();

        private string _portName;
        /// <summary>
        /// 串口名称如“COM1”
        /// </summary>
        [DefaultValue("COM1")]
        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }
        /// <summary>
        /// 停止位
        /// </summary>
        [DefaultValue(StopBits.One)]
        private StopBits _stopBit;
        public StopBits StopBit
        {
            get { return _stopBit; }
            set { _stopBit = value; }
        }
        /// <summary>
        ///波特率
        /// </summary>
        [DefaultValue(9600)]
        private int _baudRate;
        public int BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }
        /// <summary>
        /// 数据位
        /// </summary>
        [DefaultValue(StopBits.One)]
        private int _dataBits;
        public int DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }
        /// <summary>
        ///奇偶校验
        /// </summary>
        [DefaultValue(Parity.None)]
        private Parity _parityCheck;
        public Parity ParityCheck
        {
            get { return _parityCheck; }
            set { _parityCheck = value; }
        }
        /// <summary>
        /// 串口是否已打开
        /// </summary>
        [DefaultValue(false)]
        public bool IsOpened
        {
            get {return _sP.IsOpen; }
        }
        /// <summary>
        /// 是否以16进制显示
        /// </summary>
        [DefaultValue(false)]
        private bool _isHex;
        public bool IsHex
        {
            get { return _isHex; }
            set { _isHex = value; }
        }
        #endregion

        public event Action<string> ReceiveData;

        #region//打开串口
        public void OpenCom()
        {
           
            if (!_sP.IsOpen)
            {
                try
                {
                    Set_Parameters();
                    _sP.Open();
                }
                catch (Exception)
                {
                    _sP.Dispose();
                }
            }
        }
        #endregion

        private void Set_Parameters()
        {
            _sP.PortName = _portName;
            _sP.StopBits = _stopBit;
            _sP.BaudRate = _baudRate;
            _sP.DataBits = _dataBits;
            _sP.Parity = _parityCheck;
            _sP.ReadTimeout = -1;
            _sP.RtsEnable = true;
            _sP.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_isHex)
            {
                {
                    Byte[] ReceivedData = new Byte[_sP.BytesToRead];
                    _sP.Read(ReceivedData, 0, ReceivedData.Length);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < ReceivedData.Length; i++)
                    {
                        sb.AppendFormat("{0:X2}", ReceivedData[i]);
                    }
                    Perform_event("0X" + sb + "\r\n");           
                }
            }
            else
            {
                //Byte[] ReceivedData = new Byte[_sP.BytesToRead];
                //_sP.Read(ReceivedData, 0, ReceivedData.Length);
                //StringBuilder sb = new StringBuilder();
                //sb.Append(Encoding.UTF8.GetString(ReceivedData, 0, ReceivedData.Length));
                //(sb + "\r\n");  
                Perform_event(_sP.ReadExisting() + "\r\n");
            }
        }

        /// <summary>
        /// 将收到的数据发送出去
        /// </summary>
        private void Perform_event(string s)
        {
            if (ReceiveData != null)
            {
                ReceiveData(s);
            }
        }
        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="s">要发送的字符串</param>
        public void SendData(string s)
        {
            if (_sP.IsOpen)
            {
                try
                {
                    {
                        _sP.Write(s);
                    }
                }
                catch (Exception)
                {
                    //"发送数据错误！", "错误提示";
                }
            }
            else
            {
                //串口未打开
            }
        }
    }
}
