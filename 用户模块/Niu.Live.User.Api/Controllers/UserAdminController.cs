using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Niu.Live.User.Api.Controllers
{
    using Niu.Live.User.BusinessLogic;
    using Niu.Live.User.BusinessLogic.User;
    using Niu.Live.User.Model;

    public class UserAdminController : BaseController
    {
        #region 管理员登录

        [HttpPost]
        public HttpResponseMessage login(UserInfoAdmin userInfo)
        {
            BaseResult result = new BaseResult() { result = (int)ResultType.Error, code = (int)CodeType.ParametersError };
            if (userInfo == null) return convertJson(result.ToString());

            //用户登录
            string strResponse = UserAdminBLL.userLogin(base.requestInfo, userInfo.mobile, userInfo.password);
            
            return convertJson(strResponse);
        }

        #endregion
    }
}
