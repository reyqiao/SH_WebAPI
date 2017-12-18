using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.BusinessLogic
{
    using Niu.Cabinet;

    /// <summary>
    /// 密码规则处理类
    /// </summary>
    public sealed class PasswordRules
    {
        #region 生成用户唯一摘要 返回给客户端使用

        /// <summary>
        /// 生成用户唯一摘要 返回给客户端使用
        /// </summary>
        /// <param name="userID">策略宝用户ID</param>
        /// <param name="addTime">用户的注册时间</param>
        /// <returns></returns>
        public static string GenerateUserDigest(long userID, long ticks)
        {
            
            string digestFormat = "{0}:{1}";
            return Niu.Live.User.Core.Password.PasswordHelper.GetMD5(string.Format(digestFormat, userID.ToString(), ticks.ToString()));
        }

        #endregion

        #region 生成用户安全码 服务端入库使用 由客户端生成

        /// <summary>
        /// 生成用户安全码 服务端入库使用 由客户端生成
        /// </summary>
        /// <param name="userDigest">用户唯一摘要</param>
        /// <param name="password">用户明文密码</param>
        /// <returns></returns>
        public static string GenerateSecurityCode(string userDigest, string password)
        {
            string securityCodeFormat = "{0} {1}";
            return Niu.Live.User.Core.Password.PasswordHelper.GetMD5(string.Format(securityCodeFormat, userDigest, password));
        }

        #endregion

        #region 用户明文密码MD5加密逻辑,再加密

        /// <summary>
        /// 用户明文密码MD5加密逻辑,再加密
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="addTime">注册时间</param>
        /// <param name="password">明文密码</param>
        /// <returns>密码MD5,再加密</returns>
        public static string PasswordMd5(long userID, DateTime addTime, string password)
        {
            string p = Niu.Live.User.Core.Password.PasswordHelper.GetMD5(string.Format("{0}'{1}'{2}", userID, addTime.ToString("yyyyMMdd HH:mm:ss"), password));
            return Niu.Cabinet.Cryptography.NiuCryptoService.EncryptPassword(p);
        }

        #endregion
    }
}