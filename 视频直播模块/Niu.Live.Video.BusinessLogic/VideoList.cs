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
    public class VideoList
    {
        public static readonly VideoList Instance = new VideoList();
        public IEnumerable<VideoModel> ListByLiveId(long liveid, int index, int size)
        {
            return VideoListAccess.ListByLiveId(9072833, index, size);
        }

        public VideoModel GetVideo(long videoid)
        {
            return VideoListAccess.GetVideo(videoid);
        }

        public bool UpdateVideo(VideoModel video)
        {
            return VideoListAccess.UpdateVideo(video);
        }
        public static dynamic GetUser(long userid)
        {
            return VideoListAccess.GetUser(userid);
        }
        public static string AddVideo(Dictionary<string, string> video)
        {
            return VideoListAccess.AddVideo(video);
        }
        public static bool CloseVideo(string scid)
        {
            return VideoListAccess.CloseVideo(scid);
        }
        public static bool UpdateUser(long userid, Dictionary<string, string> dictionary)
        {
            return VideoListAccess.UpdateUser(userid, dictionary);
        }
        public static dynamic GetLiveLeftJoinLiveVideoByLiveId(int liveid)
        {
            return VideoListAccess.GetLiveLeftJoinLiveVideoByLiveId(liveid);
        }

        public static dynamic GetVideo(string scid)
        {
            return VideoListAccess.GetVideo(scid);
        }
        public static dynamic GetLiveVideo(string scid)
        {
            return VideoListAccess.GetLiveVideo(scid);
        }
        public static bool UpdateReply(string arg1, DateTime arg2, DateTime arg3, string arg4)
        {
            return VideoListAccess.UpdateReply(arg1, arg2, arg3, arg4);
        }

    }
}
