using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.Model
{
    public class VideoModel
    {
        public long VideoId { get; set; }
        public long LiveId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string HDLUrl { get; set; }
        public string RTMPUrl { get; set; }
        public string HLSUrl { get; set; }
        public string ReplayUrl { get; set; }
        public int DeleteSign { get; set; }
        public string Cover { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }

        public DateTime ReplayTime { get; set; }
        public int Weight { get; set; }
        public int ChatRoomId { get; set; }
        public string ObsCommand { get; set; }
        public int OnlineCount { get; set; }
        public int PlayCount { get; set; }

        public int RLiveId { get; set; }
        public string RLiveChannel { get; set; }
        public string RStreamId { get; set; }
    }
}
