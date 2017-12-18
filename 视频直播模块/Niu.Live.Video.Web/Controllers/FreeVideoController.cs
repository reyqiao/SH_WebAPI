using Niu.Live.Video.BusinessLogic;
using Niu.Live.Video.Model;
using Niu.Live.Video.Utils;
using Niu.Live.Video.Web.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Niu.Live.Video.Web.Controllers
{
     [JsonFilter]
    public class FreeVideoController : Controller
    {
        public string List(string usertoken, long liveid = 0)
        {
            var freevideos = FreeVideo.Instance.GetAllByLiveId(liveid).Select(c => new { freeId = c.FreeId, videoId = c.VideoId, videoTitle=c.Video.Title });
            return JsonHelp.CamelCaseSerializeObject(new { code = 0, data = freevideos });
        }
               [HttpPost]
        public string Add(string usertoken, FreeVideoModel freevideo)
        {
            if (FreeVideo.Instance.AddFreeVideo(freevideo))
                return JsonHelp.CamelCaseSerializeObject(new { code = 0 });
            else
                return JsonHelp.CamelCaseSerializeObject(new { code = -1 });
        }
               [HttpPost]
        public string Update(string usertoken, FreeVideoModel freevideo)
        {
            if (FreeVideo.Instance.UpdateFreeVideo(freevideo))
                return JsonHelp.CamelCaseSerializeObject(new { code = 0 });
            else
                return JsonHelp.CamelCaseSerializeObject(new { code = -1 });
        }
    }
}