using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Web;

namespace Niu.Live.User.Api.Controllers
{
    using Niu.Live.User.Model;
    using Niu.Cabinet.Web.ExtensionMethods;
    using Niu.Cabinet.Conversion;

    /// <summary>
    /// 控制器基类
    /// </summary>
    public class BaseController : ApiController
    {
        #region 属性

        /// <summary>
        /// 获取包类型
        /// </summary>
        public int packType
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Request["packType"]);
            }
        }

        /// <summary>
        /// 用户Ip
        /// </summary>
        public string userIp { get; set; }

        /// <summary>
        /// 请求信息
        /// </summary>
        public RequestInfo requestInfo { set; get; }

        /// <summary>
        /// 用户令牌
        /// </summary>
        public string userToken
        {
            get
            {
                return HttpContext.Current.Request["userToken"];
            }
        }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int channelId
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Request["channelId"]);
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseController()
        {
            userIp = RequestExtensions.ClientIp(HttpContext.Current.Request);
            requestInfo = new RequestInfo(packType,userIp);
        }

        #endregion

        #region 把字符串输出成 json

        /// <summary>
        /// 把字符串输出成 json
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public HttpResponseMessage convertJson(string strJson)
        {
            var result = new HttpResponseMessage { Content = new StringContent(strJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            
            //增加跨域
            result.Headers.Add("access-control-allow-origin", "*");

            return result;
        }

        #endregion
    }
}
