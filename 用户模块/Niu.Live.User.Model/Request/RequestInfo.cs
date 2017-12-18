using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.Model
{
    /// <summary>
    /// 请求信息实体
    /// </summary>
    public class RequestInfo
    {
        public int PackType { get { return _packType; } }
        private int _packType = 0;

        public string UserIP { get { return _userIP; } }
        private string _userIP = string.Empty;

        public RequestInfo(int packType, string userIP)
        {
            _packType = packType;
            _userIP = userIP;
        }
    }
}