using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.DataAccess.User
{
    using Dapper;
    using Niu.Live.User.Model;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// 匿名用户数据处理类
    /// </summary>
    public class UserAnonymousAccess
    {
        #region 属性

        static string DB_UserConnection =  ConfigurationManager.ConnectionStrings["DB_LiveUserConnection"] != null ? ConfigurationManager.ConnectionStrings["DB_LiveUserConnection"].ConnectionString : "";

        #endregion

        #region 根据ID获取用户信息

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserAnonymous GetByUserID(long userId)
        {
            UserAnonymous userAnonymous = null;
            try
            {
                using(IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                    string sql = " select top 1 userid,channelid,nickname,[type] ,[status],addtime from [dbo].[useranonymous] with(nolock) where userid=@userid ";
                    return conn.Query<UserAnonymous>(sql, new { userid = userId }).FirstOrDefault();
                }
            }
            catch{  }

            return userAnonymous;
        }

        #endregion

        #region 创建匿名用户

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool AddUser(int channelId,string nickName,int type,int status, out UserAnonymous userInfo)
        {
            userInfo = null;
            DynamicParameters p = new DynamicParameters();
            p.Add("@ChannelId", channelId);
            p.Add("@NickName", nickName);
            p.Add("@Type", type);
            p.Add("@Status", status);

            try
            {
                 using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                 {
                    userInfo = conn.Query<UserAnonymous>("UserAnonymous_Add", p, null
                        ,true,null,CommandType.StoredProcedure).FirstOrDefault();
                 }
                 return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
