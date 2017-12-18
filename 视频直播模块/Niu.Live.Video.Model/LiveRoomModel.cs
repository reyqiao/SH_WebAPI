using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.Model
{
    public class LiveRoomModel
    {
        public long  LiveId { get; set; }
        public int ChannelId { get; set; }
        public string Title { get; set; }
        public string KeyWord { get; set; }
        public string  Description { get; set; }
        public string  LogoUrl { get; set; }
        public int Audit { get; set; }
        public int Visitor { get; set; }
        public int OpenType { get; set; }
        public long ChatRoomId { get; set; }
        public int State { get; set; }
        public string  ChatTitle { get; set; }

    }
}
