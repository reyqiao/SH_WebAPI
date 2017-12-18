using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Niu.Cabinet;
using Niu.Live.User.IModel.TokenManager;
namespace Niu.Live.LiveRoom.Web.Controllers
{
    public class BaseController : Controller
    {
        public TokenUserInfo userinfo = null;
        public BaseController()
        {

        }
        /// <summary> 
        /// 错误日志（Controller发生异常时会执行这里） 
        /// </summary> 
        public class Errotattribute : ActionFilterAttribute, IExceptionFilter
        {
            /// <summary> 
            /// 异常 
            /// </summary> 
            /// <param name="filterContext"></param> 
            public void OnException(ExceptionContext filterContext)
            {
                //获取异常信息，入库保存 
                Exception Error = filterContext.Exception;
                string Message = Error.Message;//错误信息 
                string Url = filterContext.RequestContext.HttpContext.Request.RawUrl;//错误发生地址 
                filterContext.ExceptionHandled = true;
                Niu.Cabinet.Logging.LogRecord _log = new Cabinet.Logging.LogRecord(Cabinet.Config.AppSetting.AppSettingString("logpath"));
                _log.WriteSingleLog("liveroom", filterContext.Exception.Message);
                filterContext.Result = new RedirectResult("/Error/Show/");//跳转至错误提示页面 
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (Request != null)
            {
                if (!string.IsNullOrEmpty(Request["usertoken"]))
                {
                    TokenUserInfo userinfo = new TokenUserInfo();
                    if (Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(Request["usertoken"], out userinfo))
                        userinfo.accId = string.Format("{0}_{1}_{2}", userinfo.channelId, (int)userinfo.tokenType, userinfo.userId);
                    else
                        filterContext.Result = new RedirectResult("/index/");//跳转至没有登陆页 

                }
                else
                {
                    filterContext.Result = Json(new { code = -1, msg = "token无效" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}