using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Chat.Core.Model
{
    public class Im_Chatroom_Msg
    {
        public long id { get; set; }

        public string msgId { get; set; }

        public int msgType { get; set; }

        public long roomId { get; set; }

        public string fromAccId { get; set; }

        public int resendFlag { get; set; }

        public string attach { get; set; }

        public string ext { get; set; }
        public int IsManger { get; set; }
        public int RoleId { get; set; }

    }
}