using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calibration
{
    /// <summary>
    /// 控制器状态
    /// </summary>
    public class ControlInfo
    {
        /// <summary>
        /// 测试
        /// </summary>
        public bool Test { get; set; }
        /// <summary>
        /// 示教
        /// </summary>
        public bool Teach { get; set; }
        /// <summary>
        /// 自动
        /// </summary>
        public bool Auto { get; set; }
        /// <summary>
        /// 报警
        /// </summary>
        public bool Waring { get; set; }
        /// <summary>
        /// 严重错误
        /// </summary>
        public bool SError { get; set; }
        /// <summary>
        /// 安全保护
        /// </summary>
        public bool Safeguard { get; set; }
        /// <summary>
        /// 急停
        /// </summary>
        public bool EStop { get; set; }
        /// <summary>
        /// 错误
        /// </summary>
        public bool Error { get; set; }
        /// <summary>
        /// 暂停
        /// </summary>
        public bool Paused { get; set; }
        /// <summary>
        /// 运行中
        /// </summary>
        public bool Running { get; set; }
        /// <summary>
        /// 准备好
        /// </summary>
        public bool Ready { get; set; }
        /// <summary>
        /// 获取状态时 错误警告编码
        /// </summary>
        public int ErrorCode { get { return erroeCode; } set { erroeCode = value; } }
        private int erroeCode = 0;
    }
}
