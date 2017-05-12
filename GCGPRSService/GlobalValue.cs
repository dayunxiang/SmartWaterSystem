﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Entity;
using System.Data;
using SmartWaterSystem;
using System.IO;
using System.Collections.Concurrent;

namespace GCGPRSService
{
    public class GlobalValue
    {
        private static GlobalValue _instance;
        public static GlobalValue Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GlobalValue();

                return _instance;
            }
        }

        #region 客户端列表
        /// <summary>
        /// 更新lstClient锁
        /// </summary>
        public object lstClientLock = new object();
        /// <summary>
        /// 客户端列表
        /// </summary>
        public List<CallSocketEntity> lstClient = new List<CallSocketEntity>();  //客户端列表

        /// <summary>
        /// 搜索客户端列表,获取索引位置,未找到返回-1
        /// </summary>
        /// <param name="TerId">终端号</param>
        /// <param name="DevType">终端类型</param>
        public int GetlstClientIndex(short TerId, ConstValue.DEV_TYPE DevType)
        {
            for (int i = 0; i < lstClient.Count; i++)
            {
                if (lstClient[i].TerId == TerId && lstClient[i].DevType == DevType)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 如果已存在,返回已存在的对象索引，否则添加一个
        /// </summary>
        /// <param name="TerId">终端号</param>
        /// <param name="DevType">终端类型</param>
        public int lstClientAdd(short TerId, ConstValue.DEV_TYPE DevType)
        {
            lock (lstClientLock)
            {
                bool isExist = false;
                int index = -1;
                for (int i = 0; i < lstClient.Count; i++)
                {
                    if (lstClient[i].TerId == TerId && lstClient[i].DevType == DevType)
                    {
                        index = i;
                        isExist = true;
                    }
                }
                if (!isExist)    //不存在先新建对象
                {
                    CallSocketEntity callEnt = new CallSocketEntity();
                    callEnt.TerId = TerId;
                    callEnt.DevType = DevType;
                    lstClient.Add(callEnt);
                    index = lstClient.Count - 1;
                }
                return index;
            }
        }

        /// <summary>
        /// 添加到客户端列表,如果存在,直接返回所在索引值，没有就添加
        /// </summary>
        /// <param name="TerId">终端号</param>
        /// <param name="DevType">终端类型</param>
        /// <param name="TableId">表ID,请使用枚举PackFromType  -99:SL651； P68:-1:校时数据,-2:下送命令帧,>-1:数据库中获取的命令帧ID</param>
        /// <param name="pack">带发送命令包</param>
        /// <param name="RemoveSame">是否移除相同功能码命令包</param>
        public int lstClientAdd(short TerId, ConstValue.DEV_TYPE DevType,int TableId,Package pack,bool RemoveSame=true)
        {
            lock (lstClientLock)
            {
                bool isExist = false;
                int index = -1;
                for (int i = 0; i < lstClient.Count; i++)
                {
                    if (lstClient[i].TerId == TerId && lstClient[i].DevType == DevType)
                    {
                        isExist = true;
                        index = i;
                    }
                }
                if (!isExist)    //不存在先新建对象
                {
                    CallSocketEntity callEnt = new CallSocketEntity();
                    callEnt.TerId = TerId;
                    callEnt.DevType = DevType;
                    SendPackageEntity sendPack = new SendPackageEntity();
                    sendPack.TableId = TableId;
                    sendPack.SendPackage = pack;
                    callEnt.lstWaitSendCmd.Add(sendPack);
                    lstClient.Add(callEnt);
                    index = lstClient.Count - 1;
                }
                else
                {
                    if (RemoveSame)
                    {
                        for (int j = lstClient[index].lstWaitSendCmd.Count-1;j>=0; j--)   //先移除相同功能码的命令
                        {
                            if (lstClient[index].lstWaitSendCmd[j].SendPackage != null && lstClient[index].lstWaitSendCmd[j].SendPackage.C1 == pack.C1)
                                lstClient[index].lstWaitSendCmd.RemoveAt(j);
                        }
                    }
                    SendPackageEntity sendPack = new SendPackageEntity();   //再添加
                    sendPack.TableId = TableId;
                    sendPack.SendPackage = pack;
                    lstClient[index].lstWaitSendCmd.Add(sendPack);
                }
                return index;
            }
        }

        /// <summary>
        /// 从客户端列表中移除命令包
        /// </summary>
        /// <param name="TerId"></param>
        /// <param name="DevType"></param>
        /// <param name="FunCode"></param>
        public void lstClientRemove(short TerId, ConstValue.DEV_TYPE DevType,byte FunCode)
        {
            lock(lstClientLock)
            {
                for (int i = 0; i < lstClient.Count; i++)
                {
                    if (lstClient[i].TerId == TerId && lstClient[i].DevType == DevType)
                    {
                        for (int j = lstClient[i].lstWaitSendCmd.Count-1;j>=0; j--)
                        {
                            if (lstClient[i].lstWaitSendCmd[j].SendPackage != null && lstClient[i].lstWaitSendCmd[j].SendPackage.C1 == FunCode)
                            {
                                lstClient[i].lstWaitSendCmd.RemoveAt(j);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 添加至客户端列表
        /// </summary>
        /// <param name="sendEntity"></param>
        public void lstClientAdd(SendPackageEntity sendEntity)
        {
            lock(lstClientLock)
            {
                short terid = sendEntity.SendPackage.DevID;
                ConstValue.DEV_TYPE devtype = sendEntity.SendPackage.DevType;
                bool isExist = false;
                int index = -1;
                for (int i = 0; i < lstClient.Count; i++)
                {
                    if (lstClient[i].TerId == terid && lstClient[i].DevType == devtype)
                    {
                        isExist = true;
                        index = i;
                    }
                }
                if (!isExist)    //不存在先新建对象
                {
                    CallSocketEntity callEnt = new CallSocketEntity();
                    callEnt.TerId = terid;
                    callEnt.DevType = devtype;

                    callEnt.lstWaitSendCmd.Add(sendEntity);
                    lstClient.Add(callEnt);
                    return;
                }
                else
                {
                    lstClient[index].lstWaitSendCmd.Add(sendEntity);
                }
            }
        }

        #endregion


        /// <summary>
        /// //启动记录,用于smartsocket连接过来的时候将启动记录发送过去
        /// </summary>
        public List<string> lstStartRecord = new List<string>();
        public string SmartWaterHeartBeatName = "heartbeat";

        /// <summary>
        /// HTTP 服务
        /// </summary>
        public HTTPService HttpService = new HTTPService();

        /// <summary>
        /// GPRS远传数据操作管理线程
        /// </summary>
        public SocketSQLManager SocketSQLMag = new SocketSQLManager();
        /// <summary>
        /// GPRS远传服务线程
        /// </summary>
        public SocketManager SocketMag = new SocketManager();

        private ConcurrentQueue<GPRSPreFrameDataEntity> _GPRS_PreFrameData = new ConcurrentQueue<GPRSPreFrameDataEntity>();
        /// <summary>
        /// GPRS压力帧队列
        /// </summary>
        public ConcurrentQueue<GPRSPreFrameDataEntity> GPRS_PreFrameData
        {
            get { return _GPRS_PreFrameData; }
            set { _GPRS_PreFrameData = value; }
        }

        private ConcurrentQueue<GPRSFlowFrameDataEntity> _GPRS_FlowFrameData = new ConcurrentQueue<GPRSFlowFrameDataEntity>();
        /// <summary>
        /// GPRS流量帧数据
        /// </summary>
        public ConcurrentQueue<GPRSFlowFrameDataEntity> GPRS_FlowFrameData
        {
            get { return _GPRS_FlowFrameData; }
            set { _GPRS_FlowFrameData = value; }
        }

        private ConcurrentQueue<GPRSUniversalFrameDataEntity> _GPRS_UniversalFrameData = new ConcurrentQueue<GPRSUniversalFrameDataEntity>();
        /// <summary>
        /// GPRS通用终端帧数据
        /// </summary>
        public ConcurrentQueue<GPRSUniversalFrameDataEntity> GPRS_UniversalFrameData
        {
            get { return _GPRS_UniversalFrameData; }
            set { _GPRS_UniversalFrameData = value; }
        }

        private ConcurrentQueue<GPRSOLWQFrameDataEntity> _GPRS_OLWQFrameData = new ConcurrentQueue<GPRSOLWQFrameDataEntity>();
        /// <summary>
        /// GPRS水质终端帧队列
        /// </summary>
        public ConcurrentQueue<GPRSOLWQFrameDataEntity> GPRS_OLWQFrameData
        {
            get { return _GPRS_OLWQFrameData; }
            set { _GPRS_OLWQFrameData = value; }
        }

        private ConcurrentQueue<GPRSHydrantFrameDataEntity> _GPRS_HydrantFrameData = new ConcurrentQueue<GPRSHydrantFrameDataEntity>();
        /// <summary>
        /// 消防栓帧队列
        /// </summary>
        public ConcurrentQueue<GPRSHydrantFrameDataEntity> GPRS_HydrantFrameData
        {
            get { return _GPRS_HydrantFrameData; }
            set { _GPRS_HydrantFrameData = value; }
        }

        private ConcurrentQueue<GPRSPrectrlFrameDataEntity> _GPRS_PrectrlFrameData = new ConcurrentQueue<GPRSPrectrlFrameDataEntity>();
        /// <summary>
        /// 压力控制器帧队列
        /// </summary>
        public ConcurrentQueue<GPRSPrectrlFrameDataEntity> GPRS_PrectrlFrameData
        {
            get { return _GPRS_PrectrlFrameData; }
            set { _GPRS_PrectrlFrameData = value; }
        }

        private ConcurrentQueue<GPRSNoiseFrameDataEntity> _GPRS_NoiseFrameData = new ConcurrentQueue<GPRSNoiseFrameDataEntity>();
        /// <summary>
        /// 噪声远传控制器帧队列
        /// </summary>
        public ConcurrentQueue<GPRSNoiseFrameDataEntity> GPRS_NoiseFrameData
        {
            get { return _GPRS_NoiseFrameData; }
            set { _GPRS_NoiseFrameData = value; }
        }

        private ConcurrentQueue<GPRSWaterWorkerFrameDataEntity> _GPRS_WaterworkerFrameData = new ConcurrentQueue<GPRSWaterWorkerFrameDataEntity>();
        /// <summary>
        /// GPRS水厂帧队列
        /// </summary>
        public ConcurrentQueue<GPRSWaterWorkerFrameDataEntity> GPRS_WaterworkerFrameData
        {
            get { return _GPRS_WaterworkerFrameData; }
            set { _GPRS_WaterworkerFrameData = value; }
        }

        private ConcurrentQueue<GPRSAlarmFrameDataEntity> _GPRS_AlarmFrameData = new ConcurrentQueue<GPRSAlarmFrameDataEntity>();
        /// <summary>
        /// GPRS报警帧队列
        /// </summary>
        public ConcurrentQueue<GPRSAlarmFrameDataEntity> GPRS_AlarmFrameData
        {
            get { return _GPRS_AlarmFrameData; }
            set { _GPRS_AlarmFrameData = value; }
        }

        /// <summary>
        /// 通用终端配置，用于帧数据解析
        /// </summary>
        public DataTable UniversalDataConfig = null;

        private List<GPRSCmdFlag> _lstSendedCmdId = new List<GPRSCmdFlag>();
        /// <summary>
        /// GPRS下发命令后对应的数据库表,用于更新数据库
        /// </summary>
        public List<GPRSCmdFlag> lstSendedCmdId
        {
            get { return _lstSendedCmdId; }
            set { _lstSendedCmdId = value; }
        }

        private Dictionary<int, string> _lstAlarmType = new Dictionary<int, string>();
        /// <summary>
        /// 报警类型表,对应数据库中AlarmType表
        /// </summary>
        public Dictionary<int, string> lstAlarmType
        {
            get { return _lstAlarmType; }
            set { _lstAlarmType = value; }
        }

        private Dictionary<string,float> _lstOffsetValue = new Dictionary<string, float>();
        /// <summary>
        /// 偏移值表,对应数据库中OffsetValue表,key按照TerminalID+TerminalType+Funcode拼接
        /// </summary>
        public Dictionary<string, float> lstOffsetValue
        {
            get { return _lstOffsetValue; }
            set { _lstOffsetValue = value; }
        }
    }
}
