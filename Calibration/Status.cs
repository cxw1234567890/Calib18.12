using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calibration
{
    public class Status
    {
        /// <summary>
        /// 当前机器人
        /// </summary>
        public int CurrentRobot { get; set; }

        /// <summary>
        /// 控制器状态
        /// </summary>
        public ControlInfo ControlInfo { get { return controlInfo; } set { controlInfo = value; } }
        private ControlInfo controlInfo = new ControlInfo();

        /// <summary>
        /// 机器人状态
        /// </summary>
        public RobotInfo RobotInfo { get { return robotInfo; } set { robotInfo = value; } }
        private RobotInfo robotInfo = new RobotInfo();

        /// <summary>
        /// 当前位置信息
        /// </summary>
        public PositionInfo PosInfo { get { return posInfo; } set { posInfo = value; } }
        private PositionInfo posInfo = new PositionInfo();

        /// <summary>
        /// 点数据
        /// </summary>
        public List<PositionInfo> Points { get { return points; } set { points = value; } }
        private List<PositionInfo> points = new List<PositionInfo>();

        /// <summary>
        /// 起始点位置信息
        /// </summary>
        public int[] HomeSet { get; set; }

        /// <summary>
        /// 回起始位置 各轴动作次序
        /// </summary>
        public int[] Hordr { get; set; }
        /// <summary>
        /// IO状态 第0字节
        /// </summary>
        public Int16 IoIn0 { get; set; }
        /// <summary>
        /// IO状态 第1字节
        /// </summary>
        public Int16 IoIn1 { get; set; }
    }
}
