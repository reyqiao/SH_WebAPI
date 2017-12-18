using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Niu.Live.Models
{
    public class ChatRoom
    {
        public long roomid { get; set; }//聊天室id
        public string name { get; set; }//聊天室名称，长度限制128个字符
        public string operatoraccid { get; set; }//操作者账号，必须是创建者才可以操作
        public string announcement { get; set; }//公告，长度限制4096个字符
        public string broadcasturl { get; set; }//直播地址，长度限制1024个字符
        public string ext { get; set; }//扩展字段，长度限制4096个字符
        public string needNotify { get; set; }//true或false,是否需要发送更新通知事件，默认true
        public string notifyExt { get; set; }//通知事件扩展字段，长度限制2048
        public bool valid { get; set; }//false:关闭聊天室；true:打开聊天室

    }
}