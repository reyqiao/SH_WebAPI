using Niu.Live.Video.Core.Live;
using Niu.Live.Video.DataAccess;
using Niu.Live.Video.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.BusinessLogic
{
    public class VideoLive
    {
        public static readonly VideoLive Instance = new VideoLive();
        public static readonly YiZhiBo yizhibo = new YiZhiBo();

        public VideoModel CreateLive(long userid, string title, string desc, long liveid)
        {
            //todo:校验用户是否有创建权限

            var video = ZegoLive.Instance.CreateLive(userid, "公共直播", title, desc, liveid);

            LiveRoomAccess.ChangeLiveRoomState(liveid, 2);

            return video;
        }


        public bool CloseLive(long videoid, string obscommand = "toClose")
        {
            var video = LiveVideoAccess.GetVideoById(videoid);

            if (!ZegoLive.Instance.CloseLive(video.RStreamId, obscommand))
                return false;
            if (!LiveRoomAccess.ChangeLiveRoomState(video.LiveId, 2))
                return false;

            return true;
        }
        #region 一直播
        public BaseResult Login(long userid)
        {
            return yizhibo.Login(userid);
        }
        public dynamic CreateVideo(long userid, string desc, string title, string cover, string mainId)
        {
            return ZegoLive.Instance.CreateLive(userid, "公共直播", title, desc, long.Parse(mainId));
            // return yizhibo.CreateVideo(userid, desc, title, cover, mainId);
        }
        public dynamic CloseVideo(long userid, string scid)
        {
            return ZegoLive.Instance.CloseLive(scid, "toClose");
        }
        #endregion

    }
}
