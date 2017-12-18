using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BaseInfo
    {
        public BaseInfo() { }

        //唯一标识码
        public string macId;
        public string MacId
        {
            get { return macId; }
            set { macId = value; }
        }
        //系统来源 安卓 1 IOS 2
        public int source;
        public int Source
        {
            get { return source; }
            set { source = value; }
        }
        //App版本号
        public string version;
        public string Version
        {
            get { return version; }
            set { version = value; }

        }
        //开户通道
        public string channel;
        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }
        //手机型号
        public string phonetype;
        public string PhoneType
        {
            get { return phonetype; }
            set { phonetype = value; }
        }
        //App市场
        public string appMarket;
        public string AppMarket
        {
            get { return appMarket; }
            set { appMarket = value; }
        }

    }

}
