namespace Calibration
{
    public class ResponseStatus
    {
        /// <summary>
        ///已响应登陆命令
        /// </summary>
        public bool? Login { get; set; }
        /// <summary>
        /// 已响应登出命令
        /// </summary>
        public bool? Logout { get; set; }
        /// <summary>
        /// 已响应Start命令
        /// </summary>
        public bool? Start { get; set; }
        /// <summary>
        /// 已响应Stop命令
        /// </summary>
        public bool? Stop { get; set; }
        /// <summary>
        /// 已响应Paus命令
        /// </summary>
        public bool? Pause { get; set; }
        /// <summary>
        /// 已响应Continue命令
        /// </summary>
        public bool? Continue { get; set; }
        /// <summary>
        /// 已响应Reset命令
        /// </summary>
        public bool? Reset { get; set; }
        /// <summary>
        /// 已响应SetMotorsO命令
        /// </summary>
        public bool? SetMotorsOn { get; set; }
        /// <summary>
        /// 已响应SetMotorsOf命令
        /// </summary>
        public bool? SetMotorsOff { get; set; }
        /// <summary>
        /// 已响应SetCurRobo命令
        /// </summary>
        public bool? SetCurRobot { get; set; }
        /// <summary>
        /// 已响应GetCurRobot命令
        /// </summary>
        public bool? GetCurRobot { get; set; }
        /// <summary>
        /// 已响应Home命令
        /// </summary>
        public bool? Home { get; set; }
        /// <summary>
        /// 已响应GetIO命令
        /// </summary>
        public bool? GetIO { get; set; }
        /// <summary>
        /// 已响应SetIO命令
        /// </summary>
        public bool? SetIO { get; set; }
        /// <summary>
        /// 已响应GetIOByte命令
        /// </summary>
        public bool? GetIOByte { get; set; }
        /// <summary>
        ///  已响应SetIOByte命令
        /// </summary>
        public bool? SetIOByte { get; set; }
        /// <summary>
        /// 已响应GetIOWord命令
        /// </summary>
        public bool? GetIOWord { get; set; }
        /// <summary>
        ///  已响应SetIOWord命令
        /// </summary>
        public bool? SetIOWord { get; set; }
        /// <summary>
        /// 已响应GetMemIO命令
        /// </summary>
        public bool? GetMemIO { get; set; }
        /// <summary>
        /// 已响应SetMemIO命令
        /// </summary>
        public bool? SetMemIO { get; set; }
        /// <summary>
        /// 已响应GetMemIOByte命令
        /// </summary>
        public bool? GetMemIOByte { get; set; }
        /// <summary>
        /// 已响应SetMemIOByte命令
        /// </summary>
        public bool? SetMemIOByte { get; set; }
        /// <summary>
        /// 已响应GetMemIOWord命令
        /// </summary>
        public bool? GetMemIOWord { get; set; }
        /// <summary>
        /// 已响应SetMemIOWord命令
        /// </summary>
        public bool? SetMemIOWord { get; set; }
        /// <summary>
        /// 已响应GetVariable命令
        /// </summary>
        public bool? GetVariable { get; set; }
        /// <summary>
        ///已响应SetVariable命令
        /// </summary>
        public bool? SetVariable { get; set; }
        /// <summary>
        /// 已响应GetStatus命令
        /// </summary>
        public bool? GetStatus { get; set; }
        /// <summary>
        /// 已响应Execute命令
        /// </summary>
        public bool? Execute { get; set; }
        /// <summary>
        /// 已响应Abort命令
        /// </summary>
        public bool? Abort { get; set; }
    }
}
