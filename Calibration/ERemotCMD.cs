using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calibration
{
    public enum ERemotCMD
    {
        /// <summary>
        ///登陆
        /// </summary>
        Login,
        /// <summary>
        /// 登出
        /// </summary>
        Logout,
        /// <summary>
        /// 执行指定编号的函数
        /// </summary>
        Start,
        /// <summary>
        /// 停止所有的任务和命令
        /// </summary>
        Stop,
        /// <summary>
        /// 暂停所有任务
        /// </summary>
        Pause,
        /// <summary>
        /// 继续暂停了的任务
        /// </summary>
        Continue,
        /// <summary>
        /// 清除紧急停止和错误
        /// </summary>
        Reset,
        /// <summary>
        /// 打开机器人电机
        /// </summary>
        SetMotorsOn,
        /// <summary>
        /// 关闭机器人电机
        /// </summary>
        SetMotorsOff,
        /// <summary>
        /// 选择机器人
        /// </summary>
        SetCurRobot,
        /// <summary>
        /// 过去当前的机器人编号
        /// </summary>
        GetCurRobot,
        /// <summary>
        /// 将机器人手臂移动到由用户定义的起始点位置上
        /// </summary>
        Home,
        /// <summary>
        /// 获取指定的I/O位
        /// </summary>
        GetIO,
        /// <summary>
        /// 设置I/O指定位
        /// </summary>
        SetIO,
        /// <summary>
        /// 获得指定的I/O端口（8位
        /// </summary>
        GetIOByte,
        /// <summary>
        /// 设置I/O指定端口（8位）
        /// </summary>
        SetIOByte,
        /// <summary>
        /// 获得指定的I/O字端口（16位）
        /// </summary>
        GetIOWord,
        /// <summary>
        /// 设置I/O指定字端口（8位）
        /// </summary>
        SetIOWord,
        /// <summary>
        /// 获取指定的内存I/O位
        /// </summary>
        GetMemIO,
        /// <summary>
        /// 设置指定的内存I/O位
        /// </summary>
        SetMemIO,
        /// <summary>
        /// 获取指定内存I/O端口
        /// </summary>
        GetMemIOByte,
        /// <summary>
        /// 设置指定的内存I/O端口（8位）
        /// </summary>
        SetMemIOByte,
        /// <summary>
        /// 获取指定的内存I/O字端口（16位）
        /// </summary>
        GetMemIOWord,
        /// <summary>
        /// 设置指定的内存I/O字端口（16位）
        /// </summary>
        SetMemIOWord,
        /// <summary>
        /// 获取备份（全局保留）参数的值
        /// </summary>
        GetVariable,
        /// <summary>
        /// 获取备份（全局保留）数组参数的值
        /// </summary>
        SetVariable,
        /// <summary>
        /// 获取控制器的状态
        /// </summary>
        GetStatus,
        /// <summary>
        /// 执行命令
        /// </summary>
        Execute,
        /// <summary>
        /// 中止命令的执行
        /// </summary>
        Abort,
        /// <summary>
        /// 空命令
        /// </summary>
        NULL,
    }
}
