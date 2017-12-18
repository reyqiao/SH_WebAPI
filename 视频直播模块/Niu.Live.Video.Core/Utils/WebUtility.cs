using Niu.Cabinet.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.Core.Utils
{
    public class WebUtility
    {

        public static string Post(string addr, string data)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                var encoding = Encoding.UTF8;


                byte[] postData = encoding.GetBytes(data);
                byte[] responseData = wc.UploadData(addr, "post", postData);
                return encoding.GetString(responseData);
            }
            catch (Exception ex)
            {
                return string.Format("访问网络错误！地址：{0}；参数：{1}；错误：{2}" + addr, data, ex.Message);
            }

        }

        public static string Post(string addr, Dictionary<string, string> dic)
        {
            var data = string.Empty;
            foreach (var item in dic)
            {
                data += item.Key + "=" + item.Value + "&";
            }
            data = data.TrimEnd('&');
            return Post(addr, data);
        }



        public static string Get(string addr)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(addr);
        }

        public static string Get(string addr, Dictionary<string, string> dic)
        {
            var data = string.Empty;
            foreach (var item in dic)
            {
                data += item.Key + "=" + item.Value + "&";
            }
            data = data.TrimEnd('&');
            addr = addr + "?" + data;
            return Get(addr);
        }
    }
}
