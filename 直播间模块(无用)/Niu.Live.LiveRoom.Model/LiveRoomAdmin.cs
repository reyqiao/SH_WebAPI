using System;


namespace Niu.Live.LiveRoom.Model
{
    public class LiveRoomAdmin
    {
        public long LiveId { get; set; }
        public long UserId { get; set; }//用户id
        public string AccId { get; set; }//云信Id
        public int AdminType { get; set; }//管理员类型，1老师，2助理

        public int ChannelId { get; set; }
    }
}
