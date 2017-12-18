using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Niu.Live.User.DataAccess.RequestRecord
{
    using Dapper;

    public class UserActionLogAccess
    {
        #region 属性

        static string DB_UserConnection = ConfigurationManager.ConnectionStrings["DB_LiveUserConnection"] != null ? ConfigurationManager.ConnectionStrings["DB_LiveUserConnection"].ConnectionString : "";

        #endregion

        #region 操作成功添加

        /// <summary>
        /// 操作成功添加
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="packType"></param>
        /// <param name="userIP"></param>
        /// <param name="userAgent"></param>
        /// <param name="userID"></param>
        /// <param name="extend"></param>
        /// <returns></returns>
        public static bool AddUserActionSuccess(int interfaceType, int packType, string userIP, long userID, string extend = "")
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                    string sql = @" INSERT INTO [dbo].[Log_UserActionSuccess]([InterfaceType],[PackType],[UserIP],[UserID],[Extend])
                                 VALUES(@InterfaceType,@PackType,@UserIP,@UserID,@Extend)  ";
                    conn.Execute(sql, new { InterfaceType = interfaceType, PackType = packType,
                                    UserIP = userIP,UserID = userID, Extend = extend
                    });

                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        #endregion

        #region 操作失败添加

        /// <summary>
        /// 操作失败添加
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="packType"></param>
        /// <param name="codeType"></param>
        /// <param name="userIP"></param>
        /// <param name="userAgent"></param>
        /// <param name="userInfo"></param>
        /// <param name="userID"></param>
        /// <param name="extend"></param>
        /// <param name="message"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool AddUserActionFailed(int interfaceType, int packType, int codeType, string userIP,  string userInfo, long userID, string extend, string message, out int count)
        {
            count = 0;

            DynamicParameters p = new DynamicParameters();
            p.Add("@InterfaceType", interfaceType);
            p.Add("@PackType", packType);
            p.Add("@UserIP", userIP);
            p.Add("@UserID", userID);
            p.Add("@Extend", extend);
            p.Add("@Message", message);
            p.Add("@CodeType", codeType);
            p.Add("@FailedCount", 0, DbType.Int32, ParameterDirection.Output);

            try
            {
                using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                     conn.Execute("SP_Log_UserActionFailed_Add", p, null, null, CommandType.StoredProcedure);

                     count = p.Get<int>("@FailedCount");
                     return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}