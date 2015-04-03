﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    /// <summary>
    /// 记录仪命令参数
    /// </summary>
    public enum NOISE_LOG_COMMAND
    {
        #region 更新配置
        /// <summary>
        /// 记录仪时间
        /// </summary>
        WRITE_TIME = 0x11,
        /// <summary>
        /// 记录仪无线通讯
        /// </summary>
        WRITE_WIRELESS = 0x12,
        /// <summary>
        /// 采集数据时间段
        /// </summary>
        WRITE_START_END_TIME = 0x13,
        /// <summary>
        /// 采集时间间隔
        /// </summary>
        WRITE_INTERVAL = 0x14,
        /// <summary>
        /// 远传功能开关
        /// </summary>
        WRITE_REMOTE_SWITCH = 0x16,
        /// <summary>
        /// 远传发送时间
        /// </summary>
        WRITE_REMOTE_SEND_TIME = 0x17,

        #endregion

        #region 读取配置
        /// <summary>
        /// 串口读取记录仪时间
        /// </summary>
        READ_TIME = 0x41,
        /// <summary>
        /// 串口读取记录仪无线通讯的【收发频率＋无线速率＋发射功率＋串口速率＋唤醒时间】
        /// </summary>
        READ_WIRELESS = 0x42,
        /// <summary>
        /// 串口读取记录仪采集数据的时间
        /// </summary>
        READ_START_END_TIME = 0x43,
        /// <summary>
        /// 串口读取记录仪采集时间间隔
        /// </summary>
        READ_INTERVAL = 0x44,
        /// <summary>
        /// 串口读取记录仪远传功能
        /// </summary>
        READ_REMOTE = 0x46,
        /// <summary>
        /// 串口读取记录仪远传发送时间
        /// </summary>
        READ_REMOTE_SEND_TIME = 0x47,
        /// <summary>
        /// 串口读取记录仪的ID
        /// </summary>
        READ_NOISE_LOG_ID = 0x4e,
        #endregion

        #region 发送
        /// <summary>
        /// 响应命令发送
        /// </summary>
        SEND_RESPONSE_DATA = 0xa0,
        /// <summary>
        /// 相应启动命令(静态标准值)
        /// </summary>
        SEND_RESPONSE_DATA_ORIGITY = 0xa1,
        #endregion

        #region 控制
        /// <summary>
        /// 串口发送控制记录仪【启动/停止】命令
        /// </summary>
        CTRL_START_OR_STOP = 0x70,
        /// <summary>
        /// 串口读取记录仪数据
        /// </summary>
        CTRL_START_READ = 0x71,
        /// <summary>
        /// 清除FLASH
        /// </summary>
        CTRL_CLEAR_FLASH = 0x72,
        /// <summary>
        /// 读取静态基准值
        /// </summary>
        CTRL_READORIGITY = 0xa1
        #endregion
    }
    /// <summary>
    /// 远传控制器命令参数
    /// </summary>
    public enum NOISE_CTRL_COMMAND
    {
        #region 设置
        /// <summary>
        /// 无线通讯
        /// </summary>
        WRITE_WIRELESS = 0x18,
        /// <summary>
        /// 串口与GPRS模块的通讯波特率
        /// </summary>
        WRITE_GPRS_BAUDRATE = 0x19,
        /// <summary>
        /// 远传控制器IP
        /// </summary>
        WRITE_IP = 0x1a,
        /// <summary>
        /// 远传控制器端口号
        /// </summary>
        WRITE_PORT = 0x1b,
        /// <summary>
        /// 远传控制器设备与记录仪设备对应的ID
        /// </summary>
        WRITE_CTRL_NOISE_LOG_ID = 0x1c,
        #endregion

        #region 读取
        /// <summary>
        /// 串口读取远传控制器无线通讯
        /// </summary>
        READ_WIRELESS = 0x48,
        /// <summary>
        /// 串口与GPRS模块的通讯波特率
        /// </summary>
        READ_GPRS_BAUDRATE = 0x49,
        /// <summary>
        /// 串口读取远传控制器IP
        /// </summary>
        READ_IP = 0x4a,
        /// <summary>
        /// 串口读取远传控制器端口号
        /// </summary>
        READ_PORT = 0x4b,
        /// <summary>
        /// 串口读取远传控制器设备与记录仪设备对应的ID号
        /// </summary>
        READ_CTRL_NOISE_LOG_ID = 0x4c,
        /// <summary>
        /// 串口读取远程控制器的ID
        /// </summary>
        READ_NOISE_CTRL_ID = 0x4f
        #endregion
    }

    /// <summary>
    /// 巡视仪命令参数(Test->Form1)
    /// </summary>
    public enum NOISE_TOUR_COMMAND
    {
        /// <summary>
        /// 串口设置巡视仪【收发频率＋无线速率＋串口速率】
        /// </summary>
        SETTING = 0x10,

        /// <summary>
        /// 串口读取巡视仪 收发频率＋无线速率+串口速率
        /// </summary>
        READ_WIRELESS = 0x40,
        /// <summary>
        /// 串口读取巡视仪的ID
        /// </summary>
        READ_NOISE_TOUR_ID = 0x4d
    }

    public enum UNIVERSAL_COMMAND
    {
        #region 设置
        /// <summary>
        /// 设置时间
        /// </summary>
        SET_TIME = 0x10,
        /// <summary>
        /// 设置从站脉冲时间间隔
        /// </summary>
        SET_PLUSEINTERVAL = 0x11,
        /// <summary>
        /// 设置从站模拟量时间间隔
        /// </summary>
        SET_SIMINTERVAL = 0x14,
        /// <summary>
        /// 设置从站485采集时间间隔
        /// </summary>
        SET_485INTERVAL = 0x16,
        /// <summary>
        /// 设置从站采集配置功能
        /// </summary>
        SET_COLLECTCONFIG = 0x18,
        /// <summary>
        /// 设置从站485采集MODBUS执行标识
        /// </summary>
        SET_MODBUSEXEFLAG = 0x19,
        /// <summary>
        /// 设置485采集modbus协议
        /// </summary>
        SET_MODBUSPROTOCOL = 0x1b,
        /// <summary>
        /// 设置从站ID
        /// </summary>
        SET_ID = 0x22,
        /// <summary>
        /// 设置从站IP
        /// </summary>
        SET_IP = 0x23,
        /// <summary>
        /// 设置从站端口号
        /// </summary>
        SET_PORT = 0x24,
        /// <summary>
        /// 设置从站通信方式
        /// </summary>
        SET_COMTYPE = 0x2a,
        #endregion

        #region 读取
        /// <summary>
        /// 读取从站时间
        /// </summary>
        READ_TIME = 0x40,
        /// <summary>
        /// 读取从站波特率
        /// </summary>
        READ_BAUD = 0x42,
        /// <summary>
        /// 读取从站模拟量时间间隔
        /// </summary>
        READ_SIMINTERVAL = 0x44,
        /// <summary>
        /// 读取从站RS485时间间隔
        /// </summary>
        READ_485INTERVAL = 0x46,
        /// <summary>
        /// 读取从站脉冲时间间隔
        /// </summary>
        READ_PLUSEINTERVAL = 0x41,
        /// <summary>
        /// 读取从站采集功能配置
        /// </summary>
        READ_COLLECTCONFIG = 0x48,
        /// <summary>
        /// 读取从站MODBUS协议执行标识
        /// </summary>
        READ_MODBUSEXEFLAG = 0x49,
        /// <summary>
        /// 读取从站485采集modbus协议
        /// </summary>
        READ_MODBUSPROTOCOL = 0x4b,
        /// <summary>
        /// 读取从站
        /// </summary>
        READ_ID = 0x51,
        READ_IP = 0x52,
        READ_PORT = 0x53,
        READ_CELLPHONE = 0x54,
        READ_COMTYPE = 0x59,
        #endregion

        #region 控制命令
        /// <summary>
        /// 复位命令
        /// </summary>
        RESET = 0x72,
        /// <summary>
        /// 启动终端采集功能命令
        /// </summary>
        EnableCollect = 0x73,
        /// <summary>
        /// 从站第1路模拟量零点值校准
        /// </summary>
        CalibartionSimualte1 = 0x4E,
        /// <summary>
        /// 从站第2路模拟量零点值校准
        /// </summary>
        CalibartionSimualte2= 0x4F
        #endregion
    }

    /// <summary>
    /// 控制码类型
    /// </summary>
    public enum CTRL_COMMAND_TYPE
    {
        /// <summary>
        /// 由主站发出的命令帧(应答帧) 如：主站向从站设置时钟 0x8
        /// </summary>
        REQUEST_BY_MASTER = 0x8,
        /// <summary>
        /// 由主站发出的应答帧 如：主站回应从站接收采集数据成功。0x4
        /// </summary>
        RESPONSE_BY_MASTER = 0x4,
        /// <summary>
        /// 由从站发出的数据帧 如：从站向主站传送采集数据。0x2
        /// </summary>
        REQUEST_BY_SLAVE = 0x2,
        /// <summary>
        /// 由从站发出的应答帧 如：从站应答主站读取终端采集时间间隔。0x1
        /// </summary>
        RESPONSE_BY_SLAVE = 0x1,
    }

    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DEV_TYPE
    {
        /// <summary>
        /// 数据采集终端
        /// </summary>
        Data_CTRL = 0x00,
        /// <summary>
        /// 压力控制器
        /// </summary>
        PRESS_CTRL = 0x01,
        /// <summary>
        /// 通用终端
        /// </summary>
        UNIVERSAL_CTRL = 0x02,
        /// <summary>
        /// 便携式压力控制终端
        /// </summary>
        MOBELE_PRESSURE = 0x03,
        /// <summary>
        /// 噪音记录仪
        /// </summary>
        NOISE_LOG = 0x04,
        /// <summary>
        /// 数据远传控制器
        /// </summary>
        NOISE_CTRL = 0x05,
        /// <summary>
        /// 巡视仪
        /// </summary>
        NOISE_TOUR = 0x06
    }

    #region 远传命令
    /// <summary>
    /// 压力终端远传命令参数
    /// </summary>
    public enum GPRS_WRITE
    {
        /// <summary>
        /// 设置时间
        /// </summary>
        WRITE_TIME = 0X10,
        /// <summary>
        /// 压力时间间隔
        /// </summary>
        WRITE_PREINTERVAL = 0x11,
        /// <summary>
        /// 压力偏移量
        /// </summary>
        WRITE_PREOFFSET = 0x12,
        /// <summary>
        /// 流量时间间隔
        /// </summary>
        WRITE_FLOWINTERVAL = 0x14,
        /// <summary>
        /// 压力报警上限值
        /// </summary>
        WRITE_PRE_UPPERLIMIT = 0x15,
        /// <summary>
        /// 压力报警下限值
        /// </summary>
        WRITE_PRE_LOWLIMIT = 0x16,
        /// <summary>
        /// 电池电压报警下限值
        /// </summary>
        WRITE_BATTERY_LOWLIMIT = 0x17,
        /// <summary>
        /// 斜率报警上限值
        /// </summary>
        WRITE_SLOPE_UPPERLIMIT = 0x18,
        /// <summary>
        /// 斜率报警下限值
        /// </summary>
        WRITE_SLOPE_LOWLIMIT = 0x19,
        /// <summary>
        /// 心跳时间间隔
        /// </summary>
        WRITE_HEARTINTERVAL = 0x1A,
        /// <summary>
        /// 设置采集功能配置
        /// </summary>
        WRITE_COLLECT = 0x1B,
        /// <summary>
        /// 设置通讯方式
        /// </summary>
        WRITE_COMTYPE = 0x1C,
        /// <summary>
        /// 485波特率
        /// </summary>
        WRITE_BAUD = 0x1D,
        /// <summary>
        /// 设置ID
        /// </summary>
        WRITE_SETID = 0x1E,
        /// <summary>
        /// 设置IP
        /// </summary>
        WRITE_IP = 0x1F,
        /// <summary>
        /// 设置端口
        /// </summary>
        WRITE_PORT = 0x20,
        /// <summary>
        /// 设置短信手机号
        /// </summary>
        WRITE_CELLPHONE = 0x21,
        /// <summary>
        /// 上限报警功能投/退
        /// </summary>
        WRITE_PREUPPERALARM_ENABLE = 0x22,
        /// <summary>
        /// 下限报警功能投/退
        /// </summary>
        WRITE_PRELOWALARM_ENABLE = 0x23,
        /// <summary>
        /// 斜率上限报警投/退
        /// </summary>
        WRITE_SLOPEUPPERALARM_ENABLE = 0x24,
        /// <summary>
        /// 斜率下限报警投/退
        /// </summary>
        WRITE_SLOPELOWALARM_ENABLE = 0x25,
        /// <summary>
        /// 电池电压采集时间间隔
        /// </summary>
        WRITE_BATTERY_INTERVAL = 0x26,
        /// <summary>
        /// 压力量程
        /// </summary>
        WRITE_PRE_SPAN = 0X28
    }

    public enum GPRS_READ
    {
        /// <summary>
        /// 从站向主站发送压力采集数据
        /// </summary>
        READ_PREDATA = 0xA0,
        /// <summary>
        /// 从站向主站发送流量采集数据
        /// </summary>
        READ_FLOWDATA = 0xA1,
        /// <summary>
        /// 从站向主站发送设备报警信息
        /// </summary>
        READ_ALARMINFO = 0xA2,
        /// <summary>
        /// 通用终端发送脉冲数据
        /// </summary>
        READ_UNIVERSAL_PLUSE=0xA0,
        /// <summary>
        /// 通用终端发送模拟量1路数据
        /// </summary>
        READ_UNIVERSAL_SIM1=0xA1,
        /// <summary>
        /// 通用终端发送模拟量2路数据
        /// </summary>
        READ_UNIVERSAL_SIM2=0xA2,
        /// <summary>
        /// 通用终端发送RS485 1路数据
        /// </summary>
        READ_UNVERSAL_RS4851=0xA3,
        /// <summary>
        /// 通用终端发送RS485 2路数据
        /// </summary>
        READ_UNVERSAL_RS4852 = 0xA4,
        /// <summary>
        /// 通用终端发送RS485 3路数据
        /// </summary>
        READ_UNVERSAL_RS4853 = 0xA5,
        /// <summary>
        /// 通用终端发送RS485 4路数据
        /// </summary>
        READ_UNVERSAL_RS4854 = 0xA6,
        /// <summary>
        /// 通用终端发送RS485 5路数据
        /// </summary>
        READ_UNVERSAL_RS4855 = 0xA7,
        /// <summary>
        /// 通用终端发送RS485 6路数据
        /// </summary>
        READ_UNVERSAL_RS4856 = 0xA8,
        /// <summary>
        /// 通用终端发送RS485 7路数据
        /// </summary>
        READ_UNVERSAL_RS4857 = 0xA9,
        /// <summary>
        /// 通用终端发送RS485 8路数据
        /// </summary>
        READ_UNVERSAL_RS4858 = 0xAA
    }

    public enum GPRS_CTRL
    {
        /// <summary>
        /// 设置GPRS招测功能
        /// </summary>
        CTRL_ZHAOCE_ENABLE = 0x70,
        /// <summary>
        /// GPRS招测
        /// </summary>
        CTRL_ZHAOCE = 0x71,
        /// <summary>
        /// 复位
        /// </summary>
        CTRL_RESET = 0x72,
        /// <summary>
        /// 启动终端采集功能
        /// </summary>
        CTRL_COL_ENABLE = 0x73,
        /// <summary>
        /// 读取压力历史数据
        /// </summary>
        CTRL_PREHISTORY = 0x74,
        /// <summary>
        /// 读取流量历史数据
        /// </summary>
        CTRL_FLOWHISTORY = 0x75,
        /// <summary>
        /// 压力数据条数
        /// </summary>
        CTRL_PREDATACOUNT = 0x76,
        /// <summary>
        /// 流浪数据条数
        /// </summary>
        CTRL_FLOWDATACOUNT = 0x77
    }
    #endregion

    class CommandInfo
    {

    }
}