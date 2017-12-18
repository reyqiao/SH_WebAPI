using Newtonsoft.Json;
using Niu.Cabinet.Time;
using Niu.Live.User.DataAccess.User;
using Niu.Live.User.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.BusinessLogic.User
{
    public class UserAdminBLL
    {
        #region 用户登录

        #region 登录参数验证

        /// <summary>
        /// 登录参数验证
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private static bool validLoginPara(RequestInfo requestInfo, int channelId, string mobile, string password, out UserInfo uvm, out string userToken, out BaseResult result)
        {
            uvm = null;
            userToken = string.Empty;
            long userId = 0L;
            bool isSuccess = false;
            bool isValidPassword = true;
            result = new BaseResult() { result = (int)ResultType.None, code = (int)CodeType.ParametersError };

            uvm = UserUtils.getUserInfoByMobile(channelId, mobile);
            if (uvm == null || uvm.userId <= 0) //没有此用户信息
            {
                isSuccess = OldUserBLL.userDealWith(mobile, password, out userId);
                if (isSuccess && userId > 0)
                {
                    uvm = UserUtils.getUserInfoById(userId);
                    isValidPassword = false;
                    goto ctn;
                }

                result.code = (int)CodeType.LoginError;
                result.message = CodeType.LoginError.GetDescription();
                return false;
            }

        ctn:
            //生成令牌
            uvm.tokenType = Model.tokenType.Formal;
            uvm.channelId = uvm.channelId;
            Niu.Live.User.Core.TokenManager.GenerateMobileUserToken(UserUtils.convertToTokenUserInfo(uvm), out userToken);

            string pwd = UserUtils.passwordMD5(uvm.userId, uvm.addTime, password);
            //新的验证方式
            if (isValidPassword && (pwd != uvm.password || uvm.roleId != 7))
            {
                LogUserActionFacade.UserLoginFailedRecord(requestInfo, (int)CodeType.LoginError, uvm.userId, uvm.nickName, CodeType.LoginError.GetDescription());
                result.code = (int)CodeType.LoginError;
                result.message = CodeType.LoginError.GetDescription();
                return false;
            }

            return true;
        }

        #endregion

        public static string userLogin(RequestInfo requestInfo, string mobile, string password, int channelId = 1)
        {
            UserInfo uvm = null;
            string userToken = string.Empty; //用户令牌
            string errorMessage = string.Empty;
            BaseResult result = null;

            //登录参数验证
            if (!validLoginPara(requestInfo,channelId, mobile, password, out uvm, out userToken, out result))
            {
                return result.ToString();
            }

            //返回登录信息
            dynamic response = new ExpandoObject();
            dynamic userInfo = new ExpandoObject();

            response.result = (int)ResultType.Success;
            response.code = CodeType.Default;
            response.message = "登录成功";

            response.userInfo = userInfo;
            response.userInfo.state = uvm.status;
            response.userInfo.userId = uvm.userId;
            response.userInfo.userName = uvm.nickName;
            response.userInfo.userLogoUrl = "https://img.niuguwang.com/static/img/userlogo.png";
            response.userInfo.userToken = userToken;

            byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(mobile);
            var base64Mobile = Convert.ToBase64String(bt);

            response.userInfo.mobile = base64Mobile;

            //添加登录日志
            Task.Factory.StartNew(() =>
            {
                LogUserActionFacade.UserLoginSuccessRecord(requestInfo, uvm.userId);
                UserAccess.UpdateLastVisitTime(uvm.userId,requestInfo.UserIP);
            });

            return JsonConvert.SerializeObject(response);
        }

        #endregion
    }
}
