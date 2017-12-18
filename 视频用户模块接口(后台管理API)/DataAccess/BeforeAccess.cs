using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Net;
using Niugu.Common;
using NiuGu.Utility;


namespace DataAccess
{
    public class BeforeAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DB_CFD"].ConnectionString;
        public static string DB_Live = ConfigurationManager.ConnectionStrings["DB_Live"].ConnectionString;
        #region 直播间基础数据
        //获取直播间基础数据
        public static bool GetLive_BaseData(out DataSet ds, out string errorMessage)
        {
           
            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_BaseData]", ConnectionString, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    errorMessage = "暂无数据";
                    return false;
                }
            }
            return false;
        }

        public static bool Live_login(string userName, out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@userName", userName);
            thisParams[0].Direction = ParameterDirection.Input;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[Live_login]", ConnectionString, ref thisParams, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    errorMessage = "暂无数据";
                    return false;
                }
            }
            return false;
        }
        //游客模式
        public static bool UpdateLive_LiveBaseIsVisitor(int IsVisitor, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@IsVisitor", IsVisitor);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("UpdateLive_LiveBaseIsVisitor", ConnectionString, ref thisParams, out errorMessage);
        }


        //跑马灯管理
        public static bool GetLive_MarqueeList(out DataSet ds, out string errorMessage)
        {
            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_MarqueeList]", ConnectionString, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    errorMessage = "暂无数据";
                    return false;
                }
            }
            return false;
        }
        //跑马灯删除    
        public static bool UpdateLive_Marquee(int id, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@id", id);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("UpdateLive_Marquee", ConnectionString, ref thisParams, out errorMessage);
        }
        //跑马灯添加    
        public static bool AddLive_Marquee(string MarqueeText, string MarqueeLink, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[2];

            thisParams[0] = new SqlParameter("@MarqueeText", MarqueeText);
            thisParams[0].Direction = ParameterDirection.Input;
            thisParams[1] = new SqlParameter("@MarqueeLink", MarqueeLink);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("AddLive_Marquee", ConnectionString, ref thisParams, out errorMessage);
        }

        //直播间公告
        public static bool UpdateLive_LiveBaseNotice(string Notice, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@Notice", Notice);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("UpdateLive_LiveBaseNotice", ConnectionString, ref thisParams, out errorMessage);
        }
        //顶部banner管理
        public static bool UpdateLive_LiveBaseTopBanner(string TopBanner, string BannerLink, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[2];

            thisParams[0] = new SqlParameter("@TopBanner", TopBanner);
            thisParams[0].Direction = ParameterDirection.Input;
            thisParams[1] = new SqlParameter("@BannerLink", BannerLink);
            thisParams[1].Direction = ParameterDirection.Input;


            return SQLCommon.ExecuteStoredProcedure("UpdateLive_LiveBaseTopBanner", ConnectionString, ref thisParams, out errorMessage);
        }
        #endregion

        #region 老师介绍管理
        //获取老师列表
        public static bool GetLive_TeacherList(out DataSet ds, out string errorMessage)
        {
            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_TeacherList]", ConnectionString, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    errorMessage = "暂无数据";
                    return false;
                }
            }
            return false;
        }
        //获取老师信息根据id
        public static bool GetLive_Teacher(int id, out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@id", id);
            thisParams[0].Direction = ParameterDirection.Input;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_Teacher]", ConnectionString, ref thisParams, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    errorMessage = "暂无数据";
                    return false;
                }
            }
            return false;
        }


        //添加老师信息
        public static bool AddLive_Teacher(string teachername, string TeacherFace, string TeacherTag, string WinRate, string Income, string Introduce, string TeacherInfo, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[7];

            thisParams[0] = new SqlParameter("@teachername", teachername);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@TeacherFace", TeacherFace);
            thisParams[1].Direction = ParameterDirection.Input;

            thisParams[2] = new SqlParameter("@TeacherTag", TeacherTag);
            thisParams[2].Direction = ParameterDirection.Input;

            thisParams[3] = new SqlParameter("@WinRate", WinRate);
            thisParams[3].Direction = ParameterDirection.Input;

            thisParams[4] = new SqlParameter("@Income", Income);
            thisParams[4].Direction = ParameterDirection.Input;

            thisParams[5] = new SqlParameter("@Introduce", Introduce);
            thisParams[5].Direction = ParameterDirection.Input;

            thisParams[6] = new SqlParameter("@TeacherInfo", TeacherInfo);
            thisParams[6].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("AddLive_Teacher", ConnectionString, ref thisParams, out errorMessage);

        }
        //修改老师 
        public static bool UpdateLive_Teacher(int id, string teachername, string TeacherFace, string TeacherTag, string WinRate, string Income, string Introduce, string teacherinfo, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[8];

            thisParams[0] = new SqlParameter("@teachername", teachername);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@TeacherFace", TeacherFace);
            thisParams[1].Direction = ParameterDirection.Input;

            thisParams[2] = new SqlParameter("@TeacherTag", TeacherTag);
            thisParams[2].Direction = ParameterDirection.Input;

            thisParams[3] = new SqlParameter("@WinRate", WinRate);
            thisParams[3].Direction = ParameterDirection.Input;

            thisParams[4] = new SqlParameter("@Income", Income);
            thisParams[4].Direction = ParameterDirection.Input;

            thisParams[5] = new SqlParameter("@Introduce", Introduce);
            thisParams[5].Direction = ParameterDirection.Input;

            thisParams[6] = new SqlParameter("@id", id);
            thisParams[6].Direction = ParameterDirection.Input;

            thisParams[7] = new SqlParameter("@teacherinfo", teacherinfo);
            thisParams[7].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("UpdateLive_Teacher", ConnectionString, ref thisParams, out errorMessage);
        }
        //删除老师
        public static bool DeleteLive_Teacher(int id, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@id", id);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("DeleteLive_Teacher", ConnectionString, ref thisParams, out errorMessage);
        }

        #endregion

        #region 用户管理
        //获取用户列表
        public static bool GetLive_User(int type, int pagesize, int pageindex, string phone, out int totalcount, out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            totalcount = 0;
            ds = null;
            SqlParameter[] thisParams = new SqlParameter[5];
            thisParams[0] = new SqlParameter("@Type", type);
            thisParams[0].Direction = ParameterDirection.Input;
            thisParams[1] = new SqlParameter("@Pagesize", pagesize);
            thisParams[1].Direction = ParameterDirection.Input;
            thisParams[2] = new SqlParameter("@Pageindex", pageindex);
            thisParams[2].Direction = ParameterDirection.Input;
            thisParams[3] = new SqlParameter("@phone", phone);
            thisParams[4] = new SqlParameter("@Totalcount", SqlDbType.Int, 32);
            thisParams[4].Direction = ParameterDirection.Output;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_User]", ConnectionString, ref thisParams, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (DBNull.Value != thisParams[4].Value)
                    {
                        totalcount = Convert.ToInt32(thisParams[4].Value);
                        return true;
                    }
                }
            }
            return false;
        }

        //获取禁言列表
        public static bool GetNeteaseImChatroomMuteUserLog(int type, int pagesize, int pageindex, out int totalcount, out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            totalcount = 0;
            ds = null;
            SqlParameter[] thisParams = new SqlParameter[4];
            thisParams[0] = new SqlParameter("@Type", type);
            thisParams[0].Direction = ParameterDirection.Input;
            thisParams[1] = new SqlParameter("@Pagesize", pagesize);
            thisParams[1].Direction = ParameterDirection.Input;
            thisParams[2] = new SqlParameter("@Pageindex", pageindex);
            thisParams[2].Direction = ParameterDirection.Input;
            thisParams[3] = new SqlParameter("@Totalcount", SqlDbType.Int, 32);
            thisParams[3].Direction = ParameterDirection.Output;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetNeteaseImChatroomMuteUserLog]", DB_Live, ref thisParams, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (DBNull.Value != thisParams[3].Value)
                    {
                        totalcount = Convert.ToInt32(thisParams[3].Value);
                        return true;
                    }
                }
            }
            return false;
        }
        //获取黑名单列表
        public static bool GetNeteaseImUserBlock(int type, int pagesize, int pageindex, out int totalcount, out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            totalcount = 0;
            ds = null;
            SqlParameter[] thisParams = new SqlParameter[4];
            thisParams[0] = new SqlParameter("@Type", type);
            thisParams[0].Direction = ParameterDirection.Input;
            thisParams[1] = new SqlParameter("@Pagesize", pagesize);
            thisParams[1].Direction = ParameterDirection.Input;
            thisParams[2] = new SqlParameter("@Pageindex", pageindex);
            thisParams[2].Direction = ParameterDirection.Input;
            thisParams[3] = new SqlParameter("@Totalcount", SqlDbType.Int, 32);
            thisParams[3].Direction = ParameterDirection.Output;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetNeteaseImUserBlock]", DB_Live, ref thisParams, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (DBNull.Value != thisParams[3].Value)
                    {
                        totalcount = Convert.ToInt32(thisParams[3].Value);
                        return true;
                    }
                }
            }
            return false;
        }

        //接触禁言\解除黑名单\转为永久会员
        public static bool DeleteLive_User(int id, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@id", id);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("DeleteLive_User", ConnectionString, ref thisParams, out errorMessage);
        }

        //接触禁言\解除黑名单\转为永久会员
        public static bool DeleteNeteaseImChatroomMuteUserLog(int id, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@id", id);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("DeleteNeteaseImChatroomMuteUserLog", DB_Live, ref thisParams, out errorMessage);
        }

        public static bool DeleteNeteaseImUserBlock(int id, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@id", id);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("DeleteNeteaseImUserBlock", DB_Live, ref thisParams, out errorMessage);
        }
        /// <summary>
        /// 设置用户直播间的角色
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool SetUserRole(int roomId, long userId, int roleId, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[3];

            thisParams[0] = new SqlParameter("@roomId", roomId);
            thisParams[1] = new SqlParameter("@userId", userId);
            thisParams[2] = new SqlParameter("@roleId", roleId);
            return SQLCommon.ExecuteStoredProcedure("Live_User_Role", ConnectionString, ref thisParams, out errorMessage);
        }

        #endregion

        #region 视频管理
        //获取视频页面信息
        public static bool GetLive_VideoList(int VideoId, out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            ds = null;
            SqlParameter[] thisParams = new SqlParameter[1];
            thisParams[0] = new SqlParameter("@VideoId", VideoId);
            thisParams[0].Direction = ParameterDirection.Input;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_VideoList]", ConnectionString, ref thisParams, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        //获取视频栏目
        public static bool GetLive_VideoType(int Id, out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            ds = null;
            SqlParameter[] thisParams = new SqlParameter[1];
            thisParams[0] = new SqlParameter("@Id", Id);
            thisParams[0].Direction = ParameterDirection.Input;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_VideoType]", ConnectionString, ref thisParams, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        //添加视频栏目
        public static bool Addive_VideoType(string VideoName, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@VideoName", VideoName);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("Addive_VideoType", ConnectionString, ref thisParams, out errorMessage);
        }

        //修改视频栏目
        public static bool UpdateLive_VideoType(int Id, string VideoName, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[2];

            thisParams[0] = new SqlParameter("@Id", Id);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@VideoName", VideoName);
            thisParams[1].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("UpdateLive_VideoType", ConnectionString, ref thisParams, out errorMessage);
        }

        //删除视频栏目
        public static bool DeleteLive_VideoType(int Id, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@Id", Id);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("DeleteLive_VideoType", ConnectionString, ref thisParams, out errorMessage);
        }

        //添加视频
        public static bool Addive_Video(string VideoId, string VideoTheme, string VideoPath, string Cover, string Introduce, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[5];

            thisParams[0] = new SqlParameter("@VideoId", VideoId);
            thisParams[0].Direction = ParameterDirection.Input;
            thisParams[1] = new SqlParameter("@VideoTheme", VideoTheme);
            thisParams[1].Direction = ParameterDirection.Input;
            thisParams[2] = new SqlParameter("@VideoPath", VideoPath);
            thisParams[2].Direction = ParameterDirection.Input;
            thisParams[3] = new SqlParameter("@Cover", Cover);
            thisParams[3].Direction = ParameterDirection.Input;
            thisParams[4] = new SqlParameter("@Introduce", Introduce);
            thisParams[4].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("Addive_Video", ConnectionString, ref thisParams, out errorMessage);
        }


        //删除视频
        public static bool DeleteLive_Video(int Id, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@Id", Id);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("DeleteLive_Video", ConnectionString, ref thisParams, out errorMessage);
        }

        #endregion

        #region 直播预告管理
        //获取直播预告列表
        public static bool GetLive_Notice(out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;


            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_Notice]", ConnectionString, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    return true;

                }
            }
            return false;
        }

        public static bool GetLive_NoticePic(out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;


            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_NoticePic]", ConnectionString, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    return true;

                }
            }
            return false;
        }

        //添加直播预告
        public static bool AddLive_Notice(string Day, string BeginTime, string EndTime, string LiveTheme, string Teacher, string LiveType, string LiveIntroduce, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[7];

            thisParams[0] = new SqlParameter("@Day", Day);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@BeginTime", BeginTime);
            thisParams[1].Direction = ParameterDirection.Input;

            thisParams[2] = new SqlParameter("@EndTime", EndTime);
            thisParams[2].Direction = ParameterDirection.Input;

            thisParams[3] = new SqlParameter("@LiveTheme", LiveTheme);
            thisParams[3].Direction = ParameterDirection.Input;

            thisParams[4] = new SqlParameter("@Teacher", Teacher);
            thisParams[4].Direction = ParameterDirection.Input;

            thisParams[5] = new SqlParameter("@LiveType", LiveType);
            thisParams[5].Direction = ParameterDirection.Input;

            thisParams[6] = new SqlParameter("@LiveIntroduce", LiveIntroduce);
            thisParams[6].Direction = ParameterDirection.Input;


            return SQLCommon.ExecuteStoredProcedure("AddLive_Notice", ConnectionString, ref thisParams, out errorMessage);
        }

        public static bool AddLive_NoticePic(string noticepic, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@NoticePic", noticepic);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("AddLive_NoticePic", ConnectionString, ref thisParams, out errorMessage);
        }

        //删除直播预告
        public static bool DeleteLive_Notice(int id, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@id", id);
            thisParams[0].Direction = ParameterDirection.Input;

            return SQLCommon.ExecuteStoredProcedure("DeleteLive_Notice", ConnectionString, ref thisParams, out errorMessage);
        }
        #endregion

        #region 前台界面接口
        //获取直播详情
        public static bool GetLive_NoticeById(int id, out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            ds = null;
            SqlParameter[] thisParams = new SqlParameter[1];
            thisParams[0] = new SqlParameter("@id", id);

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_NoticeById]", ConnectionString, ref thisParams, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    return true;
                }
            }
            return false;
        }
        //获取精品视频列表
        public static bool GetLive_Video(int type, int pagesize, int pageindex, out int totalcount, out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            totalcount = 0;
            ds = null;
            SqlParameter[] thisParams = new SqlParameter[4];
            thisParams[0] = new SqlParameter("@Type", type);
            thisParams[0].Direction = ParameterDirection.Input;
            thisParams[1] = new SqlParameter("@Pagesize", pagesize);
            thisParams[1].Direction = ParameterDirection.Input;
            thisParams[2] = new SqlParameter("@Pageindex", pageindex);
            thisParams[2].Direction = ParameterDirection.Input;
            thisParams[3] = new SqlParameter("@Totalcount", SqlDbType.Int, 32);
            thisParams[3].Direction = ParameterDirection.Output;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_Video]", ConnectionString, ref thisParams, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (DBNull.Value != thisParams[3].Value)
                    {
                        totalcount = Convert.ToInt32(thisParams[3].Value);
                        return true;
                    }
                }
            }
            return false;
        }
        //获取直播日
        public static bool GetLive_NoticeDay(out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            ds = null;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_NoticeDay]", ConnectionString, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        //根据直播日获取通知
        public static bool GetLive_NoticeByDay(string day, out DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;
            ds = null;
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[1];

            thisParams[0] = new SqlParameter("@Day", day);
            thisParams[0].Direction = ParameterDirection.Input;
            if (SQLCommon.ExecuteStoredProcedure("[dbo].[GetLive_NoticeByDay]", ConnectionString, ref thisParams, out ds, out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

    }

    public class LiveUserAccess
    {
        public static List<object> GetRole(Int64 userid)
        {
            try
            {
                string sql = @"SELECT [Role].RoleName,[Role].RoleID
                        FROM [User_Role_Live]
                        left join [Role]
                        on [Role].RoleID=[User_Role_Live].RoleID
                        where [User_Role_Live].UserID=@UserID";

                SqlParameter[] parms = { new SqlParameter("@UserID", SqlDbType.BigInt, 8) };
                parms[0].Value = userid;

                DataTable dt = SqlHelper.ExecuteDataSet(sql, parms).Tables[0];

                List<object> rolelist = new List<object>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        rolelist.Add(new { name = dr["RoleName"].ToString(), id = dr["RoleID"] });
                    }

                }
                return rolelist;
            }
            catch
            {
                return null;
            }
        }


        public static DataTable GetRoleList()
        {
            try
            {
                string sql = @"select [RoleID],[RoleName],[IsManage] from [Role] order by RoleID ";
                return SqlHelper.ExecuteDataSet(sql, null).Tables[0];
            }
            catch
            {
                return null;
            }
        }

        public static int SetDisable(Int64 userid, int status)
        {
            try
            {
                string sql = @"update [User] set [Status]=@status,LastVisitTime=GetDate() where userid=@userid";

                SqlParameter[] parms = { new SqlParameter("@userid", SqlDbType.BigInt, 8), new SqlParameter("@status", SqlDbType.SmallInt) };
                parms[0].Value = userid;
                parms[1].Value = status;

                return SqlHelper.ExecuteNonQuery(sql, parms);
            }
            catch
            {
                return -1;
            }
        }

        public static int Delete(Int64 userid)
        {
            try
            {
                string sql = @"
                    delete [NeteaseImUser] where userid=@userid;
                    delete [User_Role_Live] where userid=@userid;                 
                    delete [User] where userid=@userid";

                SqlParameter[] parms = { new SqlParameter("@userid", SqlDbType.BigInt) };
                parms[0].Value = userid;

                return SqlHelper.ExecuteNonQuery(sql, parms);
            }
            catch
            {
                return -1;
            }
        }

        public static int ResetPassword(Int64 userid, string defaultPassword = "11111a")
        {
            try
            {
                string sql = @"update [User] set [Password]=@Password,LastVisitTime=GetDate() where userid=@userid";

                SqlParameter[] parms = { new SqlParameter("@userid", SqlDbType.BigInt, 8), new SqlParameter("@Password", SqlDbType.VarChar, 100) };
                parms[0].Value = userid;
                parms[1].Value = passwordMD5(userid, defaultPassword);

                return SqlHelper.ExecuteNonQuery(sql, parms);
            }
            catch
            {
                return -1;
            }
        }


        public static int ResetRole(Int64 userid, string roles)
        {

            SqlConnection conn = (SqlConnection)SqlHelper.GetConnection();
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();
            try
            {
                string sql = @"delete from User_Role_Live where UserID=@userid";
                SqlParameter[] parms = { new SqlParameter("@userid", SqlDbType.BigInt) };
                parms[0].Value = userid;

                SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, parms);
                string[] roleids = roles.Split(',');
                if (roleids != null && roleids.Length > 0)
                {
                    foreach (var item in roleids)
                    {
                        if (string.IsNullOrEmpty(item.Trim()))
                            continue;

                        Int64 RoleID = item.TryParseByLong();

                        sql = "insert into User_Role_Live (UserID,RoleID) values (@UserID,@RoleID);";
                        SqlParameter[] insertparms = { new SqlParameter("@UserID", SqlDbType.BigInt), new SqlParameter("@RoleID", SqlDbType.BigInt) };
                        insertparms[0].Value = userid;
                        insertparms[1].Value = RoleID;

                        SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, insertparms);
                    }
                }


                tran.Commit();

                return 1;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                //throw ex;
                return 0;
            }
            finally
            {
                if (tran != null)
                    tran.Dispose();
                if (conn != null)
                    conn.Close();
            }
        }


        static string passwordMD5(long userID, string password)
        {
            string sql = "select addTime from [User] where userid=@userid";
            SqlParameter[] parms = { new SqlParameter("@userid", SqlDbType.BigInt) };
            parms[0].Value = userID;

            DataTable dt = SqlHelper.ExecuteDataSet(sql, parms).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                string p = NiuGu.Security.HashManager.Hash("MD5", (string.Format("{0}'{1}'{2}", userID, dt.Rows[0]["addTime"].TryParseByDateTime().ToString("yyyyMMdd HH:mm:ss"), password)));
                return Niu.Cabinet.Cryptography.NiuCryptoService.EncryptPassword(p.ToUpper());
            }
            return null;
        }
    }
}