using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.BusinessLogic
{
    using Niu.Live.User.DataAccess.RequestRecord;
    using Niu.Live.User.Model;

    /// <summary>
    /// 日志操作类
    /// </summary>
    public class LogUserActionFacade
    {
        #region 用户操作成功添加

        /// <summary>
        /// 用户操作成功添加
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="userID"></param>
        /// <param name="interfaceType"></param>
        /// <param name="extend"></param>
        public static void UserActionSuccessRecord(RequestInfo requestInfo, long userID, int interfaceType, string extend = "")
        {
            UserActionLogAccess.AddUserActionSuccess(interfaceType, requestInfo.PackType, requestInfo.UserIP, userID, extend);
        }

        #endregion

        #region 用户登录成功记录添加

        /// <summary>
        /// 用户登录成功记录
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="userID"></param>
        /// <param name="extend"></param>
        /// <returns></returns>
        public static bool UserLoginSuccessRecord(RequestInfo requestInfo, long userID, string extend = "")
        {
            return UserActionLogAccess.AddUserActionSuccess((int)InterfaceType.Login, requestInfo.PackType, requestInfo.UserIP, userID, extend);
        }

        #endregion

        #region 用户登录失败记录添加

        /// <summary>
        /// 用户登录失败记录添加
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="codeType"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <param name="extend"></param>
        /// <returns></returns>
        public static int UserLoginFailedRecord(RequestInfo requestInfo, int codeType, long userID, string userName, string message, string extend = "")
        {
            int count = 0;
            UserActionLogAccess.AddUserActionFailed((int)InterfaceType.Login, requestInfo.PackType, codeType, requestInfo.UserIP, userName, userID, extend, message, out count);

            return count;
        }

        #endregion
    }
}