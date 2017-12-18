using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Chat.Core.Model
{
    public class Im_User
    {
        public string accid { get; set; }

        public string name { get; set; }

        public string token { get; set; }

        public bool blockFlag { get; set; }
        public long userId { get; set; }//牛股王Id



    }
    public class Block_User
    {
        public string AccId { get; set; }//操作者ID
        public string Target { get; set; }//被加黑Id
    }

}