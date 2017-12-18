using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.Provider.UserInfoProvider
{
    using Niu.Live.User.IModel.UserInfo;
    using Niu.Live.User.IModel;
    using Niu.Live.User.Provider.Common;
    using Niu.Live.User.Provider.UserInfoDataAccess;

    /// <summary>
    /// 用户信息提供类
    /// </summary>
    public class UserInfoProvider
    {
        #region 获取用户信息

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public static UserInfo getUserInfo(long userId,EnumManager.userType userType)
        {
            UserInfo userInfo = null;

            if (userId <= 0) return userInfo;

            Dictionary<EnumManager.userType, Func<long, UserInfo>> dic = new Dictionary<EnumManager.userType, Func<long, UserInfo>>();

            dic.Add(EnumManager.userType.Anonymous, UserInfoDataAccess.GetAnonymousByUserID);
            dic.Add(EnumManager.userType.Formal, UserInfoDataAccess.GetByUserID);

            if (dic.ContainsKey(userType))
            {
                userInfo = dic[userType](userId);
            }

            if (userInfo != null 
                && string.IsNullOrEmpty(userInfo.logoPhotoUrl))
            {
                userInfo.logoPhotoUrl = Constant.userLogoUrl;
            }

            return userInfo;
        }

        #endregion
    }
}
