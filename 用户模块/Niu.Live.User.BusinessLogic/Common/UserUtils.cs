using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;

namespace Niu.Live.User.BusinessLogic
{
    #region using

    using Niu.Cabinet;
    using Niu.Cabinet.Cryptography;
    using Niu.Live.User.Model;
    using Niu.Live.User.DataAccess.User;
    using Niu.Live.User.Core.Valid;

    #endregion

    /// <summary>
    /// 用户工具类
    /// </summary>
    public class UserUtils
    {
        #region 判断手机是否存在

        /// <summary>
        /// 判断手机是否存在
        /// </summary>
        /// <param name="mobile">手机</param>
        /// <param name="isExist">返回是否存在</param>
        /// <param name="errorMessage">返回异常信息</param>
        /// <returns>false/true</returns>
        public static bool isExistMobile(int channelId, string mobile)
        {
            mobile = Niu.Cabinet.Cryptography.NiuCryptoService.EncryptPassword(mobile);
            return UserAccess.mobileIsExist(channelId, mobile);
        }

        #endregion

        #region 判断昵称是否存在

        //<summary>
        //判断昵称是否存在
        //</summary>
        //<param name="nickname">昵称</param>
        //<param name="isExist">返回是否存在</param>
        //<param name="errorMessage">返回异常信息</param>
        //<returns>false/true</returns>
        public static bool isExistNickName(string nickname, out bool isExist)
        {
            string errorMessage = string.Empty;
            isExist = true;
            bool result = false;

            try
            {
                isExist = UserAccess.IsExisNickName(nickname);
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        #endregion

        #region 更新密码

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="addTime">用户注册时间</param>
        /// <param name="password">密码</param>
        /// <param name="errorMessage"></param>
        /// <returns>false/true</returns>
        public static bool updatePassword(long userId, DateTime addTime, string password, int level, string SecurityCode = null, string Rcode = null)
        {
            string errorMessage = string.Empty;
            string md5Str = string.Format("{0}'{1}'{2}", userId, addTime.ToString("yyyyMMdd HH:mm:ss"), password);
            string pwd = NiuCryptoService.EncryptPassword(Niu.Live.User.Core.Password.PasswordHelper.GetMD5(md5Str));

            return UserAccess.UpdatePassword(userId, pwd, level, out errorMessage, SecurityCode, Rcode);
        }

        #endregion

        #region 用户密码MD5加密逻辑,再加密

        /// <summary>
        /// 用户密码MD5加密逻辑,再加密
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="addTime">注册时间</param>
        /// <param name="password">明文密码</param>
        /// <returns>密码MD5,再加密</returns>
        public static string passwordMD5(long userID, DateTime addTime, string password)
        {
            string p = Niu.Live.User.Core.Password.PasswordHelper.GetMD5(string.Format("{0}'{1}'{2}", userID, addTime.ToString("yyyyMMdd HH:mm:ss"), password));
            return NiuCryptoService.EncryptPassword(p);
        }

        #endregion

        #region 检查用户名是否合规

        /// <summary>
        /// 检查用户名是否合规
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns>true 合规 false 不合规</returns>
        public static bool checkNickName(string nickName)
        {
            int byteLen = System.Text.Encoding.Default.GetByteCount(nickName);

            if (byteLen < 4 || byteLen > 20) return false;

            bool result = true;
            
            if (ValidHelper.IsNumeric(nickName)) return false;
            //if (IsNumber(userName[0].ToString())) return false;
            string[] list = new string[] { "·", "～", "！", "＠", "＃", "￥", "％", "…", "＆", "×", "（", "）", "—", "＝", "＋", "，", "。", "？", "、", "：", "；", "“", "”", "《", "》", "‘", "’", "＼", "｜", "　" };

            foreach (string s in list)
            {
                if (nickName.Contains(s))
                {
                    return false;
                }
            }

            Regex r = new Regex(@"^[0-9a-zA-Z\u4E00-\u9FA5\-_]+$");
            if (r.Match(nickName).Success)
            {
                for (int i = 0; i < nickName.Length; i++)
                {
                    if (Char.IsSurrogate(nickName, i))
                    {
                        return false;
                    }
                    if (Char.IsSurrogatePair(nickName, i))
                    {
                        return false;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        #endregion

        #region 根据手机获取用户信息

        /// <summary>
        /// 根据手机获取用户信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static UserInfo getUserInfoByMobile(int channelId, string mobile)
        {
            UserInfo userInfo = null;

            if(!string.IsNullOrEmpty(mobile))
            {
                mobile = NiuCryptoService.EncryptPassword(mobile);
                userInfo = UserAccess.GetByMobile(channelId, mobile);
            }

            if(userInfo != null && string.IsNullOrEmpty(userInfo.logoPhotoUrl))
            {
                userInfo.logoPhotoUrl = Constant.userLogoUrl;
            }

            return userInfo;
        }
        
        #endregion

        #region 根据Id 获取用户信息

        /// <summary>
        /// 根据Id 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static UserInfo getUserInfoById(long userId)
        {
            UserInfo userInfo = null;

            if(userId >0)  userInfo = UserAccess.GetByUserID(userId);

            if (userInfo != null && string.IsNullOrEmpty(userInfo.logoPhotoUrl))
            {
                userInfo.logoPhotoUrl = Constant.userLogoUrl;
            }

            return userInfo;
        }

        #endregion

        #region 根据Id 获取匿名用户信息

        /// <summary>
        /// 根据Id 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static UserAnonymous getUserAnonymousInfoById(long userId)
        {
            UserAnonymous userInfo = null;

            if (userId > 0)
            {
                userInfo = UserAnonymousAccess.GetByUserID(userId);
            }

            return userInfo;
        }

        #endregion

        #region 用户信息转 token 实体

        /// <summary>
        /// 用户信息转 token 实体
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static TokenUserInfo convertToTokenUserInfo(IUserInfo userInfo)
        {
            TokenUserInfo tokenUserInfo = null;

            if (userInfo != null)
            {
                tokenUserInfo = new TokenUserInfo()
                {
                    userId = userInfo.userId,
                    nickName = userInfo.nickName,
                    tokenType = userInfo.tokenType,
                    status = userInfo.status,
                    type = (userType)userInfo.type,
                    channelId = userInfo.channelId,
                    roleId = userInfo.roleId,
                    roleName = userInfo.roleName,
                    isManage = userInfo.isManage
                };
            }

            return tokenUserInfo;
        }

        #endregion

        #region 自动生成匿名用户昵称

        /// <summary>
        /// 自动生成用户昵称,规则游客 + 3字母 + 用户ID
        /// </summary>
        /// <returns></returns>
        public static string autoGenerateAName()
        {
            string str = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
            Random r = new Random();

            string nickName = string.Format("游客{0}{1}{2}", str[r.Next(0, 51)], str[r.Next(0, 51)], str[r.Next(0, 51)]);

            return nickName;
        }

        #endregion

        #region 自动生成用户昵称

        /// <summary>
        /// 自动生成用户昵称
        /// </summary>
        /// <returns></returns>
        public static string autoGgenerateName(string userName = "")
        {
            string nickName = string.Empty;
            string tempStr = string.Empty;
            string tempNumber = string.Empty;
            int count = 0;
            string str = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
            Random r = new Random();
            int min = 0, max = 999;

            do
            {
                tempStr = string.Format("{0}{1}{2}", str[r.Next(0, 51)], str[r.Next(0, 51)], str[r.Next(0, 51)]);
                tempNumber = r.Next(min, max).ToString();

                nickName = string.Format("{0}{1}{2}",userName, tempStr, tempNumber);

                if (!UserAccess.IsExisNickName(nickName))
                {
                    break;
                }

                min += 50;
                max += 100;

                count++;
            } while (count < 8);

            if (count == 8)
            {
                nickName = string.Format("{0}{1}",userName, DateTime.Now.ToString("yyMMddHHmmssfff"));
            }

            return nickName;
        }

        #endregion

        #region 是否是黑名单用户

        /// <summary>
        /// 是否黑名单用户
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="isExist"></param>
        /// <returns></returns>
        public static bool isBlackUser(long userId)
        {
            bool isBlackUser = false;
            try
            {
                isBlackUser = UserAccess.IsBlackUser(userId);
            }
            catch{ }

            return isBlackUser;
        }

        #endregion
    }
}