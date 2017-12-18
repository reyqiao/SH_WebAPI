using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Niu.Live.User.Api.Controllers
{
    using Niu.Live.User.Model;
    using Niu.Live.User.BusinessLogic;

    /// <summary>
    /// 用户控制器类
    /// </summary>
    public class UserController : BaseController
    {
        #region 用户登录

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="channelId">渠道ID</param>
        /// <param name="mobile">手机号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage login(UserInfo userInfo)
        {
            BaseResult result = new BaseResult() { result = (int)ResultType.Error, code = (int)CodeType.ParametersError };
            if (userInfo == null) return convertJson(result.ToString());

            //用户登录
            string strResponse = UserBLL.userLogin(base.requestInfo, userInfo.channelId, userInfo.mobile, userInfo.password);

            return convertJson(strResponse);
        }

        #endregion

        #region 用户登录

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="channelId">渠道ID</param>
        /// <param name="mobile">手机号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage login(string mobile, string password)
        {
            BaseResult result = new BaseResult() { result = (int)ResultType.Error, code = (int)CodeType.ParametersError };
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(password)) return convertJson(result.ToString());

            //用户登录
            string strResponse = UserBLL.userLogin(base.requestInfo,1, mobile, password);

            return convertJson(strResponse);
        }

        #endregion

        #region 用户注册

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="code">验证码</param>
        /// <param name="nickName">昵称</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage register(string mobile,string code,string nickName,string password)
        {
            int channelId = 1;
            //用户注册
            string strResponse =  UserBLL.userRegister(base.requestInfo, channelId, nickName, mobile, password, code);

            return convertJson(strResponse);
        }

        #endregion

        #region 用户添加

        /// <summary>
        /// 用户添加
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="code">验证码</param>
        /// <param name="nickName">昵称</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage userAdd(string mobile, string nickName, string password)
        {
            int channelId = 1;
            //用户注册
            string strResponse = UserBLL.userAdd(base.requestInfo, channelId, nickName, mobile, password);

            return convertJson(strResponse);
        }

        #endregion

        #region 密码相关

        #region 修改密码

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage modifyPassword(string oldPwd, string newPwd)
        {
            //令牌解析
            string userToken = base.userToken;
            TokenUserInfo userInfo = null;
            Niu.Live.User.Core.TokenManager.ValidateMobileUserToken(userToken, out userInfo);

            string strResponse = UserBLL.modifyPassword(base.requestInfo, oldPwd, newPwd, userInfo);

            return convertJson(strResponse);
        }

        #endregion

        #region 重置密码

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="code">验证码</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage resetPassword(int channelId, string mobile, string code,string password)
        {
            //重置密码
            string strResponse = UserBLL.resetPassword(base.requestInfo, channelId, mobile, code, password);

            return convertJson(strResponse);
        }

        #endregion

        #endregion

        #region 获取用户信息

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage getUserInfo(long userId)
        {
            //获取用户信息
            string strResponse = UserBLL.getUserInfo(base.userToken,userId);

            return convertJson(strResponse);
        }

        #endregion

        #region 获取用户登录信息

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage getLoginInfo(string mobile)
        {
            //获取登录信息
            string strResponse = UserBLL.getLoginInfo(base.channelId, mobile);

            return convertJson(strResponse);
        }

        #endregion
    }
}
