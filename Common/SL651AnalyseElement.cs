﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Entity;

namespace SmartWaterSystem
{
    public class SL651AnalyseElement
    {
        static NLog.Logger logger = NLog.LogManager.GetLogger("SL651AnalyseElement");
        public static string GetFuncodeName(byte b)
        {
            string name = "";
            switch (b)
            {
                case 0x2F:
                    name = "链路维持报";
                    break;
                case 0x30:
                    name = "测试报";
                    break;
                case 0x31:
                    name = "均匀时段降水量/水位";
                    break;
                case 0x32:
                    name = "遥测站定时报";
                    break;
                case 0x33:
                    name = "遥测站加报";
                    break;
                case 0x34:
                    name = "遥测站小时报";
                    break;
                case 0x35:
                    name = "人工置数报";
                    break;
                case 0x37:
                    name = "查询遥测站实时数据";
                    break;
                case 0x38:
                    name = "查询时段降水量/水位";
                    break;
                case 0x39:
                    name = "读取人工置数";
                    break;
                case 0x3A:
                    name = "查询指定要素数据";
                    break;
                case 0x40:
                    name = "修改基本配置表";
                    break;
                case 0x41:
                    name = "读取基本配置表";
                    break;
                case 0x42:
                    name = "修改运行配置表";
                    break;
                case 0x43:
                    name = "读取运行配置表";
                    break;
                case 0x45:
                    name = "查询软件版本";
                    break;
                case 0x46:
                    name = "查询状态和报警";
                    break;
                case 0x47:
                    name = "初始化固态存储数据";
                    break;
                case 0x48:
                    name = "恢复出厂设置";
                    break;
                case 0x49:
                    name = "修改密码";
                    break;
                case 0x4A:
                    name = "设置时钟";
                    break;
                case 0x4F:
                    name = "水量定值控制";
                    break;
                case 0x50:
                    name = "查询事件记录";
                    break;
                case 0x51:
                    name = "查询时钟";
                    break;
                case 0xE0:
                    name = "设置人工置数内容";
                    break;
                case 0xE1:
                    name = "设置校准水位1";
                    break;
                case 0xE2:
                    name = "设置校准水位2";
                    break;
                case 0xE3:
                    name = "设置均匀时段报上传时间";
                    break;
                case 0xE4:
                    name = "读取均匀时段报上传时间";
                    break;
            }
            return name;
        }

        /// <summary>
        /// 解析要素信息
        /// </summary>
        /// <param name="funcode">功能码</param>
        /// <param name="elements">要素信息(值引用)</param>
        /// <param name="loopcount">循环次数，防止死循环,为0时退出</param>
        /// <returns></returns>
        public static string AnalyseElement( byte funcode, byte[] elements, byte[] dt, ref Universal651SerialPortEntity spEntity)
        {
            string strcontent = "";
            if (elements == null)
            {
                strcontent = "要素信息:空,";
                return strcontent;
            }
            int oldelemntslen = elements.Length;  //保留旧的elements数据长度,如果循环一次后，elements的长度没有变化，说明有数据解析不了，则退出递归返回
            if (spEntity == null)
                spEntity = new Universal651SerialPortEntity();
            try
            {
                switch (funcode)
                {
                    case (byte)SL651_COMMAND.QueryTime:
                        #region 查询时间
                        if (dt == null)
                            return "";
                        string str_dt = string.Format("{0:X2}", dt[0]) + "-" + string.Format("{0:X2}", dt[1]) + "-" + string.Format("{0:X2}", dt[2]) + " " +
                            string.Format("{0:X2}", dt[3]) + ":" + string.Format("{0:X2}", dt[4]) + ":" + string.Format("{0:X2}", dt[5]);
                        strcontent = "当前时间:" + str_dt + ",";
                        spEntity.IsOptQueryTime = true;
                        spEntity.QueryTime = str_dt;
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.QueryVer:
                        #region 查询版本信息
                        int len = Convert.ToInt32(elements[0]);

                        strcontent = "版本信息:";
                        int verlen = Convert.ToInt32(elements[7]);
                        string str_ver = "";
                        for (int i = 0; i < verlen; i++)
                        {
                            if (elements[8 + i] >= 32 && elements[8 + i] <= 127)  //ASCII打印字符范围
                                str_ver += string.Format("{0:X2}", elements[8 + i]);
                        }
                        str_ver = ConvertHelper.HexToASCII(str_ver);
                        strcontent += str_ver + ",";
                        spEntity.IsOptQueryVer = true;
                        spEntity.Ver = str_ver;
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.QueryEvent:
                        #region 查询事件
                        strcontent = "事件信息:";
                        string str_event = "";
                        if (elements.Length == 7 + 32 * 2)
                        {
                            str_event = string.Format(@"数据初始化记录:{0};参数变更记录:{1};状态量变位记录:{2};仪表故障记录:{3};密码修改记录:{4};
                        终端故障记录:{5};交流失电记录:{6};蓄电池电压低告警记录:{7};终端箱非法打开记录:{8};水泵故障记录:{9};
                        剩余水量越限告警记录:{10};水位超限告警记录:{11};水压超限告警记录:{12};水质参数超限告警记录:{13};数据出错记录:{14};
                        发报文记录:{15};收报文记录:{16};发报文出错记录:{17}",
                            BitConverter.ToInt16(new byte[] { elements[8], elements[7] }, 0), BitConverter.ToInt16(new byte[] { elements[10], elements[9] }, 0), BitConverter.ToInt16(new byte[] { elements[12], elements[11] }, 0), BitConverter.ToInt16(new byte[] { elements[14], elements[13] }, 0), BitConverter.ToInt16(new byte[] { elements[16], elements[15] }, 0),
                            BitConverter.ToInt16(new byte[] { elements[18], elements[17] }, 0), BitConverter.ToInt16(new byte[] { elements[20], elements[19] }, 0), BitConverter.ToInt16(new byte[] { elements[22], elements[21] }, 0), BitConverter.ToInt16(new byte[] { elements[24], elements[23] }, 0), BitConverter.ToInt16(new byte[] { elements[26], elements[25] }, 0),
                            BitConverter.ToInt16(new byte[] { elements[28], elements[27] }, 0), BitConverter.ToInt16(new byte[] { elements[30], elements[29] }, 0), BitConverter.ToInt16(new byte[] { elements[32], elements[31] }, 0), BitConverter.ToInt16(new byte[] { elements[34], elements[33] }, 0), BitConverter.ToInt16(new byte[] { elements[36], elements[35] }, 0),
                            BitConverter.ToInt16(new byte[] { elements[38], elements[37] }, 0), BitConverter.ToInt16(new byte[] { elements[40], elements[39] }, 0), BitConverter.ToInt16(new byte[] { elements[42], elements[41] }, 0));
                        }
                        spEntity.IsOptQueryEvent = true;
                        spEntity.QueryEvent = str_event;
                        strcontent += str_event + ",";
                        strcontent = strcontent.Replace(" ", "");
                        strcontent = strcontent.Replace("\r", "");
                        strcontent = strcontent.Replace("\n", "");
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.ChPwd:
                        #region 修改密码
                        for (int i = 1; i < elements.Length - 2; i++)
                        {
                            if (elements[i - 1] == PackageDefine.PwdFlag[0] && elements[i] == PackageDefine.PwdFlag[1])
                            {
                                strcontent += "新密码:" +BitConverter.ToInt16(new byte[] { elements[i + 2], elements[i + 1] },0) + ",";
                                spEntity.IsOptPwd = true;
                                spEntity.CPwd0 = elements[i + 1];
                                spEntity.CPwd1 = elements[i + 2];
                                break;
                            }
                        }
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.QueryAlarm:      //状态及报警
                        #region 查询状态及报警
                        strcontent = "状态及报警信息:";
                        string str_alarm = "";
                        if (elements.Length == 7 + 2 + 4)
                        {
                            str_alarm = string.Format(@"交流电充电状态:{0};蓄电池电压状态:{1};水位超限报警:{2};流量超限报警:{3};水质超限报警:{4};流量仪表故障:{5};
                                                    水位仪表故障报警:{6};终端箱门状态:{7};存储器状态:{8};IC卡功能有效:{9};水泵工作状态:{10};剩余水量报警:{11}",
                                                     ((elements[12] & 0x01) == 0x01 ? "停电" : "正常"), ((elements[12] & 0x02) == 0x02 ? "电压低" : "正常"), ((elements[12] & 0x04) == 0x04 ? "报警" : "正常"), ((elements[12] & 0x08) == 0x08 ? "报警" : "正常"), ((elements[12] & 0x10) == 0x10 ? "报警" : "正常"),
                                                     ((elements[12] & 0x20) == 0x20 ? "故障" : "正常"), ((elements[12] & 0x40) == 0x40 ? "故障" : "正常"), ((elements[12] & 0x80) == 0x80 ? "关闭" : "正常"), ((elements[11] & 0x01) == 0x01 ? "异常" : "正常"), ((elements[11] & 0x04) == 0x04 ? "IC卡有效" : "正常"),
                                                     ((elements[11] & 0x08) == 0x08 ? "水泵停机" : "正常"), ((elements[11] & 0x10) == 0x10 ? "水量超限" : "正常"));
                        }
                        spEntity.IsOptQueryAlarm = true;
                        spEntity.QueryAlarm = str_alarm;
                        strcontent += str_alarm + ",";
                        strcontent = strcontent.Replace(" ", "");
                        strcontent = strcontent.Replace("\r", "");
                        strcontent = strcontent.Replace("\n", "");
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.SetCalibration1: //设置校准水位1
                    case (byte)SL651_COMMAND.SetCalibration2: //设置校准水位2
                        #region 设置校准水位1、2
                        if (elements.Length == 9)
                        {
                            spEntity.IsOptSetCalibration=true;
                            spEntity.SetCalibration = BitConverter.ToInt16(new byte[] { elements[8], elements[7] }, 0).ToString();
                            if (funcode == (byte)SL651_COMMAND.SetCalibration1)
                                strcontent = "相对水位1校准值:" + spEntity.SetCalibration;
                            else
                                strcontent = "相对水位2校准值:" + spEntity.SetCalibration;
                        }
                        else
                        {
                            strcontent = "设置校准水位返回长度不正确";
                        }
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.HourReport:      //小时报
                    case (byte)SL651_COMMAND.TimingReport:    //定时报
                    case (byte)SL651_COMMAND.TestReport:      //测试报
                    case (byte)SL651_COMMAND.AddtionReport:   //遥测站加报
                    case (byte)SL651_COMMAND.QueryCurData:    //查询实时数据
                    case (byte)SL651_COMMAND.QueryElements:   //查询指定要素
                        #region
                        int sumlen, pointlen;
                        if (elements.Length > 0 && elements[0] == PackageDefine.AddrFlagHeader[0] && elements[1] == PackageDefine.AddrFlagHeader[1])
                        {
                            strcontent = "分类码:" + "0x" + string.Format("{0:X2}", elements[7]) + ",";
                            elements = BytesRemove(elements, 8);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.ObservationTimeFlag[0] && elements[1] == PackageDefine.ObservationTimeFlag[1])
                        {
                            strcontent += "观测时间:" + string.Format("{0:X2}", elements[2]) + string.Format("{0:X2}", elements[3]) +
                                    string.Format("{0:X2}", elements[4]) + string.Format("{0:X2}", elements[5]) + string.Format("{0:X2}", elements[6]) + ",";
                            elements = BytesRemove(elements, 7);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.RainFallLimitFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i < sumlen + 2; i++)
                            {
                                strcontent += "雨量加报阀值:" + ConvertToDouble(string.Format("{0:X2}", elements[i])) / Math.Pow(10, pointlen) + "mm,";  //1
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        //if (elements.Length > 0 && elements[0] == PackageDefine.PeriodPrecipitationFlag[0])
                        //{
                        //    GetSumLen(elements[1], out sumlen, out pointlen);
                        //    for (int i = 2; i + 1 < sumlen + 2; i += 3)
                        //    {
                        //        strcontent += "时段降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";  //10
                        //    }
                        //    elements = BytesRemove(elements, sumlen + 2);
                        //}
                        if (elements.Length > 0 && elements[0] == PackageDefine.RainFallAddupFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "降水累计量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.InstantWaterlevelFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 3 < sumlen + 2; i += 4)
                            {
                                if (!strcontent.Contains("瞬时水位:"))
                                    strcontent += "瞬时水位:";
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])
                                            + string.Format("{0:X2}", elements[i + 2]) + string.Format("{0:X2}", elements[i + 3])) / Math.Pow(10, pointlen) + "m,";  //1000
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.PowerFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 1 < sumlen + 2; i += 2)
                            {
                                strcontent += "电源电压:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])) / Math.Pow(10, pointlen) + "v,";  //100
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.RainFallTookFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 1 < sumlen + 2; i += 2)
                            {
                                strcontent += "降水历时:" + string.Format("{0:X2}", elements[i]) + "." + string.Format("{0:X2}", elements[i + 1]) + ",";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.AlarmFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 3 < sumlen + 2; i += 4)
                            {
                                strcontent += "状态及报警信息:" + string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])
                                    + string.Format("{0:X2}", elements[i + 2]) + string.Format("{0:X2}", elements[i + 3]) + ",";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation5MinFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "1h内5min时段降水量:";
                            for (int i = 2; i < sumlen + 2; i++)
                            {
                                if (elements[i] == 0xff)
                                    strcontent += "--,";
                                else
                                    strcontent += Convert.ToDouble(elements[i]) / 10 + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Waterlevel5MinFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "1h内5min间隔相对水位:";
                            for (int i = 2; i + 1 < sumlen + 2; i += 2)
                            {
                                if (elements[i + 1] == 0xff && elements[i] == 0xff)
                                    strcontent += "--,";
                                else
                                    strcontent += (((double)BitConverter.ToUInt16(new byte[] { elements[i + 1], elements[i] }, 0)) / 100).ToString("f2") + "cm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.InstantWaterTempFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 1 < sumlen + 2; i += 2)
                            {
                                strcontent += "瞬时水温:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])) / Math.Pow(10, pointlen) + "℃,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.PHFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 1 < sumlen + 2; i += 2)
                            {
                                strcontent += "PH:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])) / Math.Pow(10, pointlen) + ",";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.ConductivityFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "电导率:" + Convert.ToInt32(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) +
                                    string.Format("{0:X2}", elements[i + 2])) + ",";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.TurbidityFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 1 < sumlen + 2; i += 2)
                            {
                                strcontent += "浊度:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])) + ",";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.PrecipitationFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "当前降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.DayPrecipitationFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "日降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation1min[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "1min降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation5min[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "5min降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation10min[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "10min降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation30min[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "30min降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation1h[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "1h降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation2h[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "2h降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation3h[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "3h降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation6h[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "6h降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation12h[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < sumlen + 2; i += 3)
                            {
                                strcontent += "12h降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0)
                        {
                            if (oldelemntslen > elements.Length)
                                strcontent += AnalyseElement(funcode, elements, null, ref spEntity);
                            else
                            {
                                strcontent+="未解析数据:"+ ConvertHelper.ByteArrayToHexString(elements);
                            }
                            
                        }
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.ReadBasiConfig:  //查询遥测站基本配置表
                    case (byte)SL651_COMMAND.SetBasicConfig:  //设置遥测站基本配置表
                        #region 查询基本配置表
                        for (int i = 1; i < elements.Length - 5; i++)
                        {
                            if (elements[i - 1] == PackageDefine.AddrFlagParm[0] && elements[i] == PackageDefine.AddrFlagParm[1])
                            {
                                strcontent += "遥测站地址:0x" + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2]) +
                                    string.Format("{0:X2}", elements[i + 3]) + string.Format("{0:X2}", elements[i + 4]) + string.Format("{0:X2}", elements[i + 5]) + ",";
                                spEntity.IsOptA1_A5 = true;
                                spEntity.CA5 = elements[i + 1];
                                spEntity.CA4 = elements[i + 2];
                                spEntity.CA3 = elements[i + 3];
                                spEntity.CA2 = elements[i + 4];
                                spEntity.CA1 = elements[i + 5];
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 4; i++)
                        {
                            if (elements[i - 1] == PackageDefine.CenterAddrFlag[0] && elements[i] == PackageDefine.CenterAddrFlag[1])
                            {
                                strcontent += "中心站地址:0x" + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2]) +
                                    string.Format("{0:X2}", elements[i + 3]) + string.Format("{0:X2}", elements[i + 4]) + ",";
                                spEntity.IsOptCenterAddr = true;
                                spEntity.CCenterAddr = elements[i + 1];  //第1字节是第1中心站
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 2; i++)
                        {
                            if (elements[i - 1] == PackageDefine.PwdFlag[0] && elements[i] == PackageDefine.PwdFlag[1])
                            {
                                strcontent += "新密码:" + BitConverter.ToInt16(new byte[] { elements[i + 2], elements[i + 1] }, 0) + ",";
                                spEntity.IsOptPwd = true;
                                spEntity.CPwd0 = elements[i + 1];
                                spEntity.CPwd1 = elements[i + 2];
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 1; i++)
                        {
                            if (elements[i - 1] == PackageDefine.WorkTypeFlag[0] && elements[i] == PackageDefine.WorkTypeFlag[1])
                            {
                                strcontent += "工作方式:";
                                spEntity.IsOptWorkType = true;
                                spEntity.WorkType = Convert.ToUInt16(elements[i + 1]);
                                switch (spEntity.WorkType)
                                {
                                    case 1:
                                        strcontent += "自报工作状态" + ",";
                                        break;
                                    case 2:
                                        strcontent += "自报确认工作状态" + ",";
                                        break;
                                    case 3:
                                        strcontent += "查询/应答工作状态" + ",";
                                        break;
                                    case 4:
                                        strcontent += "调试或维修状态" + ",";
                                        break;
                                    default:
                                        strcontent += "工作方式:0x" + string.Format("{0:X2}", elements[i + 1]) + ",";
                                        break;
                                }
                                break;
                            }

                        }
                        for (int i = 1; i < elements.Length - 8; i++)
                        {
                            if (elements[i - 1] == PackageDefine.ElementsFlag[0] && elements[i] == PackageDefine.ElementsFlag[1])
                            {
                                strcontent += "采集要素:0x";
                                for (int j = 1; j <= 8; j++)
                                {
                                    strcontent += string.Format("{0:X2}", elements[i + j]);
                                }
                                strcontent += ",";
                                spEntity.IsOptElements = true;
                                spEntity.Elements = new byte[] { elements[i + 1], elements[i + 2], elements[i + 3], elements[i + 4], elements[i + 5], elements[i + 6], elements[i + 7], elements[i + 8] };
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 10; i++)
                        {
                            if (elements[i - 1] == PackageDefine.Channel1Flag[0] && elements[i] == PackageDefine.Channel1Flag[1])
                            {
                                strcontent += "中1主信道类型及地址:";
                                spEntity.ChannelType = 0;
                                spEntity.IsOptCh = true;
                            }
                            else if (elements[i - 1] == PackageDefine.Channel2Flag[0] && elements[i] == PackageDefine.Channel2Flag[1])
                            {
                                strcontent += "中2主信道类型及地址:";
                                spEntity.ChannelType = 1;
                                spEntity.IsOptCh = true;
                            }
                            else if (elements[i - 1] == PackageDefine.Channel3Flag[0] && elements[i] == PackageDefine.Channel3Flag[1])
                            {
                                strcontent += "中3主信道类型及地址:";
                                spEntity.ChannelType = 2;
                                spEntity.IsOptCh = true;
                            }
                            else if (elements[i - 1] == PackageDefine.Channel4Flag[0] && elements[i] == PackageDefine.Channel4Flag[1])
                            {
                                strcontent += "中4主信道类型及地址:";
                                spEntity.ChannelType = 3;
                                spEntity.IsOptCh = true;
                            }

                            if (spEntity.IsOptCh)
                            {
                                spEntity.Channel = Convert.ToUInt16(elements[i + 1]);
                                switch (spEntity.Channel)
                                {
                                    case 0:
                                        strcontent += "关闭";
                                        break;
                                    case 1:
                                        strcontent += "短信";
                                        break;
                                    case 2:
                                        strcontent += "IPV4";
                                        break;
                                    case 3:
                                        strcontent += "北斗";
                                        break;
                                    case 4:
                                        strcontent += "海事卫星";
                                        break;
                                    case 5:
                                        strcontent += "PSTN";
                                        break;
                                    case 6:
                                        strcontent += "超短波";
                                        break;
                                    default:
                                        strcontent += "主信道类型:0x" + string.Format("{0:X2}", elements[i + 1]);
                                        break;
                                }
                                if (spEntity.Channel > 0)
                                {
                                    string ip = "";
                                    for (int j = 2; j <= 7; j++)
                                    {
                                        ip += string.Format("{0:X2}", elements[i + j]);
                                    }
                                    strcontent += ",";
                                    char[] ip_chars = ip.ToCharArray();
                                    string str_ip = "";
                                    for (int j = 0; j < ip_chars.Length; j++)
                                    {
                                        str_ip += ip_chars[j];
                                        if ((j + 1) % 3 == 0)
                                            str_ip += ".";
                                    }
                                    str_ip = str_ip.Substring(0, str_ip.Length - 1);
                                    strcontent += str_ip;

                                    string[] str_ips = str_ip.Split('.');
                                    if (str_ips != null && str_ips.Length == 4)
                                    {
                                        spEntity.Ip1 = str_ips[0];
                                        spEntity.Ip2 = str_ips[1];
                                        spEntity.Ip3 = str_ips[2];
                                        spEntity.Ip4 = str_ips[3];
                                    }
                                }

                                strcontent += ":" + string.Format("{0:X2}", elements[i + 8]) + string.Format("{0:X2}", elements[i + 9]) + string.Format("{0:X2}", elements[i + 10]);
                                if (spEntity.Channel > 0)
                                    spEntity.Port = Convert.ToInt32(string.Format("{0:X2}", elements[i + 8]) + string.Format("{0:X2}", elements[i + 9]) + string.Format("{0:X2}", elements[i + 10]));
                                strcontent += ",";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 7; i++)
                        {
                            if (elements[i - 1] == PackageDefine.StandbyChannel1Flag[0] && elements[i] == PackageDefine.StandbyChannel1Flag[1])
                            {
                                strcontent += "备用信道1类型及地址:";
                                spEntity.ChannelType = 0;
                                spEntity.IsOptStandbyCh = true;
                            }
                            else if (elements[i - 1] == PackageDefine.StandbyChannel2Flag[0] && elements[i] == PackageDefine.StandbyChannel2Flag[1])
                            {
                                strcontent += "备用信道2类型及地址:";
                                spEntity.ChannelType = 1;
                                spEntity.IsOptStandbyCh = true;
                            }
                            else if (elements[i - 1] == PackageDefine.StandbyChannel3Flag[0] && elements[i] == PackageDefine.StandbyChannel3Flag[1])
                            {
                                strcontent += "备用信道3类型及地址:";
                                spEntity.ChannelType = 2;
                                spEntity.IsOptStandbyCh = true;
                            }
                            else if (elements[i - 1] == PackageDefine.StandbyChannel4Flag[0] && elements[i] == PackageDefine.StandbyChannel4Flag[1])
                            {
                                strcontent += "备用信道4类型及地址:";
                                spEntity.ChannelType = 3;
                                spEntity.IsOptStandbyCh = true;
                            }
                            if (spEntity.IsOptStandbyCh)
                            {
                                spEntity.StandByCh = Convert.ToUInt16(elements[i + 1]);
                                switch (spEntity.StandByCh)
                                {
                                    case 0:
                                        strcontent += "关闭";
                                        break;
                                    case 1:
                                        strcontent += "短信";
                                        break;
                                    case 2:
                                        strcontent += "IPV4";
                                        break;
                                    case 3:
                                        strcontent += "北斗";
                                        break;
                                    case 4:
                                        strcontent += "海事卫星";
                                        break;
                                    case 5:
                                        strcontent += "PSTN";
                                        break;
                                    case 6:
                                        strcontent += "超短波";
                                        break;
                                    default:
                                        strcontent += "信道类型:0x" + string.Format("{0:X2}", elements[i + 1]);
                                        break;
                                }
                                string telnum = "";
                                if (spEntity.StandByCh > 0)
                                {
                                    for (int j = 2; j < 8; j++)
                                    {
                                        telnum += string.Format("{0:X2}", elements[i + j]);
                                    }
                                }
                                spEntity.StandbyChTelnum = telnum;
                                strcontent += telnum + ",";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 12; i++)
                        {
                            if (elements[i - 1] == PackageDefine.IdentifyNumFlag[0] && elements[i] == PackageDefine.IdentifyNumFlag[1])
                            {
                                strcontent += "卡类型:";
                                spEntity.IsOptIdentifyNum = true;
                                spEntity.IdentifyNum = Convert.ToInt32(Convert.ToChar(elements[i + 1]).ToString());
                                switch (spEntity.IdentifyNum)
                                {
                                    case 1:
                                        strcontent += "移动通信卡" + ",卡号:";
                                        break;
                                    case 2:
                                        strcontent += "北斗卫星通信卡" + ",卡号:";
                                        break;
                                    default:
                                        strcontent += "卡类型:0x" + string.Format("{0:X2}", elements[i + 1]) + ",卡号:";
                                        break;
                                }
                                for (int j = 2; j <= 13; j++)
                                {
                                    spEntity.telnum += Convert.ToUInt16(elements[i + j]) - 48;
                                }
                                strcontent += spEntity.telnum;
                                break;
                            }
                        }
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.ReadRunConfig:   //查询遥测站运行配置表
                    case (byte)SL651_COMMAND.SetRunConfig:    //设置遥测站运行配置表
                        #region 查询运行配置表
                        for (int i = 1; i < elements.Length - 1; i++)
                        {
                            if (elements[i - 1] == PackageDefine.PeriodIntervalFlag[0] && elements[i] == PackageDefine.PeriodIntervalFlag[1])
                            {
                                spEntity.IsOptPeriodInterval = true;
                                spEntity.PeriodInterval = Convert.ToInt32(string.Format("{0:X2}", elements[i + 1]));
                                strcontent += "定时报时间间隔:" + spEntity.PeriodInterval + "h,";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 1; i++)
                        {
                            if (elements[i - 1] == PackageDefine.AddIntervalFlag[0] && elements[i] == PackageDefine.AddIntervalFlag[1])
                            {
                                spEntity.IsOptAddInterval = true;
                                spEntity.AddInterval = Convert.ToInt32(string.Format("{0:X2}", elements[i + 1]));  //string.Format("{0:X2}", elements[i + 1])
                                strcontent += "加报时间间隔:" + spEntity.AddInterval + "min,";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 1; i++)
                        {
                            if (elements[i - 1] == PackageDefine.PrecipitationStartTimeFlag[0] && elements[i] == PackageDefine.PrecipitationStartTimeFlag[1])
                            {
                                spEntity.IsOptPrecipitationStartTime = true;
                                spEntity.PrecipitationStartTime = Convert.ToInt32(string.Format("{0:X2}", elements[i + 1]));
                                strcontent += "降水量日起始时间:" + spEntity.PrecipitationStartTime + "h,";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 2; i++)
                        {
                            if (elements[i - 1] == PackageDefine.SamplingFlag[0] && elements[i] == PackageDefine.SamplingFlag[1])
                            {
                                spEntity.IsOptSamplingInterval = true;
                                spEntity.SamplingInterval = Convert.ToInt32(string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2]));
                                strcontent += "采样间隔:" + spEntity.SamplingInterval + "s,";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 1; i++)
                        {
                            if (elements[i - 1] == PackageDefine.WaterLevelIntervalFlag[0] && elements[i] == PackageDefine.WaterLevelIntervalFlag[1])
                            {
                                spEntity.IsOptWaterLevelInterval = true;
                                spEntity.WaterLevelInterval = Convert.ToInt32(string.Format("{0:X2}", elements[i + 1]));
                                strcontent += "水位数据存储间隔:" + spEntity.WaterLevelInterval + "min,";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 1; i++)
                        {
                            if (elements[i - 1] == PackageDefine.RainFallPrecisionFlag[0] && elements[i] == PackageDefine.RainFallPrecisionFlag[1])
                            {
                                spEntity.IsOptRainFallPrecision = true;
                                spEntity.RainFallPrecision = ConvertToDouble(string.Format("{0:X2}", elements[i + 1])) / 10;
                                strcontent += "雨量计分辨率:" + spEntity.RainFallPrecision + "mm,";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 1; i++)
                        {
                            if (elements[i - 1] == PackageDefine.WaterLevelGaugePrecisionFlag[0] && elements[i] == PackageDefine.WaterLevelGaugePrecisionFlag[1])
                            {
                                spEntity.IsOptWaterLevelGaugePrecision = true;
                                spEntity.WaterLevelGaugePrecision = ConvertToDouble(string.Format("{0:X2}", elements[i + 1])) / 10;
                                strcontent += "水位计分辨率:" + spEntity.WaterLevelGaugePrecision + "cm,";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 1; i++)
                        {
                            if (elements[i - 1] == PackageDefine.RainFallLimitFlag[0] && elements[i] == PackageDefine.RainFallLimitFlag[1])
                            {
                                spEntity.IsOptRainFallLimit = true;
                                spEntity.RainFallLimit = Convert.ToInt32(string.Format("{0:X2}", elements[i + 1]));
                                strcontent += "雨量加报阀值:" + spEntity.RainFallLimit + "mm,";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 4; i++)
                        {
                            if (elements[i - 1] == PackageDefine.WaterLevelBasicFlag[0] && elements[i] == PackageDefine.WaterLevelBasicFlag[1])
                            {
                                spEntity.IsOptWaterLevelBasic = true;
                                if (elements[i + 1] == 0xFF)  //负数5个字节、正数4个字节
                                {
                                    string tmp = string.Format("{0:X2}", elements[i + 2]) + string.Format("{0:X2}", elements[i + 3]) +
                                    string.Format("{0:X2}", elements[i + 4]) + string.Format("{0:X2}", elements[i + 5]);
                                    spEntity.WaterLevelBasic = (-1) * ConvertToDouble(tmp) / 1000;
                                    strcontent += "水位基值:" + spEntity.WaterLevelBasic + "m,";
                                }
                                else
                                {
                                    string tmp = string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2]) +
                                    string.Format("{0:X2}", elements[i + 3]) + string.Format("{0:X2}", elements[i + 4]);
                                    spEntity.WaterLevelBasic = ConvertToDouble(tmp) / 1000;
                                    strcontent += "水位基值:" + spEntity.WaterLevelBasic + "m,";
                                }
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 3; i++)
                        {
                            if (elements[i - 1] == PackageDefine.WaterLevelAmendLimitFlag[0] && elements[i] == PackageDefine.WaterLevelAmendLimitFlag[1])
                            {
                                spEntity.IsOptWaterLevelAmendLimit = true;
                                if (elements[i + 1] == 0xFF)  //负数四个字节、正数三个字节
                                {
                                    string tmp = string.Format("{0:X2}", elements[i + 2]) +
                                        string.Format("{0:X2}", elements[i + 3]) + string.Format("{0:X2}", elements[i + 4]);
                                    spEntity.WaterLevelAmendLimit = (-1) * ConvertToDouble(tmp) / 1000;
                                    strcontent += "水位修正基值:" + spEntity.WaterLevelAmendLimit + "m,";
                                }
                                else
                                {
                                    string tmp = string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2]) + string.Format("{0:X2}", elements[i + 3]);
                                    spEntity.WaterLevelAmendLimit = ConvertToDouble(tmp) / 1000;
                                    strcontent += "水位修正基值:" + spEntity.WaterLevelAmendLimit + "m,";
                                }
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 2; i++)
                        {
                            if (elements[i - 1] == PackageDefine.AddtionWaterLevelFlag[0] && elements[i] == PackageDefine.AddtionWaterLevelFlag[1])
                            {
                                spEntity.IsOptAddtionWaterLevel = true;
                                spEntity.AddtionWaterLevel = ConvertToDouble(string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / 100;
                                strcontent += "加报水位:" + spEntity.AddtionWaterLevel + "m,";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 2; i++)
                        {
                            if (elements[i - 1] == PackageDefine.AddtionWaterLevelUpLimitFlag[0] && elements[i] == PackageDefine.AddtionWaterLevelUpLimitFlag[1])
                            {
                                spEntity.IsOptAddtionWaterLevelUpLimit = true;
                                spEntity.AddtionWaterLevelUpLimit = ConvertToDouble(string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / 100;
                                strcontent += "加报水位以上加报阀值:" + spEntity.AddtionWaterLevelUpLimit + "m,";
                                break;
                            }
                        }
                        for (int i = 1; i < elements.Length - 2; i++)
                        {
                            if (elements[i - 1] == PackageDefine.AddtionWaterLevelLowLimitFlag[0] && elements[i] == PackageDefine.AddtionWaterLevelLowLimitFlag[1])
                            {
                                spEntity.IsOptAddtionWaterLevelLowLimit = true;
                                spEntity.AddtionWaterLevelLowLimit = ConvertToDouble(string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / 100;
                                strcontent += "加报水位以下加报阀值:" + spEntity.AddtionWaterLevelLowLimit + "m,";
                                break;
                            }
                        }
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.PrecipitationConstantCtrl:
                        #region 查询水量定值控制
                        if (elements.Length == 8)
                        {
                            strcontent = "水量定值控制";

                            if (elements[7] == 0x00)
                                strcontent += "投出";
                            else if (elements[7] == 0xFF)
                                strcontent += "投出";
                            spEntity.IsOptPreConstCtrl = true;
                            spEntity.PreConstCtrl = elements[7];
                        }
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.ManualSetParmReport:
                    case (byte)SL651_COMMAND.QueryManualSetParm:
                    case (byte)SL651_COMMAND.SetManualSetParm:
                        #region 查询、设置人工置数
                        for (int i = 1; i < elements.Length; i++)
                        {
                            if (elements[i - 1] == PackageDefine.ManualSetParmFlag[0] && elements[i] == PackageDefine.ManualSetParmFlag[1])
                            {
                                strcontent = "人工置数:";
                                string str_manualinfo = "";
                                for (int j = 2; j < elements.Length; j++)
                                {
                                    if (elements[j] >= 32 && elements[j] <= 127)  //ASCII打印字符范围
                                        str_manualinfo += string.Format("{0:X2}", elements[j]);
                                }
                                str_manualinfo = ConvertHelper.HexToASCII(str_manualinfo);
                                strcontent += str_manualinfo + ",";
                                spEntity.IsOptQueryManualSetParm = true;
                                spEntity.QueryManualSetParm = str_manualinfo;
                                break;
                            }
                        }
                        
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.UniformityTimeReport:  //均匀时段水文报  结构相同,都是:要素标识符,第一个数据，第二个数据...
                    case (byte)SL651_COMMAND.QueryPrecipitation:  //查询时段降水量
                        #region
                        if (elements.Length > 0 && elements[0] == PackageDefine.AddrFlagHeader[0] && elements[1] == PackageDefine.AddrFlagHeader[1])
                        {
                            strcontent = "分类码:" + "0x" + string.Format("{0:X2}", elements[7]) + ",";
                            elements = BytesRemove(elements, 8);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.ObservationTimeFlag[0] && elements[1] == PackageDefine.ObservationTimeFlag[1])
                        {
                            strcontent += "观测时间:" + string.Format("{0:X2}", elements[2]) + string.Format("{0:X2}", elements[3]) +
                                    string.Format("{0:X2}", elements[4]) + string.Format("{0:X2}", elements[5]) + string.Format("{0:X2}", elements[6]) + ",";
                            elements = BytesRemove(elements, 7);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.TimeStepFlag[0] && elements[1] == PackageDefine.TimeStepFlag[1])
                        {
                            strcontent += "时间步长:" + string.Format("{0:X2}", elements[2]) + string.Format("{0:X2}", elements[3]) +string.Format("{0:X2}", elements[4]) + ",";
                            elements = BytesRemove(elements, 5);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.RainFallLimitFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i < elements.Length; i++)
                            {
                                strcontent += "雨量加报阀值:" + ConvertToDouble(string.Format("{0:X2}", elements[i])) / Math.Pow(10, pointlen) + "mm,";  //1
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.RainFallAddupFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += "降水累计量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.InstantWaterlevelFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "瞬时水位:";
                            for (int i = 2; i + 3 < elements.Length; i += 4)
                            {
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])
                                            + string.Format("{0:X2}", elements[i + 2]) + string.Format("{0:X2}", elements[i + 3])) / Math.Pow(10, pointlen) + "m,";  //1000
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.PowerFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 1 < elements.Length; i += 2)
                            {
                                strcontent += "电源电压:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])) / Math.Pow(10, pointlen) + "v,";  //100
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.RainFallTookFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 1 < elements.Length; i += 2)
                            {
                                strcontent += "降水历时:" + string.Format("{0:X2}", elements[i]) + "." + string.Format("{0:X2}", elements[i + 1]) + ",";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.AlarmFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 3 < elements.Length; i += 4)
                            {
                                strcontent += "状态及报警信息:" + string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])
                                    + string.Format("{0:X2}", elements[i + 2]) + string.Format("{0:X2}", elements[i + 3]) + ",";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation5MinFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "1h内5min时段降水量:";
                            for (int i = 2; i < elements.Length; i++)
                            {
                                if (elements[i] == 0xff)
                                    strcontent += "--,";
                                else
                                    strcontent += Convert.ToDouble(elements[i]) / 10 + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Waterlevel5MinFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "1h内5min间隔相对水位:";
                            for (int i = 2; i + 1 < elements.Length; i += 2)
                            {
                                if (elements[i + 1] == 0xff && elements[i] == 0xff)
                                    strcontent += "--,";
                                else
                                    strcontent += (((double)BitConverter.ToUInt16(new byte[] { elements[i + 1], elements[i] }, 0)) / 100).ToString("f2") + "cm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.InstantWaterTempFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "瞬时水温:";
                            for (int i = 2; i + 1 < elements.Length; i += 2)
                            {
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])) / Math.Pow(10, pointlen) + "℃,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.PHFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 1 < elements.Length; i += 2)
                            {
                                strcontent += "PH:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])) / Math.Pow(10, pointlen) + ",";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.ConductivityFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += "电导率:" + Convert.ToInt32(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) +
                                    string.Format("{0:X2}", elements[i + 2])) + ",";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.TurbidityFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 1 < elements.Length; i += 2)
                            {
                                strcontent += "浊度:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1])) + ",";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.PrecipitationFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += "当前降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.DayPrecipitationFlag[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += "日降水量:" + ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation1min[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "1min降水量:";
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation5min[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "5min降水量:";
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation10min[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "10min降水量:";
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation30min[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "30min降水量:";
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation1h[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "1h降水量:";
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation2h[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "2h降水量:";
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation3h[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "3h降水量:";
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation6h[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "6h降水量:";
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent +=  ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        if (elements.Length > 0 && elements[0] == PackageDefine.Precipitation12h[0])
                        {
                            GetSumLen(elements[1], out sumlen, out pointlen);
                            strcontent += "12h降水量:";
                            for (int i = 2; i + 2 < elements.Length; i += 3)
                            {
                                strcontent += ConvertToDouble(string.Format("{0:X2}", elements[i]) + string.Format("{0:X2}", elements[i + 1]) + string.Format("{0:X2}", elements[i + 2])) / Math.Pow(10, pointlen) + "mm,";
                            }
                            elements = BytesRemove(elements, sumlen + 2);
                        }
                        #endregion
                        break;
                    case (byte)SL651_COMMAND.ReadTimeIntervalReportTime:
                        #region 读取均匀时段报上传时间
                        if (elements.Length == 8)
                        {
                            spEntity.IsOptTimeintervalReportTime = true;
                            spEntity.TimeintervalReportTime = Convert.ToInt32(elements[7]);
                        }
                        #endregion
                        break;
                }
                if (string.IsNullOrEmpty(strcontent))
                    strcontent = "要素信息:" + ConvertHelper.ByteToString(elements, elements.Length) + ",";
            }
            catch (Exception ex)
            {
                strcontent = "解帧发生异常,ex:" + ex.Message+",部分解帧信息:"+strcontent;
                logger.ErrorException("AnalyseElement1", ex);
            }

            return strcontent;
        }

        /// <summary>
        /// 获得编码标识符表示数据字节数和小数点后位数
        /// </summary>
        /// <param name="b">编码标识符</param>
        /// <param name="sumlen">数据字节数</param>
        /// <param name="pointlen">小数点后位数</param>
        private static void GetSumLen(byte b,out int sumlen,out int pointlen)
        {
            sumlen = Convert.ToInt32(b >> 3);
            pointlen = Convert.ToInt32(b & 0x07);
        }

        private static double ConvertToDouble(string str)
        {
            if (str == "")
                return 0;
            bool isigore = true;
            foreach (char c in str.ToCharArray())
            {
                if (c != 'F')   //如果str全部都是 FF这样的数据，认为是非法数据,返回成-1
                {
                    isigore = false;
                    break;
                }
            }
            if (!isigore)
            {
                return Convert.ToDouble(str);
            }
            else
                return -1;
        }
        /// <summary>
        /// 移除source从索引0开始长度len的字节数组
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startindex"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private static byte[] BytesRemove(byte[] source, int len)
        {
            if (source.Length < len)
                return source;
            byte[] target = new byte[source.Length - len];
            for (int i = 0; i < source.Length - len; i++)
            {
                target[i] = source[i + len];
            }
            return target;
        }
    }
}
