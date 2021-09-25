using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calibration
{
    public class PositionInfo
    {
        /// <summary>
        /// 坐标X
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 坐标Y
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 坐标Z
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// 角度U(deg)
        /// </summary>
        public double U { get; set; }

        /// <summary>
        /// 角度V(deg)
        /// </summary>
        public double V { get; set; }

        /// <summary>
        /// 角度W(deg)
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// 手势 true=right false=left
        /// </summary>
        public bool Hand { get; set; }

        /// <summary>
        /// 本地坐标系编号
        /// </summary>
        public int LocalNo { get; set; }

        /// <summary>
        /// P1（脉冲）
        /// </summary>
        public int P1 { get; set; }

        /// <summary>
        /// J2（脉冲）
        /// </summary>
        public int P2 { get; set; }

        /// <summary>
        /// P3（脉冲）
        /// </summary>
        public int P3 { get; set; }

        /// <summary>
        /// P4（脉冲）
        /// </summary>
        public int P4 { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string Tage { get; set; }

        /// <summary>
        /// 点ID
        /// </summary>
        public string PID { get; set; }
    }
}
