using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calibration
{
    public class RobotInfo
    {
        /// <summary>
        /// 机器人电机使能 true=On,false=off
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 功率 true=high fales=low
        /// </summary>
        public bool Power { get; set; }
        /// <summary>
        /// Halt状态 true=Halt状态 false=非Halt状态
        /// </summary>
        public bool Halt { get; set; }
        /// <summary>
        /// 机器人处于原点位置
        /// </summary>
        public bool Home { get; set; }
        /// <summary>
        /// 运动中
        /// </summary>
        public bool Running { get; set; }
    }
}
