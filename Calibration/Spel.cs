using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calibration
{
    public class Spel
    {
        #region 基础设置
        /// <summary>
        /// 功率模式设为High
        /// </summary>
        public const string PowerHigh = "Power High";
        /// <summary>
        /// 功率模式设为Low
        /// </summary>
        public const string PowerLow = "Power Low";
        #endregion

        #region 读取状态
        /// <summary>
        /// 获取机器人状态
        /// </summary>
        public const string PrintRobotInfo_0 = "Print RobotInfo(0)";
        /// <summary>
        /// 获取当前位置
        /// </summary>
        public const string RealPos = "Print RealPos";
        /// <summary>
        /// 1关节的脉冲值
        /// </summary>
        public const string PrintPls_1 = "Print Pls(1)";
        /// <summary>
        /// 2关节的脉冲值
        /// </summary>
        public const string PrintPls_2 = "Print Pls(2)";
        /// <summary>
        /// 3关节的脉冲值
        /// </summary>
        public const string PrintPls_3 = "Print Pls(3)";
        /// <summary>
        /// 4关节的脉冲值
        /// </summary>
        public const string PrintPls_4 = "Print Pls(4)";
        #endregion

        #region 原点相关
        /// <summary>
        /// 让机器人回到起始点
        /// </summary>
        public const string Home = "Home";
        /// <summary>
        /// 设置起始点位置
        /// </summary>
        public const string HomeSet = "HomeSet";
        /// <summary>
        /// 设置起始次序
        /// </summary>
        public const string Hordr = "Hordr";
        /// <summary>
        /// 重置
        /// </summary>
        public const string Reset = "Reset";
        /// <summary>
        /// 获取机器人原点状态
        /// </summary>
        public const string PrintRobotInfo_2 = "Print RobotInfo(2)";
        #endregion

        #region 动作相关
        /// <summary>
        /// 点到点运动Go (PTP)
        /// </summary>
        public const string Go = "Go";
        /// <summary>
        ///  点到点运动TGo (PTP)
        /// </summary>
        public const string TGo = "TGo";
        /// <summary>
        ///  跳到某一点运动Jump (PTP)
        /// </summary>
        public const string Jump = "Jump";
        /// <summary>
        /// 直线运动Move
        /// </summary>
        public const string Move = "Move";
        /// <summary>
        /// 直线运动TMove
        /// </summary>
        public const string TMove = "TMove";
        /// <summary>
        /// 等待机器人进行减速停止
        /// </summary>
        public const string WaitPos = "WaitPos";
        /// <summary>
        /// 设置速度(CP)
        /// </summary>
        public const string Speeds = "Speeds";
        /// <summary>
        /// 设置速度(PTP)
        /// </summary>
        public const string Accel = "Accel";
        /// <summary>
        /// 设置速度(CP)
        /// </summary>
        public const string Accels = "Accels";
        /// <summary>
        /// 设置速度(PTP)
        /// </summary>
        public const string Speed = "Speed";
        #endregion

        #region 点位相关
        /// <summary>
        /// 保存点文件
        /// </summary>
        public const string SavePoints = "SavePoints";
        /// <summary>
        /// 读取点文件
        /// </summary>
        public const string LoadPoints = "LoadPoints";
        /// <summary>
        /// 导入至项
        /// </summary>
        public const string ImportPoints = "ImportPoints";
        /// <summary>
        /// 定义点
        /// </summary>
        public const string P = "P";
        /// <summary>
        /// 定义点标签
        /// </summary>
        public const string PLabel = "PLabel";
        /// <summary>
        /// 读取点数据集
        /// </summary>
        public const string Plist = "Plist";
        #endregion

        #region  阵列
        /// <summary>
        /// 构建阵列
        /// </summary>
        public const string Pallet = "Pallet";
        #endregion

    }
}
