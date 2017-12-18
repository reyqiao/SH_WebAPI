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

    /// <summary>
    /// 手机账号数据处理类
    /// </summary>
    public class UserAccess
    {
        #region 属性

        static string DB_UserConnection = ConfigurationManager.ConnectionStrings["DB_LiveUserConnection"] != null ? ConfigurationManager.ConnectionStrings["DB_LiveUserConnection"].ConnectionString : "";

        #endregion

        #region 判断是否存在手机号

        /// <summary>
        /// 判断是否存在手机号
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool mobileIsExist(int channelId, string mobile)
        {
            using (IDbConnection conn = new SqlConnection(DB_UserConnection))
            {
                bool isExist = false;
                string sql = " select top 1 1 from [dbo].[user] with(nolock) where mobile=@mobile and channelId=@channelId ";
                var userCount = conn.ExecuteScalar<int>(sql, new { mobile = mobile, channelId = channelId });

                if (userCount > 0)
                    isExist = true;

                return isExist;
            }
        }

        #endregion

        #region 根据ID获取用户信息

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserInfo GetByUserID(long userId)
        {
            using (IDbConnection conn = new SqlConnection(DB_UserConnection))
            {
                string sql = @" select top 1 u.*,ur.roleid,r.RoleName,r.ismanage  from [dbo].[user]  u with(nolock) 
                                left join user_role_live ur with(nolock) on u.userid = ur.userid
                                left join Role r with(nolock) on ur.roleid = r.roleid
                                where u.userid=@userid ";
                return conn.Query<UserInfo>(sql, new { userid = userId }).FirstOrDefault();
            }
        }

        #endregion

        #region 根据手机号获取用户信息

        /// <summary>
        /// 根据手机号获取用户信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static UserInfo GetByMobile(int channelId, string mobile)
        {
            using (IDbConnection conn = new SqlConnection(DB_UserConnection))
            {
                string sql = @" select top 1 u.*,ur.roleid,r.RoleName,r.ismanage 
                              from [dbo].[user] u with(nolock) 
                              left join user_role_live ur with(nolock) on u.userid = ur.userid
                              left join Role r with(nolock) on ur.roleid = r.roleid
                              where mobile=@mobile and channelId = @channelId  ";
                return conn.Query<UserInfo>(sql, new { mobile = mobile, channelId = channelId }).FirstOrDefault();
            }
        }

        #endregion

        #region 是否存在该昵称

        /// <summary>
        /// 是否存在该昵称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool IsExisNickName(string nickName)
        {
            bool isExist = false;
            using (IDbConnection conn = new SqlConnection(DB_UserConnection))
            {
                string sql = " select top 1 1 from [dbo].[user] with(nolock) where nickname=@nickname ";
                var userCount = conn.ExecuteScalar<int>(sql, new { nickname = nickName });

                if (userCount > 0) isExist = true;
            }

            return isExist;
        }

        #endregion

        #region 用户注册

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="nickName"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="userID"></param>
        /// <param name="addTime"></param>
        /// <param name="errorMessage"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        public static bool UserAdd(int channelId, string nickName, string mobile, string password, out long userID,out DateTime addTime, out string errorMessage, int userType = 0,int gender =0,int roleId = 1)
        {
            errorMessage = string.Empty;
            userID = 0L;
            addTime = DateTime.Now;

            DynamicParameters p = new DynamicParameters();
            p.Add("@ChannelId", channelId);
            p.Add("@NickName", nickName);
            p.Add("@Moblie", mobile);
            p.Add("@Type", userType);
            p.Add("@gender", gender);
            p.Add("@RoleId", roleId);

            p.Add("@UserID",  0L, DbType.Int64, ParameterDirection.Output);
            p.Add("@AddTime", DateTime.MinValue, DbType.DateTime, ParameterDirection.Output);

            try
            {
                using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                    conn.Execute("User_Add", p, null, null, CommandType.StoredProcedure);

                    userID = p.Get<long>("@UserID");
                    addTime = p.Get<DateTime>("@AddTime");
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 更新密码

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <param name="errorMessage"></param>
        /// <param name="SecurityCode"></param>
        /// <param name="Rcode"></param>
        /// <returns></returns>
        public static bool UpdatePassword(long userID, string password, int level, out string errorMessage, string SecurityCode, string Rcode )
        {
            errorMessage = string.Empty;

            string sql = " UPDATE [dbo].[User] SET [Password] = @Password, [SecurityCode] = @SecurityCode,[PwdSecurityLevel] = @PwdLevel, [Rcode] = @Rcode WHERE [UserID] = @UserID ";

            DynamicParameters p = new DynamicParameters();
            p.Add("@Password", password, DbType.String);
            p.Add("@SecurityCode", SecurityCode, DbType.String);
            p.Add("@PwdLevel", level, DbType.Int16);
            p.Add("@Rcode", Rcode, DbType.String);
            p.Add("@UserID", userID,DbType.Int64);

            try
            {
                using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                     conn.Execute(sql, p);
                     return true;
                }
            }
            catch(Exception ex)
            {
                errorMessage = ex.ToString();
                return false;
            }
        }

        #endregion

        #region 更新用户最后登录时间

        /// <summary>
        /// 更新用户最后登录时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool UpdateLastVisitTime(long userId,string ip)
        {
            try
            {
                string sql = " Update [dbo].[User] SET [LastVisitTime] =@LastVisitTime,ip1=@ip1 Where UserID = @UserID ";

                DynamicParameters p = new DynamicParameters();
                p.Add("@LastVisitTime", DateTime.Now);
                p.Add("@UserID", userId);
                p.Add("@ip1", ip);

                using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                    conn.Execute(sql, p);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 是否是黑名单ip

        /// <summary>
        /// 是否是黑名单ip
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool IsBlackUser(long userId)
        {
            bool isExist = false;
            using (IDbConnection conn = new SqlConnection(DB_UserConnection))
            {
                string sql = " select top 1 1 from dbo.[Live_User_BlackList] with(nolock) where userid=@userid ";
                var userCount = conn.ExecuteScalar<int>(sql, new { userid = userId });

                if (userCount > 0) isExist = true;
            }

            return isExist;
        }

        #endregion
    }
}
