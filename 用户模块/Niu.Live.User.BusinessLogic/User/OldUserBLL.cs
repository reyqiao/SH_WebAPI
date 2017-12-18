using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.BusinessLogic.User
{
    using Niu.Live.User.DataAccess.User;
    using Niu.Cabinet.Cryptography;

    public class OldUserBLL
    {
        #region 历史数据处理

        /// <summary>
        /// 历史数据处理
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        public static bool userDealWith(string mobile, string password,out long userId)
        {
            #region 属性

            var tempPassword = MD5Service.Create(password);
            bool isSuccess = false;
            int channelId = 1;
            string errorMessage = string.Empty;
            userId = 0L;
            int userType = 0;
            string nickName = string.Empty;

            #endregion

            var userInfo =  OldUserAccess.getUserInfo(mobile, tempPassword);
            if (userInfo == null) return isSuccess;

            nickName = userInfo.nickName;

            if (UserAccess.IsExisNickName(nickName))
                nickName = UserUtils.autoGgenerateName(nickName);

            //执行注册程序
            isSuccess = UserBLL.autoRegister(channelId, nickName, userInfo.mobile, password, out errorMessage, out userId, userType, 0,userInfo.roleId);

            return isSuccess;
        }

        #endregion
    }
}
