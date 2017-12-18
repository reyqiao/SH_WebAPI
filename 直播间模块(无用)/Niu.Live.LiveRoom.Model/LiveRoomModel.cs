using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.LiveRoom.Model
{
    public class LiveRoomModel
    {
        public long LiveId { get; set; }

        public int ChannelId { get; set; }

        public string Title { get; set; }

        public string KeyWord { get; set; }

        public string Description { get; set; }
        public string LogoUrl { get; set; }
        /// <summary>
        /// 直播间状态 0 关闭 1图文直播中 2 视频直播中
        /// </summary>
        public int OpenType { get; set; }
        public long ChatRoomId { get; set; }
        public string ChatTitle { get; set; }
        public string Tel { get; set; }//电话
        public string Pop { get; set; }//弹窗
        public string PopUrl { get; set; }//弹窗地址
        public string AccId { get; set; }//accid
        public long UserId { get; set; }//用户id
    }
}
