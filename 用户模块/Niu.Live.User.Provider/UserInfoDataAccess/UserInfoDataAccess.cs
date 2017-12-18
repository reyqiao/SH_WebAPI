using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Niu.Live.User.Provider.UserInfoDataAccess
{
    using Dapper;
    using Niu.Live.User.IModel.UserInfo;

    public class UserInfoDataAccess
    {
        #region 属性

        static string DB_UserConnection = ConfigurationManager.ConnectionStrings["DB_LiveUserConnection"].ConnectionString;

        #endregion

        #region 根据ID获取用户信息(手机用户)

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserInfo GetByUserID(long userId)
        {
            using (IDbConnection conn = new SqlConnection(DB_UserConnection))
            {
                string sql = " select top 1 userId,channelId,nickName,mobile,type,logoPhotoUrl,status,addTime from [dbo].[user] with(nolock) where userid=@userid ";
                return conn.Query<UserInfo>(sql, new { userid = userId }).FirstOrDefault();
            }
        }

        #endregion

        #region 根据ID获取用户信息(匿名用户)

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserInfo GetAnonymousByUserID(long userId)
        {
            UserInfo userAnonymous = null;
            try
            {
                using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                    string sql = " select top 1 userid,channelid,nickname,[type] ,[status],addtime, '' mobile  from [dbo].[useranonymous] with(nolock) where userid=@userid ";
                    return conn.Query<UserInfo>(sql, new { userid = userId }).FirstOrDefault();
                }
            }
            catch { }

            return userAnonymous;
        }

        #endregion
    }
}
