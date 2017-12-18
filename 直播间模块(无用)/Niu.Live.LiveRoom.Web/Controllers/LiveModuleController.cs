using Niu.Live.LiveRoom.BusinessLogic;
using Niu.Live.LiveRoom.Model;
using Niu.Live.LiveRoom.Utils;
using Niu.Live.LiveRoom.Web.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Niu.Live.LiveRoom.Web.Controllers
{
    [JsonFilter]
    public class LiveModuleController : Controller
    {
        public string GetAllLiveModule(string usertoken, long liveid)
        {
            var livemodules = LiveModuleBL.Instance.GetAllLiveModule();
            return JsonHelp.CamelCaseSerializeObject(new { code = 0, data = livemodules });
        }
        [HttpPost]
        public string AddLiveModule(string usertoken, LiveModule livemodule)
        {
            if (LiveModuleBL.Instance.AddLiveModule(livemodule))
                return JsonHelp.CamelCaseSerializeObject(new { code = 0 });
            else
                return JsonHelp.CamelCaseSerializeObject(new { code = -1 });
        }
        [HttpPost]
        public string UpdateLiveModule(string usertoken, LiveModule livemodule)
        {
            if (LiveModuleBL.Instance.UpdateLiveModule(livemodule))
                return JsonHelp.CamelCaseSerializeObject(new { code = 0 });
            else
                return JsonHelp.CamelCaseSerializeObject(new { code = -1 });
        }
        [HttpPost]
        public string UploadModulePicture(string usertoken)
        {
            return JsonHelp.CamelCaseSerializeObject(new { code = 0, data = "" });
        }
    }
}