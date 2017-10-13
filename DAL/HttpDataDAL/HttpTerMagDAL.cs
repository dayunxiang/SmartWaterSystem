﻿using Common;
using Entity;
using SmartWaterSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace DAL
{
    public class HttpTerMagDAL
    {
        public List<TerMagInfoEntity> GetAllTer(string netpathhead)
        {
            List<TerMagInfoEntity> lstTerInfo = null;
            string SQL = "select info.*,pic.PicName,tertype.TerTypeName from TerManagerInfo info left join TerMagPic pic on info.Id = pic.TerMagId  and pic.Usefor = 1" +
                                                                           " left join TerTypeInfo tertype on info.TerType=tertype.TerTypeId";
            using (SqlDataReader reader = SQLHelper.ExecuteReader(SQL, null))
            {
                lstTerInfo = new List<TerMagInfoEntity>();
                while(reader.Read())
                {
                    bool isExist = false;
                    int index = 0;
                    int id = Convert.ToInt32(reader["Id"]); 
                    for (index = 0; index < lstTerInfo.Count; index++)
                    {
                        if (lstTerInfo[index].Id == id)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    /* 如果不存在就新增TerMagInfoEntity，如果存在就添加图片 */
                    if (!isExist)
                    {
                        TerMagInfoEntity terinfo = new TerMagInfoEntity();
                        terinfo.Id = id;
                        terinfo.DevId = Convert.ToInt32(reader["TerId"]);
                        terinfo.DevName = reader["TerName"].ToString().Trim();
                        terinfo.DevType = (ConstValue.DEV_TYPE)Convert.ToInt32(reader["TerType"]);
                        terinfo.DevTypeName = reader["TerTypeName"] != DBNull.Value ? reader["TerTypeName"].ToString().Trim() : "";
                        terinfo.Addr = reader["Addr"] != DBNull.Value ? reader["Addr"].ToString().Trim() : "";
                        terinfo.Remark = reader["Remark"] != DBNull.Value ? reader["Remark"].ToString().Trim() : "";
                        terinfo.Lng = Convert.ToDouble(reader["longitude"]);
                        terinfo.Lat = Convert.ToDouble(reader["latitude"]);
                        terinfo.PicId = new List<string>();
                        terinfo.PicId.Add(GetNetaddrByName(netpathhead,reader["PicName"].ToString().Trim()));
                        lstTerInfo.Add(terinfo);
                    }
                    else
                    {
                        lstTerInfo[index].PicId.Add(GetNetaddrByName(netpathhead, reader["PicName"].ToString().Trim()));
                    }
                }
                
            }
            return lstTerInfo;
        }

        /// <summary>
        /// 通过文件名，获取网络地址
        /// </summary>
        /// <param name="netpathhead">网络地址头</param>
        /// <param name="picname">文件名</param>
        /// <returns></returns>
        string GetNetaddrByName(string netpathhead,string picname)
        {
            if (string.IsNullOrEmpty(picname))
                return "";
            else
                return Path.Combine(netpathhead, picname);
        }

        public void DelTer(int Id)
        {
            //需要删除图片
            string SQL = "DELETE FROM [TerManagerInfo] WHERE Id='"+Id+"'";
            SQLHelper.ExecuteNonQuery(SQL, null);


        }

        /// <summary>
        /// 查询终端信息
        /// </summary>
        public TerMagInfoEntity QueryTerMagInfo(ConstValue.DEV_TYPE devtype,int DevId,string netpathhead ="")
        {
            string SQL = string.Format("select info.*,pic.PicName from TerManagerInfo info left join TerMagPic pic on info.Id = pic.TerMagId and Usefor = 1 where info.TerType='{0}' AND info.TerId='{1}'", (int)devtype, DevId);
            TerMagInfoEntity terinfo = null;
            using (SqlDataReader reader = SQLHelper.ExecuteReader(SQL, null))
            {
                while (reader.Read())
                {
                    if(terinfo == null)
                    {
                        /* 如果不存在就新增TerMagInfoEntity，如果存在就添加图片 */
                        terinfo = new TerMagInfoEntity();
                        terinfo.Id = Convert.ToInt32(reader["id"]);
                        terinfo.DevId = Convert.ToInt32(reader["TerId"]);
                        terinfo.DevType = (ConstValue.DEV_TYPE)Convert.ToInt32(reader["TerType"]);
                        terinfo.Addr = reader["Addr"] != DBNull.Value ? reader["Addr"].ToString().Trim() : "";
                        terinfo.Remark = reader["Remark"] != DBNull.Value ? reader["Remark"].ToString().Trim() : "";
                        terinfo.Lng = Convert.ToDouble(reader["longitude"]);
                        terinfo.Lat = Convert.ToDouble(reader["latitude"]);
                        terinfo.PicId = new List<string>();
                        terinfo.PicId.Add(GetNetaddrByName(netpathhead, reader["PicName"].ToString().Trim()));
                    }
                    else
                    {
                        terinfo.PicId.Add(GetNetaddrByName(netpathhead, reader["PicName"].ToString().Trim()));
                    }
                }
            }
            return terinfo;
        }


        public  bool InsertTerMagInfo(TerMagInfoEntity terinfo,string PicLocalTmpDir, string PicLocalDir)
        {
            SqlTransaction trans = null;
            try
            {
                SqlParameter[] parmsinfo = new SqlParameter[]
                {
                    new SqlParameter("@terid",SqlDbType.Int),
                    new SqlParameter("@tertype",SqlDbType.Int),
                    new SqlParameter("@tername",SqlDbType.NVarChar),
                    new SqlParameter("@addr",SqlDbType.NVarChar,200),
                    new SqlParameter("@remark",SqlDbType.NVarChar,300),

                    new SqlParameter("@lng",SqlDbType.Float),
                    new SqlParameter("@lat",SqlDbType.Float),
                    new SqlParameter("@modifytime",SqlDbType.DateTime),
                    new SqlParameter("@identify",SqlDbType.Int)
                };
                parmsinfo[7].Direction = ParameterDirection.Output;
                parmsinfo[0].Value = terinfo.DevId;
                parmsinfo[1].Value = terinfo.DevType;
                parmsinfo[2].Value = terinfo.DevName;
                parmsinfo[3].Value = terinfo.Addr;
                parmsinfo[4].Value = terinfo.Remark;
                parmsinfo[5].Value = terinfo.Lng;
                parmsinfo[6].Value = terinfo.Lat;
                parmsinfo[7].Value = DateTime.Now;

                SqlConnection conn = SQLHelper.Conn;
                SQLHelper.OpenConnection();
                trans = conn.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.Transaction = trans;
                command.CommandText = "INSERT TerManagerInfo(TerId,TerType,TerName,Addr,Remark,longitude,latitude,ModifyTime) VALUES(@terid,@tertype,@tername,@addr,@remark,@lng,@lat,@modifytime);select @identify=SCOPE_IDENTITY()";
                command.Parameters.Clear();
                command.Parameters.AddRange(parmsinfo);
                command.ExecuteNonQuery();

                if (terinfo.PicId != null && terinfo.PicId.Count > 0)
                {
                    long id = Convert.ToInt64(parmsinfo[7].Value);

                    SqlParameter[] parmspic = new SqlParameter[]
                    {
                    new SqlParameter("@magid",SqlDbType.Int),
                    new SqlParameter("@picname",SqlDbType.NVarChar,40),
                    new SqlParameter("modifytime",SqlDbType.DateTime)
                    };
                    parmspic[0].Value = id;
                    command.CommandText = "INSERT TerMagPic(TerMagId,PicName,Usefor,ModifyTime) VALUES(@magid,@picname,1,@modifytime)";
                    command.Parameters.Clear();
                    command.Parameters.AddRange(parmspic);

                    foreach (string picname in terinfo.PicId)
                    {
                        parmspic[1].Value = picname;
                        parmspic[2].Value = DateTime.Now;

                        command.ExecuteNonQuery();
                    }

                    PicHelper pichelper = new PicHelper();
                    foreach (string picname in terinfo.PicId)
                    {
                        pichelper.MoveFile(Path.Combine(PicLocalTmpDir, picname), Path.Combine(PicLocalDir, picname));
                    }
                }

                trans.Commit();
                return true;
            }
            catch(Exception ex)
            {
                if(trans!=null)
                {
                    trans.Rollback();
                }
                throw ex;
            }
        }
        
        public bool UpdateTerMagInfo(TerMagInfoEntity terinfo)
        {
            string SQL = "UPDATE TerManagerInfo SET TerId = {0},TerType={1},Addr='{2}',Remark='{3}',Longitude={4},Latitude={5}";
            return false;
        }

        /// <summary>
        /// 根据表ID确定是否存在
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public bool IsExistID(Int64 tableId)
        {
            string SQL = "SELECT COUNT(1) FROM TerManagerInfo WHERE id=" + tableId;
            object objcount=SQLHelper.ExecuteScalar(SQL, null);
            if(objcount!=DBNull.Value && objcount !=null)
            {
                return Convert.ToInt32(objcount) > 0 ? true : false;
            }
            return false;
        }
        
        public void UploadRepairRec(RepairInfoEntity repairRec, string PicLocalTmpDir, string PicLocalDir)
        {
            SqlTransaction trans = null;
            try
            {
                if (repairRec == null)
                    return;
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@id",SqlDbType.Int),
                    new SqlParameter("@terid",SqlDbType.Int),
                    new SqlParameter("@breakid",SqlDbType.Int),
                    new SqlParameter("@desc",SqlDbType.NVarChar,300),
                    new SqlParameter("@userid",SqlDbType.Int),
                    new SqlParameter("modifytime",SqlDbType.DateTime),
                    new SqlParameter("@identify",SqlDbType.Int)
                };
                parms[0].Value = repairRec.Id;
                parms[1].Value = repairRec.DevId;
                parms[2].Value = repairRec.BreakdownId;
                parms[3].Value = repairRec.Desc;
                parms[4].Value = repairRec.UserId;
                parms[5].Value = repairRec.RepairTime;
                parms[6].Direction = ParameterDirection.Output;

                SqlConnection conn = SQLHelper.Conn;
                SQLHelper.OpenConnection();
                trans = conn.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.Transaction = trans;
                command.CommandText = "INSERT INTO TerRepairRec(TerMagId,TerId,BreakdownId,[Desc],UserId,ModifyTime) VALUES(@id,@terid,@breakid,@desc,@userid,@modifytime);select @identify=SCOPE_IDENTITY()";
                command.Parameters.Clear();
                command.Parameters.AddRange(parms);
                command.ExecuteNonQuery();
                long id = Convert.ToInt64(parms[6].Value);

                SqlParameter[] parmspic = new SqlParameter[]
                {
                    new SqlParameter("@magid",SqlDbType.Int),
                    new SqlParameter("@picname",SqlDbType.NVarChar,40),
                    new SqlParameter("modifytime",SqlDbType.DateTime)
                };
                parmspic[0].Value = id;
                command.CommandText = "INSERT TerMagPic(TerMagId,PicName,Usefor,ModifyTime) VALUES(@magid,@picname,2,@modifytime)";
                command.Parameters.Clear();
                command.Parameters.AddRange(parmspic);

                foreach (string picname in repairRec.PicsPath)
                {
                    parmspic[1].Value = picname;
                    parmspic[2].Value = DateTime.Now;

                    command.ExecuteNonQuery();
                }

                PicHelper pichelper = new PicHelper();
                foreach (string picname in repairRec.PicsPath)
                {
                    pichelper.MoveFile(Path.Combine(PicLocalTmpDir, picname), Path.Combine(PicLocalDir, picname));
                }

                trans.Commit();
            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                throw ex;
            }
        }

        public List<RepairInfoEntity> QueryRepairRec(long TerMagId, string netpathhead)
        {
            List<RepairInfoEntity> lstRec = null;
            string SQL = "select rec.*,pic.PicName,euser.EN_User_Name as UserName from TerRepairRec rec " +
                "left join TerMagPic pic on rec.Id = pic.TerMagId and Usefor = 2 " +
                "left join EN_User euser on euser.EN_User_ID = rec.UserId " +                                                         
                "where rec.TerMagId='" + TerMagId+"'";
            using (SqlDataReader reader = SQLHelper.ExecuteReader(SQL, null))
            {
                lstRec = new List<RepairInfoEntity>();
                while (reader.Read())
                {
                    bool isExist = false;
                    int index = 0;
                    int id = Convert.ToInt32(reader["Id"]);
                    for (index = 0; index < lstRec.Count; index++)
                    {
                        if (lstRec[index].Id == id)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    /* 如果不存在就新增TerMagInfoEntity，如果存在就添加图片 */
                    if (!isExist)
                    {
                        RepairInfoEntity terinfo = new RepairInfoEntity();
                        terinfo.Id = Convert.ToInt32(reader["TerMagId"]);
                        terinfo.DevId = Convert.ToInt32(reader["TerId"]);
                        terinfo.BreakdownId = Convert.ToInt32(reader["BreakdownId"]);
                        terinfo.Desc = reader["Desc"] != DBNull.Value ? reader["Desc"].ToString() : "";
                        terinfo.RepairTime = reader["ModifyTime"].ToString();
                        terinfo.UserId = Convert.ToInt64(reader["UserId"]);
                        terinfo.UserName = reader["UserName"].ToString();
                        
                        terinfo.PicsPath = new List<string>();
                        terinfo.PicsPath.Add(GetNetaddrByName(netpathhead, reader["PicName"].ToString().Trim()));
                        lstRec.Add(terinfo);
                    }
                    else
                    {
                        lstRec[index].PicsPath.Add(GetNetaddrByName(netpathhead, reader["PicName"].ToString().Trim()));
                    }
                }

            }
            return lstRec;
        }

        public List<TerTypeInfoEntity> GetAllTerTypeInfo()
        {
            string SQL = "select id,TerTypeId,TerTypeName,ModifyTime from TerTypeInfo";
            List<TerTypeInfoEntity> lstType = null;
            using (SqlDataReader reader = SQLHelper.ExecuteReader(SQL, null))
            {
                lstType = new List<TerTypeInfoEntity>();
                while (reader.Read())
                {
                    TerTypeInfoEntity tertype = new TerTypeInfoEntity();
                    tertype.Id = Convert.ToInt32(reader["id"]);
                    tertype.TerTypeId = reader["TerTypeId"] != DBNull.Value ? Convert.ToInt32(reader["TerTypeId"]) : 0;
                    tertype.TerTypeName = reader["TerTypeName"] != DBNull.Value ? reader["TerTypeName"].ToString() : "";
                    tertype.ModifyTime = reader["ModifyTime"] != DBNull.Value ? Convert.ToDateTime(reader["ModifyTime"]).ToString(ConstValue.DateTimeFormat) : "";
                    lstType.Add(tertype);
                }
            }
            return lstType;
        }
        
        public List<TerBreakdownInfoEntity> GetAllBreakdownInfo()
        {
            string SQL = "select id,BreakdownId,BreakdownName,ModifyTime from TerBreakdownInfo";
            List<TerBreakdownInfoEntity> lstType = null;
            using (SqlDataReader reader = SQLHelper.ExecuteReader(SQL, null))
            {
                lstType = new List<TerBreakdownInfoEntity>();
                while (reader.Read())
                {
                    TerBreakdownInfoEntity tertype = new TerBreakdownInfoEntity();
                    tertype.Id = Convert.ToInt32(reader["id"]);
                    tertype.BreakdownId = reader["BreakdownId"].ToString();
                    tertype.BreakdownName = reader["BreakdownName"] != DBNull.Value ? reader["BreakdownName"].ToString() : "";
                    tertype.ModifyTime = reader["ModifyTime"] != DBNull.Value ? Convert.ToDateTime(reader["ModifyTime"]).ToString(ConstValue.DateTimeFormat) : "";
                    lstType.Add(tertype);
                }
            }
            return lstType;
        }

        public DateTime GetMaxTerTypeModifytime()
        {
            string SQL = "select MAX(ModifyTime) from TerTypeInfo";

            object obj_max=SQLHelper.ExecuteScalar(SQL, null);
            if (obj_max != null)
            {
                return Convert.ToDateTime(obj_max);
            }
            else
                return ConstValue.MinDateTime;
        }

        public DateTime GetMaxBreakdownInfoModifytime()
        {
            string SQL = "select MAX(ModifyTime) from TerBreakdownInfo";

            object obj_max = SQLHelper.ExecuteScalar(SQL, null);
            if (obj_max != null)
            {
                return Convert.ToDateTime(obj_max);
            }
            else
                return ConstValue.MinDateTime;
        }


    }
}
