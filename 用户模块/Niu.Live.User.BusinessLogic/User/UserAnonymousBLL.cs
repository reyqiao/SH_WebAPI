using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace Niu.Live.User.BusinessLogic
{
    using Newtonsoft.Json;
    using Niu.Live.User.Core;
    using Niu.Live.User.Model;
    using Niu.Live.User.DataAccess.User;

    /// <summary>
    /// 匿名用户处理类
    /// </summary>
    public class UserAnonymousBLL
    {
        #region 获取用户信息

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public static string getUserInfo(string userToken, long userId)
        {
            TokenUserInfo user = null;

            TokenManager.ValidateMobileUserToken(userToken, out user);

            BaseResult result = new BaseResult() { result = (int)ResultType.Error, code = (int)CodeType.UserInfoError, message = "要查询的信息错误" };

            if (user == null || user.userId <= 0) return result.ToString();
            if (userId <= 0) userId = user.userId;
            if (userId <= 0)  return result.ToString();

            UserAnonymous uvm = UserUtils.getUserAnonymousInfoById(userId);

            //返回登录信息
            dynamic response = new ExpandoObject();
            dynamic userInfo = new ExpandoObject();

            response.result = (int)ResultType.Success;
            response.code = CodeType.Default;
            response.message = "获取成功";

            response.userInfo = userInfo;
            response.userInfo.userId = uvm.userId;
            response.userInfo.nickName = uvm.nickName;
            response.userInfo.userToken = userToken;
            response.userInfo.userLogoUrl = Constant.userLogoUrl;

            response.userInfo.roleId = 0;
            response.userInfo.roleName = "游客";

            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region 用户注册

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <returns></returns>
        public static string Register(int channelId,int packType)
        {
            BaseResult result = new BaseResult() { result = (int)ResultType.Error, code = (int)CodeType.InvalidChannel };

            if (channelId <= 0)
            {
                result.message = CodeType.InvalidChannel.GetDescription();
                return result.ToString();
            }

            //用户注册
            UserAnonymous userAnonymous = null;
            int status = 0; //用户状态
            int type = packType; //用户类型
            string nickName = UserUtils.autoGenerateAName();
            bool isSuccess = UserAnonymousAccess.AddUser(channelId, nickName, type, status, out userAnonymous);

            //用户令牌
            string userToken = string.Empty;
            userAnonymous.tokenType = Model.tokenType.Anonymous;
            userAnonymous.channelId = channelId;
            userAnonymous.roleName = "游客";

            TokenManager.GenerateMobileUserToken(UserUtils.convertToTokenUserInfo(userAnonymous), out userToken);

            //返回登录信息
            dynamic response = new ExpandoObject();
            dynamic userInfo = new ExpandoObject();

            response.result = isSuccess ? (int)ResultType.Success : (int)ResultType.Error;
            response.code = isSuccess ? CodeType.Default : CodeType.DbError;
            response.message = isSuccess ? "注册成功" : "注册失败";

            response.userInfo = userInfo;
            response.userInfo.userId = userAnonymous.userId;
            response.userInfo.nickName = userAnonymous.nickName;
            response.userInfo.userToken = userToken;
            response.userInfo.userLogoUrl = Constant.userLogoUrl;

            response.userInfo.roleId = 0;
            response.userInfo.roleName = "游客";



            return JsonConvert.SerializeObject(response);
        }

        #endregion
    }
}
