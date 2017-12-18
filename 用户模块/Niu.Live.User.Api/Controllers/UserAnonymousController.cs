using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Niu.Live.User.Api.Controllers
{
    using Niu.Live.User.BusinessLogic;
    using Niu.Live.User.Model;

    /// <summary>
    /// 用户匿名控制器类
    /// </summary>
    public class UserAnonymousController : BaseController
    {
        #region 用户注册

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage register()
        {
            int channelId = 1;
            int packType = base.packType;
            //匿名用户注册
            string strResponse = UserAnonymousBLL.Register(channelId,packType);

            return convertJson(strResponse);
        }

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
            string strResponse = UserAnonymousBLL.getUserInfo(base.userToken, userId);

            return convertJson(strResponse);
        }

        #endregion
    }
}
