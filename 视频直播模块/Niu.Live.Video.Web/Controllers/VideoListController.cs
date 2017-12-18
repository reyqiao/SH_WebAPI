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
    public class VideoListController : Controller
    {
        public string List(string usertoken, long liveid = 0, int index = 1, int size = 10)
        {
            var videos = VideoList.Instance.ListByLiveId(liveid, index, size);
            return JsonHelp.CamelCaseSerializeObject(new { code = 0, data = videos });
        }

        public string GetVideo(string usertoken, long videoid)
        {
            var video = VideoList.Instance.GetVideo(videoid);
            return JsonHelp.CamelCaseSerializeObject(new { code = 0, data = video });
        }
        [HttpPost]
        public string UpdateVideo(string usertoken, VideoModel video)
        {
            if (VideoList.Instance.UpdateVideo(video))
                return JsonHelp.CamelCaseSerializeObject(new { code = 0 });
            else
                return JsonHelp.CamelCaseSerializeObject(new { code = -1 });
        }
        public string PlayLive(string usertoken = "", int liveid = 0)
        {
            VideoResult br = new VideoResult() { data = "\"没有数据\"" };
            var dt = VideoList.GetLiveVideo(liveid.ToString());
            if (dt != null)
            {
                br.code = 0;
                br.result = 1;
                br.data = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            return br.ToFormatString();
        }
    }
}