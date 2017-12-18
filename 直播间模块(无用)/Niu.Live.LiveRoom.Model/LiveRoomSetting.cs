using System;
using System.Collections.Generic;

namespace Niu.Live.LiveRoom.Model
{
    public class LiveRoomSetting
    {
        public long LiveRoomId { get; set; }
        public object Marking { get; set; }
        public int Audit { get; set; }
        public int Visitor { get; set; }
        public LiveRoomSetting()
        {
            LiveRoomId = 0;
            Marking = "[]";
            Audit = 0;
            Visitor = 1;
        }
    }
    public class CustomerQQ
    {
        public string QQ { get; set; }
        public string CustomerName { get; set; }
    }
}
