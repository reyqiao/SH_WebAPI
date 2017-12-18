using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Niu.Live.LiveRoom.Web.Filter
{
    public class JsonFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.ContentType = "application/json";

            filterContext.HttpContext.Response.Headers.Add("access-control-allow-origin", "*");
            base.OnResultExecuted(filterContext);
        }
    }
}