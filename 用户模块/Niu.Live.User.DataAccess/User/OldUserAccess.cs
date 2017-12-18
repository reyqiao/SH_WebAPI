using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Niu.Live.User.DataAccess.User
{
    using Dapper;
    using Niu.Live.User.Model;

    public class OldUserAccess
    {
        #region 属性

        static string DB_UserConnection = ConfigurationManager.ConnectionStrings["DB_LiveOldUserConnection"] != null ? ConfigurationManager.ConnectionStrings["DB_LiveOldUserConnection"].ConnectionString : "";

        #endregion

        #region 根据手机号获取用户信息

        /// <summary>
        /// 根据手机号获取用户信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static OldUserInfo getUserInfo(string mobile,string password)
        {
            using (IDbConnection conn = new SqlConnection(DB_UserConnection))
            {
                string sql = @"SELECT TOP 1 
                              [i_User_NickName] nickName
                              ,[i_User_Tel] mobile
                              ,[i_User_Sex] sex
                              ,[i_User_RoleID] roleId
                              FROM [dbo].[i_User_Info] with(nolock) where i_User_Tel =@i_User_Tel and i_User_Pass =@i_User_Pass  ";


                return conn.Query<OldUserInfo>(sql, new { i_User_Tel = mobile, i_User_Pass = password }).FirstOrDefault();
            }
        }

        #endregion
    }
}
