using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.Model
{
    public class VideoResult : BaseResult
    {
        public virtual string data { get; set; }
        public string ToFormatString()
        {
            return string.Format(output, result, code, data);
        }
        const string output = "{{\"result\":{0},\"code\":{1},\"data\":{2}}}";
    }

    public class DataUtility
    {
        public static string DataToString(object obj)
        {
            if (obj == null)
                return "";
            return obj.ToString();
        }
    }
}
