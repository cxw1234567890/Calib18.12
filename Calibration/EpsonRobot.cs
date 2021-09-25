using AlarmLibrary;
using CommunicationLibrary;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Calibration
{
    /// <summary>
    /// 连接机器人的客户端类
    /// </summary>
    public class EpsonRobot
    {
       private TCPSocketClient tcpClient = new TCPSocketClient();

        public bool IsConnected { get { return isConnected; } }
        private bool isConnected = false;

        public bool IsLogin { get { return isLogin; } set { isLogin = value; } }
        private bool isLogin = false;

        private Stopwatch swKeepAlive = new Stopwatch();
        private Stopwatch swOutTime = new Stopwatch();
        private Stopwatch swExeEndTime = new Stopwatch();
        private ResponseStatus rs = new ResponseStatus();

        public Status Status { get { return status; } }
        private Status status = new Status();
        private string CEParam = string.Empty;
        private object lockER = new object();
        private Thread threadKeepAlive = null;
        public bool IsExeCMD { get { return isExeCMD; } set { isExeCMD = value; } }
        private bool isExeCMD = false;


        #region construction
        public EpsonRobot()
        {
            threadKeepAlive = new Thread(KeepAlive);
            threadKeepAlive.IsBackground = true;
            threadKeepAlive.Start();
        }
        #endregion

        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="msg"></param>
        private void LogRefresh(string msg)
        {
            NotifyG.Add(msg);
        }


        private void KeepAlive(object o)
        {
            while (true)
            {
                try
                {
                    //每隔15秒发送一次空数据
                    if (tcpClient.Connected && isLogin)
                    {
                        while (isExeCMD) { Thread.Sleep(1); }
                        ExecuteCMD(ERemotCMD.Login, "123");
                    }
                    //判断是否收到保活信息
                    Thread.Sleep(1000);
                    if (swKeepAlive.ElapsedMilliseconds < 2000)
                    {
                        isConnected = true;
                    }
                    else
                    {
                        isConnected = false;
                    }
                }
                catch (Exception ex)
                {
                    NotifyG.Add(ex.ToString());
                }
                Thread.Sleep(15000);
            }
        }
        public bool Init()
        {
            try
            {
                tcpClient.EventHandlerReceive += Receive_Data;

                if (!Login()) return false;
                //清除异常
                if (ExecuteCMD(ERemotCMD.Execute, Spel.Reset) != true) return false;
                //使能
                if (ExecuteCMD(ERemotCMD.SetMotorsOn, "0,1,2,3") != true) return false;
                //选择工具坐标
                if (ExecuteCMD(ERemotCMD.Execute, "Tool " + "3") != true) return false;
                //电源模式    
                if (ExecuteCMD(ERemotCMD.Execute, Spel.PowerLow) != true) return false;
                //速度
                if (ExecuteCMD(ERemotCMD.Execute, new string[2] { Spel.Speed,"100"}) != true) return false;
                if (ExecuteCMD(ERemotCMD.Execute, new string[2] { Spel.Speeds, "100" }) != true) return false;
                if (ExecuteCMD(ERemotCMD.Execute, new string[3] { Spel.Accel, "100", "100" }) != true) return false;
                if (ExecuteCMD(ERemotCMD.Execute, new string[3] { Spel.Accels, "100", "100" }) != true) return false;

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        //接收数据回调函数
        private void Receive_Data(string args)
        {
            try
            {
                if (ReceiveFilt(args)) return;
                LogRefresh(args);
                //登陆
                if (args.Contains("Login"))
                {
                    if (args.Contains("#Login"))
                    {
                        WResponseInfo(ERemotCMD.Login, true);
                        isLogin = true;
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.Login, false);
                    }
                }
                //登出
                else if (args.Contains("Logout"))
                {

                    if (args.Contains("#Logout"))
                    {
                        WResponseInfo(ERemotCMD.Logout, true);
                        isLogin = false;
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.Logout, false);
                    }
                }
                //执行函数
                else if (args.Contains("Start"))
                {
                    if (args == "#Start")
                    {
                        WResponseInfo(ERemotCMD.Start, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.Start, false);
                    }
                }
                //停止
                else if (args.Contains("Stop"))
                {
                    if (args == "#Stop")
                    {
                        WResponseInfo(ERemotCMD.Stop, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.Stop, false);
                    }
                }
                //暂停
                else if (args.Contains("Pause"))
                {
                    if (args== "#Pause")
                    {
                        WResponseInfo(ERemotCMD.Pause, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.Pause, false);
                    }
                }
                //继续
                else if (args.Contains("Continue"))
                {
                    if (args == "#Continue")
                    {
                        WResponseInfo(ERemotCMD.Continue, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.Continue, false);
                    }
                }
                //重置
                else if (args.Contains("Reset"))
                {
                    if (args == "#Reset")
                    {
                        WResponseInfo(ERemotCMD.Reset, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.Reset, false);
                    }
                }
                //开启机器人电机
                else if (args.Contains("SetMotorsOn"))
                {
                    if (args.Contains("#SetMotorsOn"))
                    {
                        WResponseInfo(ERemotCMD.SetMotorsOn, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.SetMotorsOn, false);
                    }
                }
                //关闭机器人电机
                else if (args.Contains("SetMotorsOff"))
                {
                    if (args.Contains("#SetMotorsOff"))
                    {
                        WResponseInfo(ERemotCMD.SetMotorsOff, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.SetMotorsOff, false);
                    }
                }
                //选择机器人
                else if (args.Contains("SetCurRobot"))
                {
                    if (args.Contains("#SetCurRobot"))
                    {
                        WResponseInfo(ERemotCMD.SetCurRobot, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.SetCurRobot, false);
                    }
                }
                //获取当前机器人编号
                else if (args.Contains("GetCurRobot"))
                {
                    if (args.Contains("#GetCurRobot"))
                    {
                        WResponseInfo(ERemotCMD.GetCurRobot, true);
                        status.CurrentRobot = Convert.ToInt32(args.Split(',')[1]);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.GetCurRobot, false);
                    }
                }
                //原点
                else if (args.Contains("Home"))
                {
                    if (args.Contains("#Home"))
                    {
                        WResponseInfo(ERemotCMD.Home, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.Home, false);
                    }
                }
                //获取指定的 I/O端口(8)
                else if (args.Contains("GetIOByte"))
                {
                    if (args.Contains("#GetIOByte"))
                    {
                        WResponseInfo(ERemotCMD.GetIOByte, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.GetIOByte, false);
                    }
                }
                //设置指定的 I/O端口
                else if (args.Contains("SetIO"))
                {
                    if (args.Contains("#SetIO"))
                    {
                        WResponseInfo(ERemotCMD.SetIO, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.SetIO, false);
                    }
                }
                //设置指定的 I/O端口(8)
                else if (args.Contains("SetIOByte"))
                {
                    if (args.Contains("#SetIOByte"))
                    {
                        WResponseInfo(ERemotCMD.SetIOByte, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.SetIOByte, false);
                    }
                }
                //获取指定的 I/O端口(16)
                else if (args.Contains("GetIOWord"))
                {
                    if (args.Contains("#GetIOWord"))
                    {
                        WResponseInfo(ERemotCMD.GetIOWord, true);
                        if (CEParam == "0") status.IoIn0 = Int16.Parse(args.Split(',')[1], NumberStyles.HexNumber);
                        if (CEParam == "1") status.IoIn1 = Int16.Parse(args.Split(',')[1], NumberStyles.HexNumber);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.GetIOWord, false);
                    }
                }
                //设置指定的 I/O端口(16)
                else if (args.Contains("SetIOWord"))
                {
                    if (args.Contains("#SetIOWord"))
                    {
                        WResponseInfo(ERemotCMD.SetIOWord, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.SetIOWord, false);
                    }
                }
                //获取指定的内存 I/O位
                else if (args.Contains("GetMemIO"))
                {
                    if (args.Contains("#GetMemIO"))
                    {
                        WResponseInfo(ERemotCMD.GetMemIO, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.GetMemIO, false);
                    }
                }
                //设置指定的内存 I/O位
                else if (args.Contains("SetMemIO"))
                {
                    if (args.Contains("#SetMemIO"))
                    {
                        WResponseInfo(ERemotCMD.SetMemIO, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.SetMemIO, false);
                    }
                }
                //获取指定的内存 I/O端口(8)
                else if (args.Contains("GetMemIOByte"))
                {
                    if (args.Contains("#GetMemIOByte"))
                    {
                        WResponseInfo(ERemotCMD.GetMemIOByte, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.GetMemIOByte, false);
                    }
                }
                //设置指定的内存 I/O端口(8)
                else if (args.Contains("SetMemIOByte"))
                {
                    if (args.Contains("#SetMemIOByte"))
                    {
                        WResponseInfo(ERemotCMD.SetMemIOByte, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.SetMemIOByte, false);
                    }
                }
                //获取指定的内存 I/O端口(16)
                else if (args.Contains("GetMemIOWord"))
                {
                    if (args.Contains("#GetMemIOWord"))
                    {
                        WResponseInfo(ERemotCMD.GetMemIOWord, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.GetMemIOWord, false);
                    }
                }
                //设置指定的内存 I/O端口(16)
                else if (args.Contains("SetMemIOWord"))
                {
                    if (args.Contains("#SetMemIOWord"))
                    {
                        WResponseInfo(ERemotCMD.SetMemIOWord, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.SetMemIOWord, false);
                    }
                }
                //获取参数值
                else if (args.Contains("SetVariable"))
                {
                    if (args.Contains("#SetVariable"))
                    {
                        WResponseInfo(ERemotCMD.SetVariable, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.SetVariable, false);
                    }
                }
                //获取状态
                else if (args.Contains("GetStatus"))
                {
                    if (args.Contains("#GetStatus"))
                    {
                        WResponseInfo(ERemotCMD.GetStatus, true);
                        ResolveControlInfo(args);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.GetStatus, false);
                    }
                }
                //执行命令
                else if (args.Contains("Execute"))
                {
                    if (args.Contains("#Execute"))
                    {
                        WResponseInfo(ERemotCMD.Execute, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.Execute, false);
                    }
                    if (RResponseInfo(ERemotCMD.Execute) == true)
                    {
                        //当前位置信息
                         if (CEParam.Contains(Spel.RealPos))
                        {
                            ResolveCurPos(args);
                        }                  
                    }
                }
                //中止命令
                else if (args.Contains("Abort"))
                {
                    if (args.Contains("#Abort"))
                    {
                        WResponseInfo(ERemotCMD.Execute, true);
                    }
                    else
                    {
                        WResponseInfo(ERemotCMD.Abort, false);
                    }
                }
                //非指令 接收点数据
                if (args.Contains("= XY("))
                {
                    ResolvePoint(args);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        //解析点信息 
        private bool ResolvePoint(string str)
        {
            try
            {
                PositionInfo pi = new PositionInfo();
                string[] value = str.Split(',');
                if (value.Count() > 4)
                {
                    pi.PID = value[1].Substring(1, 3);
                    pi.X = double.Parse(value[1].Substring(14, 7));
                    pi.Y = double.Parse(value[2].Substring(0, 9));
                    pi.Z = double.Parse(value[3].Substring(0, 9));
                    pi.U = double.Parse(value[4].Substring(0, 9));
                    pi.Hand = value[4].Contains("R") ? true : false;
                    pi.LocalNo = Convert.ToInt32(value[4].Substring(17, 1));
                    pi.Tage = value[4].Substring(19, value[4].Length - 19);
                }
                else
                {
                    pi.PID = value[0].Substring(0, 3);
                    pi.X = double.Parse(value[0].Substring(13, 6));
                    pi.Y = double.Parse(value[1].Substring(0, 9));
                    pi.Z = double.Parse(value[2].Substring(0, 9));
                    pi.U = double.Parse(value[3].Substring(0, 9));
                    pi.Hand = value[3].Contains("R") ? true : false;
                    pi.LocalNo = Convert.ToInt32(value[3].Substring(17, 1));
                    pi.Tage = value[3].Substring(19, value[3].Length - 19);
                }
                status.Points.Add(pi);
            }
            catch (Exception ex)
            {
                //NotifyG.Error(ex.ToString());
                return false;
            }
            return true;
        }

        private bool ResolveControlInfo(string str)
        {
            try
            {
                if (!str.Contains("#GetStatus")) return false;
                status.ControlInfo.ErrorCode = Convert.ToInt32(str.Split(',')[2]);
                str = str.Split(',')[1];
                if (str.Substring(0, 1) == "1")
                {
                    status.ControlInfo.Test = true;
                }
                else
                {
                    status.ControlInfo.Test = false;
                }
                if (str.Substring(1, 1) == "1")
                {
                    status.ControlInfo.Teach = true;
                }
                else
                {
                    status.ControlInfo.Teach = false;
                }

                if (str.Substring(2, 1) == "1")
                {
                    status.ControlInfo.Auto = true;
                }
                else
                {
                    status.ControlInfo.Auto = false;
                }
                if (str.Substring(3, 1) == "1")
                {
                    status.ControlInfo.Waring = true;
                }
                else
                {
                    status.ControlInfo.Waring = false;
                }
                if (str.Substring(4, 1) == "1")
                {
                    status.ControlInfo.SError = true;
                }
                else
                {
                    status.ControlInfo.SError = false;
                }
                if (str.Substring(5, 1) == "1")
                {
                    status.ControlInfo.Safeguard = true;
                }
                else
                {
                    status.ControlInfo.Safeguard = false;
                }
                if (str.Substring(6, 1) == "1")
                {
                    status.ControlInfo.EStop = true;
                }
                else
                {
                    status.ControlInfo.EStop = false;
                }
                if (str.Substring(7, 1) == "1")
                {
                    status.ControlInfo.Error = true;
                }
                else
                {
                    status.ControlInfo.Error = false;
                }
                if (str.Substring(8, 1) == "1")
                {
                    status.ControlInfo.Paused = true;
                }
                else
                {
                    status.ControlInfo.Paused = false;
                }
                if (str.Substring(9, 1) == "1")
                {
                    status.ControlInfo.Running = true;
                }
                else
                {
                    status.ControlInfo.Running = false;
                }
                if (str.Substring(10, 1) == "1")
                {
                    status.ControlInfo.Ready = true;
                }
                else
                {
                    status.ControlInfo.Ready = false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 解析当前位置
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool ResolveCurPos(string str)
        {
            try
            {
                string[] value = str.Split(',')[1].Split(':');
                status.PosInfo.X = double.Parse(value[1].Substring(0, 9).Replace(" ", ""));
                status.PosInfo.Y = double.Parse(value[2].Substring(0, 9).Replace(" ", ""));
                status.PosInfo.Z = double.Parse(value[3].Substring(0, 9).Replace(" ", ""));
                status.PosInfo.U = double.Parse(value[4].Substring(0, 9).Replace(" ", ""));
                status.PosInfo.V = double.Parse(value[5].Substring(0, 9).Replace(" ", ""));
                status.PosInfo.W = double.Parse(value[6].Substring(0, 9).Replace(" ", ""));
                status.PosInfo.Hand = value[6].Contains("R") ? true : false;
                status.PosInfo.LocalNo = Convert.ToInt32(value[6].Substring(6, value.Length - 6));
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        ///过滤响应信息
        private bool ReceiveFilt(string args)
        {
            if (args == "" || args == "\"")
            {
                swKeepAlive.Restart();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                tcpClient.Connect("192.168.0.1", 5000);
                Thread.Sleep(500);
                if (!tcpClient.Connected) { isConnected = false; return false; }
                isConnected = true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            try
            {
                tcpClient.Send(CreateCMD(ERemotCMD.Login, "123"));
                Thread.Sleep(500);
                if (IsLogin) { return true; } else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <returns></returns>
        public bool? ExecuteCMD(ERemotCMD cmd, params string[] param)
        {
            if (!tcpClient.Connected) { return null; }
            if (!isLogin) { return null; }
            lock (lockER)
            {
                try
                {
                    isExeCMD = true;
                    swOutTime.Restart();
                    //while (swExeEndTime.ElapsedMilliseconds < 12) { Thread.Sleep(1); }
                    WResponseInfo(cmd, null);
                    if (param.Count() > 0) CEParam = param[0];
                    tcpClient.Send(CreateCMD(cmd, param));
                    LogRefresh("发送"+CreateCMD(cmd, param));
                    while (RResponseInfo(cmd) == null)
                    {
                        if (swOutTime.ElapsedMilliseconds > 4000)
                        {
                            isExeCMD = false;
                            return false;
                        }
                        Thread.Sleep(1);
                    }
                    isExeCMD = false;
                    swExeEndTime.Restart();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return RResponseInfo(cmd);
        }

        /// <summary>
        /// 构造命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        private string CreateCMD(ERemotCMD cmd, params string[] args)
        {
            string strPara = string.Empty;
            if (cmd == ERemotCMD.Execute)
            {
                if (args.Length > 1)
                {
                    for (int i = 1; i < args.Length; i++)
                    {
                        strPara += args[i];
                        strPara += ',';
                    }
                    strPara = strPara.Substring(0, strPara.Length - 1);
                    return string.Format("${0},\"{1} {2}\"", cmd.ToString(), args[0], strPara);
                }
                else
                {
                    return string.Format("${0},\"{1}\"", cmd.ToString(), args[0]);
                }
            }
            else
            {
                foreach (var v in args)
                {
                    strPara += ",";
                    strPara += v;
                }
                return string.Format("${0}{1}", cmd.ToString(), strPara);
            }
        }

        /// <summary>
        /// 根据指定指令 读取应答状态
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <returns>状态：true=执行OK，false=执行false，null=超时</returns>
        private bool? RResponseInfo(ERemotCMD cmd)
        {
            try
            {
                switch (cmd)
                {
                    case ERemotCMD.Abort: return rs.Abort;
                    case ERemotCMD.Continue: return rs.Continue;
                    case ERemotCMD.Execute: return rs.Execute;
                    case ERemotCMD.GetCurRobot: return rs.GetCurRobot;
                    case ERemotCMD.GetIO: return rs.GetIO;
                    case ERemotCMD.GetIOByte: return rs.GetIOByte;
                    case ERemotCMD.GetIOWord: return rs.GetIOWord;
                    case ERemotCMD.GetMemIOByte: return rs.GetMemIOByte;
                    case ERemotCMD.GetMemIOWord: return rs.GetMemIOWord;
                    case ERemotCMD.GetStatus: return rs.GetStatus;
                    case ERemotCMD.GetVariable: return rs.GetVariable;
                    case ERemotCMD.Home: return rs.Home;
                    case ERemotCMD.Login: return rs.Login;
                    case ERemotCMD.Logout: return rs.Logout;
                    case ERemotCMD.Pause: return rs.Pause;
                    case ERemotCMD.Reset: return rs.Reset;
                    case ERemotCMD.SetCurRobot: return rs.SetCurRobot;
                    case ERemotCMD.SetIO: return rs.SetIO;
                    case ERemotCMD.SetIOByte: return rs.SetIOByte;
                    case ERemotCMD.SetIOWord: return rs.SetIOWord;
                    case ERemotCMD.SetMemIO: return rs.SetMemIO;
                    case ERemotCMD.SetMemIOByte: return rs.SetMemIOByte;
                    case ERemotCMD.SetMotorsOff: return rs.SetMotorsOff;
                    case ERemotCMD.SetMotorsOn: return rs.SetMotorsOn;
                    case ERemotCMD.SetVariable: return rs.SetVariable;
                    case ERemotCMD.Start: return rs.Start;
                    case ERemotCMD.Stop: return rs.Stop;
                    default: return null;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据指定指令 修改应答状态
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="value">状态：true=执行OK，false=执行false，null=超时</param>
        /// <returns>true=修改成功，false=修改失败</returns>
        private bool WResponseInfo(ERemotCMD cmd, bool? value)
        {
            try
            {
                switch (cmd)
                {
                    case ERemotCMD.Abort: rs.Abort = value; break;
                    case ERemotCMD.Continue: rs.Continue = value; break;
                    case ERemotCMD.Execute: rs.Execute = value; break;
                    case ERemotCMD.GetCurRobot: rs.GetCurRobot = value; break;
                    case ERemotCMD.GetIO: rs.GetIO = value; break;
                    case ERemotCMD.GetIOByte: rs.GetIOByte = value; break;
                    case ERemotCMD.GetIOWord: rs.GetIOWord = value; break;
                    case ERemotCMD.GetMemIOByte: rs.GetMemIOByte = value; break;
                    case ERemotCMD.GetMemIOWord: rs.GetMemIOWord = value; break;
                    case ERemotCMD.GetStatus: rs.GetStatus = value; break;
                    case ERemotCMD.GetVariable: rs.GetVariable = value; break;
                    case ERemotCMD.Home: rs.Home = value; break;
                    case ERemotCMD.Login: rs.Login = value; break;
                    case ERemotCMD.Logout: rs.Logout = value; break;
                    case ERemotCMD.Pause: rs.Pause = value; break;
                    case ERemotCMD.Reset: rs.Reset = value; break;
                    case ERemotCMD.SetCurRobot: rs.SetCurRobot = value; break;
                    case ERemotCMD.SetIO: rs.SetIO = value; break;
                    case ERemotCMD.SetIOByte: rs.SetIOByte = value; break;
                    case ERemotCMD.SetIOWord: rs.SetIOWord = value; break;
                    case ERemotCMD.SetMemIO: rs.SetMemIO = value; break;
                    case ERemotCMD.SetMemIOByte: rs.SetMemIOByte = value; break;
                    case ERemotCMD.SetMotorsOff: rs.SetMotorsOff = value; break;
                    case ERemotCMD.SetMotorsOn: rs.SetMotorsOn = value; break;
                    case ERemotCMD.SetVariable: rs.SetVariable = value; break;
                    case ERemotCMD.Start: rs.Start = value; break;
                    case ERemotCMD.Stop: rs.Stop = value; break;
                    default: return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public void DisConnect()
        {
            tcpClient.Close();
        }
    }
}
