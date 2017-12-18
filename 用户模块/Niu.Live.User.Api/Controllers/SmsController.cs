using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Text;

namespace Niu.Live.User.Api.Controllers
{
    #region using

    using Niu.Live.User.Model;
    using Niu.Live.User.BusinessLogic;
    using Niu.Live.User.Core.Cryptogram;
    using Niu.Live.User.Core.Json;
    using Niu.Live.User.Core.Extension;
    using Niu.Live.User.Core;

    #endregion

    /// <summary>
    /// 短信控制器类
    /// </summary>
    public class SmsController : BaseController
    {
        #region 获取验证码

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="mobile"></param>
        /// <param name="smsType"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage getSmsCode(string mobileParam,string sign)
        {
            #region 变量

            string errorMessage = string.Empty;
            BaseResult result = new BaseResult() { result = (int)ResultType.Error,code =(int)CodeType.ParametersError, message = CodeType.ParametersError.GetDescription() };
            string mobile = string.Empty;
            int smsType = 0;
            string mobileJson = string.Empty;
            Dictionary<string,object> dic = null;
            int channelId = 1;

            #endregion

            if (string.IsNullOrEmpty(mobileParam) || string.IsNullOrEmpty(sign)) return convertJson(result.ToString());
                
            try
            {
                mobileJson = Cryptogram.DecodeParam(mobileParam);

                if (!Cryptogram.Verify(mobileJson,sign)) return convertJson(result.ToString());

                dic = JSONHelper.ParseJson(mobileJson);

                if (dic == null) return convertJson(result.ToString());

                mobile = dic.SafeGetValue<string, object, string>("mobile");
                smsType = dic.SafeGetValue<string, object, int>("smsType");
            }
            catch
            {
                return convertJson(result.ToString());
            }

            if (channelId <= 0 || smsType <= 0) return convertJson(result.ToString());
            if (string.IsNullOrEmpty(mobile)) return convertJson(result.ToString());

            bool isSuccess = SmsCodeBLL.SendCode(base.requestInfo, channelId, mobile, base.requestInfo.UserIP, smsType, out errorMessage);

            result.result = isSuccess ? (int)ResultType.Success : (int)ResultType.Error;
            result.code = isSuccess ? (int)CodeType.Default : (int)CodeType.DbError;
            result.message = isSuccess ? CodeType.Default.GetDescription() : errorMessage;

            return convertJson(result.ToString());
        }

        #endregion

        #region 校验验证码是否正确

        /// <summary>
        /// 校验验证码是否正确
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <param name="smsType"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage checkSmsCode(string mobile, string code, int smsType)
        {
            int channelId = 1;
            string errorMessage = string.Empty;
            CodeType codeType;
            BaseResult result = new BaseResult() { result = (int)ResultType.Error, code = (int)CodeType.ParametersError, message = CodeType.ParametersError.GetDescription() };

            if (channelId == 0) return convertJson(result.ToString());
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(code)) return convertJson(result.ToString());

            bool isSuccess = SmsCodeBLL.CheckSmsCode(channelId, mobile.Trim(), code.Trim(), smsType, out codeType, out errorMessage);
            
            result.result = isSuccess ? (int) ResultType.Success : (int)ResultType.Error;
            result.code = isSuccess ? (int)CodeType.Default : (int) CodeType.DbError;
            result.message = isSuccess ? CodeType.Default.GetDescription() : CodeType.DbError.GetDescription();

            return convertJson(result.ToString());
        }

        #endregion
    }
}
