﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using Entity;
using Common;
using System.Data.SQLite;

namespace DAL
{
    public class TerminalDataDAL
    {
        public DataTable GetTerID_PointID(TerType type)
        {
            string SQL = "SELECT ID,TerminalID,TerminalName,Address,Remark,ModifyTime FROM Terminal WHERE TerminalType='" + (int)type + "'";
            return SQLHelper.ExecuteDataTable(SQL, null);
        }

        public bool TerminalExist(TerType type,string TerID)
        {
            string SQL = "SELECT COUNT(1) FROM Terminal WHERE TerminalType='" + (int)type + "' AND TerminalID='"+TerID+"'";
            object obj_count = SQLHelper.ExecuteScalar(SQL, null);
            if (obj_count != DBNull.Value && obj_count != null)
                return Convert.ToInt32(obj_count) > 0 ? true : false;

            return false;
        }

        #region GPRS数据操作
        public int InsertGPRSPreData(Queue<GPRSPreFrameDataEntity> datas)
        {
            lock (ConstValue.obj)
            {
                SqlTransaction trans = null;
                try
                {
                    trans = SQLHelper.GetTransaction();

                    string SQL_Frame = "INSERT INTO Frame(Dir,Frame,LogTime) VALUES(@dir,@frame,@logtime)";
                    SqlParameter[] parms_frame = new SqlParameter[]{
                new SqlParameter("@dir",SqlDbType.Int),
                new SqlParameter("@frame",SqlDbType.VarChar,2000),
                new SqlParameter("@logtime",SqlDbType.DateTime)
            };
                    SqlCommand command_frame = new SqlCommand();
                    command_frame.CommandText = SQL_Frame;
                    command_frame.Parameters.AddRange(parms_frame);
                    command_frame.CommandType = CommandType.Text;
                    command_frame.Connection = SQLHelper.Conn;
                    command_frame.Transaction = trans;

                    string SQL_PreData = "INSERT INTO Pressure_Real(TerimanlID,PressValue,CollTime,UnloadTime,HistoryFlag) VALUES(@TerId,@prevalue,@coltime,@UploadTime,0)";
                    SqlParameter[] parms_predata = new SqlParameter[]{
                    new SqlParameter("@TerId",SqlDbType.Int),
                    new SqlParameter("@prevalue",SqlDbType.Decimal),
                    new SqlParameter("@coltime",SqlDbType.DateTime),
                    new SqlParameter("@UploadTime",SqlDbType.DateTime)
                };
                    SqlCommand command_predata = new SqlCommand();
                    command_predata.CommandText = SQL_PreData;
                    command_predata.Parameters.AddRange(parms_predata);
                    command_predata.CommandType = CommandType.Text;
                    command_predata.Connection = SQLHelper.Conn;
                    command_predata.Transaction = trans;

                    //string en_point_id = "";
                    while (datas.Count > 0)
                    {
                        GPRSPreFrameDataEntity entity = datas.Dequeue();
                        if (entity == null)
                        {
                            break;
                        }
                        parms_frame[0].Value = 1;
                        parms_frame[1].Value = entity.Frame;
                        parms_frame[2].Value = entity.ModifyTime;

                        command_frame.ExecuteNonQuery();

                        //en_point_id = "";
                        //foreach (DataRow dr in dt_PointID.Rows)
                        //{
                        //    if (dr["TerminalID"].ToString() == entity.TerId)
                        //    {
                        //        en_point_id = dr["ID"].ToString();
                        //        break;
                        //    }
                        //}

                        //if (!string.IsNullOrEmpty(en_point_id))
                        //{
                            for (int i = 0; i < entity.lstPreData.Count; i++)
                            {
                                parms_predata[0].Value = entity.TerId;
                                parms_predata[1].Value = entity.lstPreData[i].PreValue;
                                parms_predata[2].Value = entity.lstPreData[i].ColTime;
                                parms_predata[3].Value = entity.ModifyTime;

                                command_predata.ExecuteNonQuery();
                            }
                        //}
                    }
                    trans.Commit();

                    return 1;
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw ex;
                }
            }
        }

        public int InsertGPRSFlowData(Queue<GPRSFlowFrameDataEntity> datas)
        {
            lock (ConstValue.obj)
            {
                SqlTransaction trans = null;
                try
                {
                    //DataTable dt_PointID = GetTerID_PointID(TerType.PreTer);
                    //if ((dt_PointID == null) || (dt_PointID.Rows.Count == 0))
                    //{
                    //    return 0;
                    //}
                    trans = SQLHelper.GetTransaction();

                    string SQL_Frame = "INSERT INTO Frame(Dir,Frame,LogTime) VALUES(@dir,@frame,@logtime)";
                    SqlParameter[] parms_frame = new SqlParameter[]{
                new SqlParameter("@dir",SqlDbType.Int),
                new SqlParameter("@frame",SqlDbType.VarChar,2000),
                new SqlParameter("@logtime",SqlDbType.DateTime)
            };
                    SqlCommand command_frame = new SqlCommand();
                    command_frame.CommandText = SQL_Frame;
                    command_frame.Parameters.AddRange(parms_frame);
                    command_frame.CommandType = CommandType.Text;
                    command_frame.Connection = SQLHelper.Conn;
                    command_frame.Transaction = trans;

                    string SQL_PreData = "INSERT INTO Flow_Real(TerminalID,FlowValue,FlowInverted,FlowInstant,CollTime,UnloadTime,HistoryFlag) VALUES(@TerId,@flowvalue,@flowreverse,@flowinstant,@coltime,@UploadTime,0)";
                    SqlParameter[] parms_predata = new SqlParameter[]{
                    new SqlParameter("@TerId",SqlDbType.Int),
                    new SqlParameter("@flowvalue",SqlDbType.Decimal),
                    new SqlParameter("@flowreverse",SqlDbType.Decimal),
                    new SqlParameter("@flowinstant",SqlDbType.Decimal),
                    new SqlParameter("@coltime",SqlDbType.DateTime),
                    new SqlParameter("@UploadTime",SqlDbType.DateTime)
                };
                    SqlCommand command_predata = new SqlCommand();
                    command_predata.CommandText = SQL_PreData;
                    command_predata.Parameters.AddRange(parms_predata);
                    command_predata.CommandType = CommandType.Text;
                    command_predata.Connection = SQLHelper.Conn;
                    command_predata.Transaction = trans;

                    //string en_point_id = "";
                    while (datas.Count > 0)
                    {
                        GPRSFlowFrameDataEntity entity = datas.Dequeue();
                        parms_frame[0].Value = 1;
                        parms_frame[1].Value = entity.Frame;
                        parms_frame[2].Value = entity.ModifyTime;

                        command_frame.ExecuteNonQuery();

                        //en_point_id = "";
                        //foreach (DataRow dr in dt_PointID.Rows)
                        //{
                        //    if (dr["TerminalID"].ToString() == entity.TerId)
                        //    {
                        //        en_point_id = dr["ID"].ToString();
                        //        break;
                        //    }
                        //}
                        //if (!string.IsNullOrEmpty(en_point_id))
                        //{
                            for (int i = 0; i < entity.lstFlowData.Count; i++)
                            {
                                parms_predata[0].Value = entity.TerId;
                                parms_predata[1].Value = entity.lstFlowData[i].Forward_FlowValue;
                                parms_predata[2].Value = entity.lstFlowData[i].Reverse_FlowValue;
                                parms_predata[3].Value = entity.lstFlowData[i].Instant_FlowValue;
                                parms_predata[4].Value = entity.lstFlowData[i].ColTime;
                                parms_predata[5].Value = entity.ModifyTime;

                                command_predata.ExecuteNonQuery();
                            }
                        //}
                    }
                    trans.Commit();

                    return 1;
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw ex;
                }
            }
        }

        public int InsertGPRSUniversalData(Queue<GPRSUniversalFrameDataEntity> datas)
        {
            lock (ConstValue.obj)
            {
                SqlTransaction trans = null;
                try
                {
                    trans = SQLHelper.GetTransaction();

                    string SQL_Frame = "INSERT INTO Frame(Dir,Frame,LogTime) VALUES(@dir,@frame,@logtime)";
                    SqlParameter[] parms_frame = new SqlParameter[]{
                new SqlParameter("@dir",SqlDbType.Int),
                new SqlParameter("@frame",SqlDbType.VarChar,2000),
                new SqlParameter("@logtime",SqlDbType.DateTime)
            };
                    SqlCommand command_frame = new SqlCommand();
                    command_frame.CommandText = SQL_Frame;
                    command_frame.Parameters.AddRange(parms_frame);
                    command_frame.CommandType = CommandType.Text;
                    command_frame.Connection = SQLHelper.Conn;
                    command_frame.Transaction = trans;

                    string SQL_Data = @"INSERT INTO UniversalTerData([TerminalID],[Simulate1],[Simulate2],[Simulate1Zero],[Simulate2Zero],
                                            [Pluse1],[Pluse2],[Pluse3],[Pluse4],[Pluse5],[RS485_1],[RS485_2],
                                            [RS485_3],[RS485_4],[RS485_5],[RS485_6],[RS485_7],[RS485_8],[CollTime],[UnloadTime],TypeTableID,TableColumnName) 
                                            VALUES(@terId,@sim1,@sim2,@sim1zero,@sim2zero,@pluse1,@pluse2,@pluse3,@pluse4,@pluse5,
                                            @rs4851,@rs4852,@rs4853,@rs4854,@rs4855,@rs4856,@rs4857,@rs4858,@coltime,@UploadTime,@tableid,@columnname)";
                    SqlParameter[] parms_data = new SqlParameter[]{
                    new SqlParameter("@terId",SqlDbType.Int),
                    new SqlParameter("@sim1",SqlDbType.Decimal),
                    new SqlParameter("@sim2",SqlDbType.Decimal),
                    new SqlParameter("@sim1zero",SqlDbType.Decimal),
                    new SqlParameter("@sim2zero",SqlDbType.Decimal),

                    new SqlParameter("@pluse1",SqlDbType.Decimal),
                    new SqlParameter("@pluse2",SqlDbType.Decimal),
                    new SqlParameter("@pluse3",SqlDbType.Decimal),
                    new SqlParameter("@pluse4",SqlDbType.Decimal),
                    new SqlParameter("@pluse5",SqlDbType.Decimal),

                    new SqlParameter("@rs4851",SqlDbType.Decimal),
                    new SqlParameter("@rs4852",SqlDbType.Decimal),
                    new SqlParameter("@rs4853",SqlDbType.Decimal),
                    new SqlParameter("@rs4854",SqlDbType.Decimal),
                    new SqlParameter("@rs4855",SqlDbType.Decimal),

                    new SqlParameter("@rs4856",SqlDbType.Decimal),
                    new SqlParameter("@rs4857",SqlDbType.Decimal),
                    new SqlParameter("@rs4858",SqlDbType.Decimal),
                    new SqlParameter("@coltime",SqlDbType.DateTime),
                    new SqlParameter("@UploadTime",SqlDbType.DateTime),

                    new SqlParameter("@tableid",SqlDbType.Int),
                    new SqlParameter("@columnname",SqlDbType.NVarChar,20)
                };
                    SqlCommand command_predata = new SqlCommand();
                    command_predata.CommandText = SQL_Data;
                    command_predata.Parameters.AddRange(parms_data);
                    command_predata.CommandType = CommandType.Text;
                    command_predata.Connection = SQLHelper.Conn;
                    command_predata.Transaction = trans;

                    while (datas.Count > 0)
                    {
                        GPRSUniversalFrameDataEntity entity = datas.Dequeue();
                        parms_frame[0].Value = 1;
                        parms_frame[1].Value = entity.Frame;
                        parms_frame[2].Value = entity.ModifyTime;

                        command_frame.ExecuteNonQuery();

                        for (int i = 0; i < entity.lstData.Count; i++)
                        {
                            parms_data[0].Value = entity.TerId;
                            parms_data[1].Value = entity.lstData[i].Sim1;
                            parms_data[2].Value = entity.lstData[i].Sim2;
                            parms_data[3].Value = entity.lstData[i].Sim1Zero;
                            parms_data[4].Value = entity.lstData[i].Sim2Zero;

                            parms_data[5].Value = entity.lstData[i].Pluse1;
                            parms_data[6].Value = entity.lstData[i].Pluse2;
                            parms_data[7].Value = entity.lstData[i].Pluse3;
                            parms_data[8].Value = entity.lstData[i].Pluse4;
                            parms_data[9].Value = entity.lstData[i].Pluse5;

                            parms_data[10].Value = entity.lstData[i].RS4851;
                            parms_data[11].Value = entity.lstData[i].RS4852;
                            parms_data[12].Value = entity.lstData[i].RS4853;
                            parms_data[13].Value = entity.lstData[i].RS4854;
                            parms_data[14].Value = entity.lstData[i].RS4855;

                            parms_data[15].Value = entity.lstData[i].RS4856;
                            parms_data[16].Value = entity.lstData[i].RS4857;
                            parms_data[17].Value = entity.lstData[i].RS4858;
                            parms_data[18].Value = entity.lstData[i].ColTime;
                            parms_data[19].Value = entity.ModifyTime;

                            parms_data[20].Value = entity.lstData[i].TypeTableID;
                            parms_data[21].Value = entity.lstData[i].TableColumnName;

                            command_predata.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取需要下发的参数
        /// </summary>
        /// <returns></returns>
        public List<GPRSCmdEntity> GetGPRSParm()
        {
            lock (ConstValue.obj)
            {
                //SELECT * FROM DL_ParamToDev WHERE ID IN (SELECT MAX(ID) FROM DL_ParamToDev WHERE SendedFlag=0 GROUP BY DeviceId)
                string SQL = "SELECT ID,DeviceId,DevTypeId,CtrlCode,FunCode,DataValue,DataLenth,SetDate FROM ParamToDev WHERE SendedFlag = 0";
                List<GPRSCmdEntity> lstCmd = null;
                using (SqlDataReader reader = SQLHelper.ExecuteReader(SQL, null))
                {
                    lstCmd = new List<GPRSCmdEntity>();
                    while (reader.Read())
                    {
                        GPRSCmdEntity cmd = new GPRSCmdEntity();

                        cmd.TableId = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : -1;
                        cmd.DeviceId = reader["DeviceId"] != DBNull.Value ? Convert.ToInt32(reader["DeviceId"]) : -1;
                        cmd.DevTypeId = reader["DevTypeId"] != DBNull.Value ? Convert.ToInt32(reader["DevTypeId"]) : -1;
                        cmd.CtrlCode = reader["CtrlCode"] != DBNull.Value ? Convert.ToInt32(reader["CtrlCode"]) : -1;
                        cmd.FunCode = reader["FunCode"] != DBNull.Value ? Convert.ToInt32(reader["FunCode"]) : -1;
                        cmd.Data = reader["DataValue"] != DBNull.Value ? reader["DataValue"].ToString() : "";

                        cmd.DataLen = reader["DataLenth"] != DBNull.Value ? Convert.ToInt32(reader["DataLenth"]) : -1;
                        cmd.ModifyTime = reader["SetDate"] != DBNull.Value ? Convert.ToDateTime(reader["SetDate"]) : DateTime.Now;
                        cmd.SendedFlag = 0;

                        lstCmd.Add(cmd);
                    }
                }

                return lstCmd;
            }
        }

        public int UpdateGPRSParmFlag(List<GPRSCmdFlag> ids)
        {
            lock (ConstValue.obj)
            {
                SqlTransaction trans = null;
                try
                {
                    if ((ids == null) || (ids.Count == 0))
                    {
                        return 0;
                    }

                    trans = SQLHelper.GetTransaction();

                    string SQL = "UPDATE ParamToDev SET SendedFlag=1 WHERE ID=@id";
                    SqlParameter parm =  new SqlParameter("@id",SqlDbType.Int);

                    SqlCommand command = new SqlCommand();
                    command.CommandText = SQL;
                    command.Parameters.Add(parm);
                    command.CommandType = CommandType.Text;
                    command.Connection = SQLHelper.Conn;
                    command.Transaction = trans;

                    for (int i = 0; i<ids.Count; i++)
                    {
                        parm.Value = ids[i].TableId;

                        command.ExecuteNonQuery();
                    }
                    trans.Commit();

                    return 1;
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw ex;
                }
            }
        }
        #endregion

        public DataTable GetTerInfo(TerType type)
        {
            string SQL = "SELECT ID,TerminalID,TerminalName,Address,Remark,ModifyTime FROM Terminal WHERE SyncState<>-1 AND TerminalType='" + (int)type + "'";
            return SQLiteHelper.ExecuteDataTable(SQL, null);
        }

        /// <summary>
        /// 查找指定类型的终端是否存在,-1:查找发生异常,0:不存在,1:存在
        /// </summary>
        /// <param name="type"></param>
        /// <param name="TerminalID"></param>
        /// <returns></returns>
        public int GetTerExist(TerType type, int TerminalID)
        {
            string SQL = "SELECT COUNT(1) FROM Terminal WHERE TerminalType='" + (int)type + "' AND TerminalID='"+TerminalID+"'";
            object obj = SQLiteHelper.ExecuteScalar(SQL, null);
            if (obj != null && obj != DBNull.Value)
            {
                return Convert.ToInt32(obj) > 0 ? 1 : 0;
            }
            else
            {
                return 0;
            }
        }

        public void DeleteTer(TerType type, int TerminalID)
        {
            string SQL = "";
            string SQL_SELECT = "SELECT COUNT(1) FROM Terminal WHERE TerminalType='" + (int)type + "' AND TerminalID='" + TerminalID + "'";
            object obj_exist = SQLiteHelper.ExecuteScalar(SQL_SELECT, null);
            bool exist = false;
            if (obj_exist != null && obj_exist != DBNull.Value)
            {
                exist = (Convert.ToInt32(obj_exist) > 0 ? true : false);
            }
            if (exist)
                SQL = "DELETE FROM Terminal WHERE TerminalType='" + (int)type + "' AND TerminalID='" + TerminalID + "'";
            else
                SQL = "UPDATE Terminal SET SyncState=-1 WHERE TerminalType='" + (int)type + "' AND TerminalID='" + TerminalID + "'";
            SQLiteHelper.ExecuteNonQuery(SQL, null);
        }

        public int GetTerminalTableMaxId()
        {
            string SQL = "SELECT MAX(id) FROM Terminal";
            object obj = SQLiteHelper.ExecuteScalar(SQL, null);
            if (obj != null && obj != DBNull.Value)
            {
                return Convert.ToInt32(obj);
            }
            else
            {
                return 0;
            }
        }

        public int GetUniversalTerWayConfigTableMaxId()
        {
            string SQL = "SELECT MAX(id) FROM UniversalTerWayConfig";
            object obj = SQLiteHelper.ExecuteScalar(SQL, null);
            if (obj != null && obj != DBNull.Value)
            {
                return Convert.ToInt32(obj);
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// 保存通用终端配置
        /// </summary>
        /// <returns></returns>
        public int SaveUniversalTerConfig(int terminalid, string name, string addr, string remark, List<UniversalWayTypeConfigEntity> lstPointID)
        {
            SQLiteTransaction trans = null;
            try
            {
                trans = SQLiteHelper.GetTransaction();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = SQLiteHelper.Conn;
                command.Transaction = trans;

                command.CommandText = "DELETE FROM Terminal WHERE SyncState<>0 AND TerminalType='" + (int)TerType.UniversalTer + "' AND TerminalID='" + terminalid + "'";
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE Terminal SET SyncState=-1 WHERE TerminalType='" + (int)TerType.UniversalTer + "' AND TerminalID='" + terminalid + "'";
                command.ExecuteNonQuery();

                command.CommandText = string.Format("INSERT INTO Terminal(ID,TerminalID,TerminalName,TerminalType,Address,Remark) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')",
                                                         GetTerminalTableMaxId() + 1, terminalid, name, (int)TerType.UniversalTer, addr, remark);
                command.ExecuteNonQuery();

                //Update UniversalTerConfig Table

                //Update UniversalTerWayConfig Table
                if (lstPointID != null && lstPointID.Count > 0)
                {
                    //UniversalTerWayConfig ID TerminalID PointID
                    command.CommandText = "DELETE FROM UniversalTerWayConfig WHERE SyncState<>0 AND TerminalID='" + terminalid + "'";
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE UniversalTerWayConfig SET SyncState=-1 WHERE TerminalID='" + terminalid + "'";
                    command.ExecuteNonQuery();

                    int configeMaxId = GetUniversalTerWayConfigTableMaxId();
                    foreach (UniversalWayTypeConfigEntity config in lstPointID)
                    {
                        configeMaxId++;
                        command.CommandText = string.Format("INSERT INTO UniversalTerWayConfig(ID,TerminalID,Sequence,PointID) VALUES('{0}','{1}','{2}','{3}')",
                            configeMaxId, terminalid,config.Sequence, config.PointID);
                        command.ExecuteNonQuery();
                    }
                }

                trans.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();
                return -1;
            }
        }

        public void DeleteUniversalWayTypeConfig(int PointID)
        {
            string SQL = "DELETE FROM UniversalTerWayConfig WHERE PointID='" + PointID + "'";
            SQLiteHelper.ExecuteNonQuery(SQL, null);
        }

        public void DeleteUniversalWayTypeConfig_TerID(int TerminalID)
        {
            string SQL_Ter = "SELECT Distinct PointID FROM UniversalTerWayConfig WHERE TerminalID='"+TerminalID+"'";
            List<string> lstPoint = new List<string>();
            using (SQLiteDataReader reader = SQLiteHelper.ExecuteReader(SQL_Ter, null))
            {
                while (reader.Read())
                {
                    lstPoint.Add(reader["PointID"].ToString());
                }
            }
            if (lstPoint != null && lstPoint.Count > 0)
            {
                foreach (string pointid in lstPoint)
                {
                    string SQL = "";
                    string SQL_SELECT = "SELECT COUNT(1) FROM UniversalTerWayConfig WHERE SyncState=-1 AND PointID='" + pointid + "' AND TerminalID='" + TerminalID + "'";
                    object obj_exist = SQLiteHelper.ExecuteScalar(SQL_SELECT, null);
                    bool exist = false;
                    if (obj_exist != null && obj_exist != DBNull.Value)
                    {
                        exist = (Convert.ToInt32(obj_exist) > 0 ? true : false);
                    }
                    if (exist)
                        SQL = "DELETE FROM UniversalTerWayConfig WHERE PointID='" + pointid + "' AND TerminalID='" + TerminalID + "'";
                    else
                        SQL = "UPDATE UniversalTerWayConfig SET SyncState=-1 WHERE PointID='" + pointid + "' AND TerminalID='" + TerminalID + "'";
                    SQLiteHelper.ExecuteNonQuery(SQL, null);
                }
            }
        }

        public List<UniversalWayTypeConfigEntity> GetUniversalWayTypeConfig(int TerminalID)
        {
            string SQL = "SELECT id,Sequence,PointID,SyncState,ModifyTime FROM UniversalTerWayConfig WHERE TerminalID AND SyncState!=-1";
            
            using (SQLiteDataReader reader = SQLiteHelper.ExecuteReader(SQL, null))
            {
                List<UniversalWayTypeConfigEntity> lstWayTypeConfig = new List<UniversalWayTypeConfigEntity>();
                while (reader.Read())
                {
                    UniversalWayTypeConfigEntity entity = new UniversalWayTypeConfigEntity();
                    entity.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : -1;
                    entity.PointID = reader["PointID"] != DBNull.Value ? Convert.ToInt32(reader["PointID"]) : -1;
                    entity.Sequence = reader["Sequence"] != DBNull.Value ? Convert.ToInt32(reader["Sequence"]) : -1;
                    entity.TerminalID = TerminalID;
                    entity.SyncState = reader["SyncState"] != DBNull.Value ? Convert.ToInt32(reader["SyncState"]) : -1;
                    entity.ModifyTime = reader["ModifyTime"] != DBNull.Value ? Convert.ToDateTime(reader["ModifyTime"]) : ConstValue.MinDateTime;

                    lstWayTypeConfig.Add(entity);
                }
                return lstWayTypeConfig;
            }
            return null;
        }

        public DataTable GetUniversalDataConfig()
        {
            string SQL = @"SELECT Type.ID,Config.TerminalID,Config.Sequence,Type.[Level],Type.[ParentID],Type.[WayType],Type.[Name],Type.[MaxMeasureRange],Type.[MaxMeasureRangeFlag],Type.[FrameWidth],Type.[Precision],Type.[Unit]
                        FROM [UniversalTerWayConfig] Config,[UniversalTerWayType] Type WHERE Config.PointID=Type.ID";

            DataTable dt = SQLHelper.ExecuteDataTable(SQL, null);
            return dt;
        }

    }
}