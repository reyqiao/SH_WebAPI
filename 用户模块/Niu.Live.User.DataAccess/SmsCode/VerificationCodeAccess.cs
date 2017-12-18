using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Niu.Live.User.DataAccess.SmsCode
{
    using Dapper;
    using Niu.Live.User.Model;

    /// <summary>
    /// 验证码数据处理类
    /// </summary>
    public class VerificationCodeAccess
    {
        #region 属性

        static string DB_UserConnection = ConfigurationManager.ConnectionStrings["DB_LiveUserConnection"].ConnectionString;

        #endregion

        #region 获取手机验证码信息

        /// <summary>
        /// 获取手机验证码信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="interfaceType"></param>
        /// <param name="verifyCodeModel"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool GetByMobile(int channelId,long mobile, int interfaceType, out VerificationCode verifyCodeModel, out string errorMessage)
        {
            errorMessage = string.Empty;
            verifyCodeModel = null;

            try
            {
                using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                    string sql = " select  channelid,mobile,interfacetype,userid,verificationcode,isverification,addtime,updatetime,sendnumber,checktime,userip from [dbo].[VerificationCode] with(nolock) where ChannelId=@ChannelId and mobile=@mobile and InterfaceType=@InterfaceType order by UpdateTime desc ";
                    verifyCodeModel = conn.Query<VerificationCode>(sql, new { mobile = mobile, InterfaceType = interfaceType, ChannelId = channelId }).FirstOrDefault();

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

        #region 短信添加

        /// <summary>
        /// 短信添加
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <param name="todaySendNumber"></param>
        /// <param name="userIP"></param>
        /// <param name="interfaceType"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool Add(int channelId, long mobile, string code, int todaySendNumber,string userIP, int interfaceType, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                    conn.Execute("VerificationCode_Add", new { ChannelId = channelId, Moblie = mobile, InterfaceType = interfaceType, VerificationCode = code, SendNumber = todaySendNumber, UserIP = userIP }, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                return false;
            }
        }

        #endregion

        #region 发送数减一

        /// <summary>
        /// 发送数减一
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="interfaceType"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool Subtraction(int channelId, long mobile, int interfaceType, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                    string sql = " update [dbo].[VerificationCode] set [SendNumber]= [SendNumber] - 1 where  ChannelId = @ChannelId and Mobile = @Moblie and InterfaceType = @InterfaceType ";
                    conn.Execute(sql, new { ChannelId = channelId, Mobile = mobile, InterfaceType = interfaceType });

                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                return false;
            }
        }
            
        #endregion

        #region 获取验证码信息

        /// <summary>
        /// 获取验证码信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <param name="todaySendNumber"></param>
        /// <param name="userIP"></param>
        /// <param name="interfaceType"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool GetByMobileCode(int channelId, long mobile, string code, DateTime updateTime, int interfaceType, out VerificationCode verifyCodeModel, out string errorMessage, bool isVertify = true)
        {
            errorMessage = string.Empty;
            verifyCodeModel = null;

            try
            {
                using (IDbConnection conn = new SqlConnection(DB_UserConnection))
                {
                    verifyCodeModel = conn.Query<VerificationCode>("VerificationCode_GetByMobileCode", new { ChannelId =channelId, Mobile = mobile, InterfaceType = interfaceType, Code = code, UpdateTime = updateTime, IsVertify = isVertify ? 1 : 0 }, null, true, null, CommandType.StoredProcedure).FirstOrDefault();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                return false;
            }
        }

        #endregion
    }
}
