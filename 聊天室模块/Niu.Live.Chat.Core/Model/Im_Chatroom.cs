using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Chat.Core.Model
{
    public class Im_Chatroom
    {

        public long roomid { get; set; }

        public bool valid { get; set; }

        public string announcement { get; set; }

        public string name { get; set; }

        public string broadcasturl { get; set; }

        public int onlineusercount { get; set; }

        public string ext { get; set; }

        public string creator { get; set; }


    }
    /// <summary>
    /// 临时禁言对象
    /// </summary>
    public class TempMuteRoom
    {
        public long roomid { get; set; }//聊天室id
        public string accid { get; set; }//操作者accid,必须是管理员或创建者
        public string target { get; set; }//被禁言的目标账号accid
        public long muteDuration { get; set; }//0:解除禁言;>0设置禁言的秒数，不能超过2592000秒(30天)
        public string needNotify { get; set; }//操作完成后是否需要发广播，true或false，默认true
        public string notifyExt { get; set; }//
    }
}