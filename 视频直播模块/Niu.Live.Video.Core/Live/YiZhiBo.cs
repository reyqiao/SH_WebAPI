using Niu.Cabinet.Cryptography;
using Niu.Live.Video.Core.Utils;
using Niu.Live.Video.DataAccess;
using Niu.Live.Video.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Niu.Live.Video.Core.Live
{
    public class YiZhiBo
    {
        public const string host = "http://api.open.xiaoka.tv";
        public const string sdkid = "10014";
        public const string sdk_secrect = "ngw(&%#%";



        private string getDefaultSign()
        {
            return Niu.Cabinet.Cryptography.MD5Service.Create(string.Format("{0}&{1}", sdk_secrect, GetTimeStamp())).ToLower();
            //  return Cryptogram.GetMD5(string.Format("{0}&{1}", sdk_secrect, WebUtility.GetTimeStamp())).ToLower();
        }

        private string getSign(Dictionary<string, string> sign, out string timestamp)
        {
            var so = sign.OrderBy(s => Ascii(s.Key));
            var signstr = "";
            foreach (var s in so)
            {
                signstr += s.Key + "=" + s.Value + "&";
            }
            timestamp = GetTimeStamp();
            signstr += string.Format("{0}&{1}", sdk_secrect, timestamp);
            return MD5Service.Create(signstr).ToLower();
        }
        private string getvalue(string key, Dictionary<string, string> dic)
        {
            return dic.ContainsKey(key) ? dic[key] : "";
        }


        public BaseResult Login(long userid)
        {
            var user = VideoListAccess.GetUser(userid);
            if (user == null || user.Count < 1) return new BaseResult() { result = 0, code = -10, message = "没有权限" };

            var postdata = new Dictionary<string, string>();
            postdata.Add("sdkid", sdkid);
            postdata.Add("time", GetTimeStamp());
            postdata.Add("sign", getDefaultSign());
            postdata.Add("memberid",user.memberid);

            var addr = host + "/openapi/member/memberid_login";
            var respdata = PostUnicode2String(addr, postdata);

            var jsondata = Deserialize(respdata);
            if (jsondata.result != 1)
            {
                return new BaseResult() { result = 0, code = -1, message = jsondata.msg };
            }
            var dbdata = new Dictionary<string, string>();
            dbdata.Add("accesstoken", jsondata.data.accesstoken);
            dbdata.Add("refreshtoken", jsondata.data.refreshtoken);


            if (!VideoListAccess.UpdateUser(userid, dbdata))
                return new BaseResult() { result = 0, code = -10, message = "更新失败！" };

            return new BaseResult() { result = 1, code = 0, message = "成功！" };
        }

        public BaseResult CreateVideo(long userid, string desc, string title, string cover, string mainId)
        {
            var user = VideoListAccess.GetUser(userid);
            if (user == null || user.Count < 1) return new BaseResult() { result = 0, code = -10, message = "没有权限" };

            var timestamp = "";
            var signdata = new Dictionary<string, string>();
            signdata.Add("sdkid", sdkid);
            signdata.Add("_accesstoken",user.accesstoken);
            signdata.Add("address", "北京");
            signdata.Add("desc", desc);
            signdata.Add("title", title);
            signdata.Add("memberid",user.memberid);
            //signdata.Add("cover", cover);
            var signstr = getSign(signdata, out timestamp);


            var postdata = new Dictionary<string, string>();
            postdata.Add("sdkid", sdkid);
            postdata.Add("time", timestamp);
            postdata.Add("sign", signstr);
            postdata.Add("_accesstoken",user.accesstoken);
            postdata.Add("address", "北京");
            postdata.Add("desc", desc);
            postdata.Add("title", title);
            postdata.Add("memberid",user.memberid);
            postdata.Add("cover", cover);

            //todo:测试域名
            var addr = "http://test.api.xiaoka.tv" + "/openapi/live/create_live_video";
            //var addr = host + "/openapi/live/create_live_video";
            var respdata = PostUnicode2String(addr, postdata);

            var jsondata = Deserialize(respdata);
            if (jsondata.result != 1)
            {
                return new BaseResult() { result = 0, code = -1, message = jsondata.msg };
            }


            var video = new Dictionary<string, string>();
            video.Add("memberid", getvalue("memberid", user));
            video.Add("userid", userid.ToString());
            video.Add("mainId", mainId);
            video.Add("title", title);
            video.Add("videoDesc", desc);
            video.Add("cover", cover);
            video.Add("startTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            video.Add("rtmpurl", jsondata.data.rtmpurl);
            video.Add("scid", jsondata.data.scid);
            video.Add("rtmp_domain", jsondata.data.rtmp_domain);
            video.Add("status", jsondata.data.status);
            video.Add("covers_s", jsondata.data.covers.s);
            video.Add("covers_m", jsondata.data.covers.m);
            video.Add("covers_b", jsondata.data.covers.b);
            video.Add("cover_domain", jsondata.data.cover_domain);
            video.Add("m3u8url", jsondata.data.m3u8url);
            video.Add("swfurl", jsondata.data.swfurl);
            if (string.IsNullOrWhiteSpace(VideoListAccess.AddVideo(video)))
                return new BaseResult() { result = 0, code = -10, message = "添加失败！" };

            return new VideoResult() { result = 1, code = 0, data = "{\"scid\":\"" + jsondata.data.scid + "\",\"rtmpurl\":\"" + jsondata.data.rtmpurl + "\"}" };
        }

        public BaseResult CloseVideo(long userid, string scid)
        {
            var user = VideoListAccess.GetUser(userid);
            if (user == null || user.Count < 1) return new BaseResult() { result = 0, code = -10, message = "没有权限" };

            var timestamp = "";
            var signdata = new Dictionary<string, string>();
            signdata.Add("sdkid", sdkid);
            signdata.Add("_accesstoken", getvalue("accesstoken", user));
            signdata.Add("memberid", getvalue("memberid", user));
            signdata.Add("scid", scid);
            var signstr = getSign(signdata, out timestamp);

            var postdata = new Dictionary<string, string>();
            postdata.Add("sdkid", sdkid);
            postdata.Add("time", timestamp);
            postdata.Add("sign", signstr);
            postdata.Add("_accesstoken", getvalue("accesstoken", user));
            postdata.Add("memberid", getvalue("memberid", user));
            postdata.Add("scid", scid);

            var addr = host + "/openapi/live/finish_live_video";
            var respdata = PostUnicode2String(addr, postdata);

            var jsondata = Deserialize(respdata);
            if (jsondata.result != 1)
            {
                return new BaseResult() { result = 0, code = -1, message = jsondata.msg };
            }

            if (!VideoListAccess.CloseVideo(scid))
                return new BaseResult() { result = 0, code = -10, message = "关闭失败！" };
            return new BaseResult() { result = 1, code = 0, message = "成功！" };
        }


        public BaseResult SendSms(string mobile, string country = "86")
        {
            var br = new BaseResult();
            var postdata = new Dictionary<string, string>();
            postdata.Add("sdkid", sdkid);
            postdata.Add("time", GetTimeStamp());
            postdata.Add("sign", getDefaultSign());
            postdata.Add("mobile", mobile);
            postdata.Add("country", country);

            var addr = host + "/openapi/member/send_sms";
            var respdata = PostUnicode2String(addr, postdata);

            var data = Deserialize(respdata);
            if (data.result == 1)
            {
                br.result = 1;
                br.code = 0;
                br.message = data.msg;
            }
            else
            {
                br.result = 0;
                br.message = data.msg;
            }
            return br;
        }

        public bool CheckSms(string checkcode, string userid, string mobile, string country = "86")
        {
            var postdata = new Dictionary<string, string>();
            postdata.Add("sdkid", sdkid);
            postdata.Add("time", GetTimeStamp());
            postdata.Add("sign", getDefaultSign());
            postdata.Add("mobile", mobile);
            postdata.Add("country", country);
            postdata.Add("checkcode", checkcode);

            var addr = host + "/openapi/member/check_sms";
            var respdata = PostUnicode2String(addr, postdata);

            var data = Deserialize(respdata);
            var rs = data.result;
            if (rs == 1)
            {
                var m = data.data.memberid;
                if (VideoListAccess.AddAndUpdateUser(userid, mobile, m))
                    return true;
            }

            return false;
        }




        public string GetVideoOnline(long userid)
        {
            var user = Video.DataAccess.VideoListAccess.GetUser(userid);
            if (user == null || user.Count < 1) return new VideoResult() { result = 0, code = -10, message = "没有权限" }.ToFormatString();

            var postdata = new Dictionary<string, string>();
            postdata.Add("sdkid", sdkid);
            postdata.Add("time", GetTimeStamp());
            postdata.Add("sign", getDefaultSign());
            postdata.Add("memberid", getvalue("memberid", user));

            var addr = host + "/openapi/live/get_member_live";
            var respdata = PostUnicode2String(addr, postdata);

            return respdata;
        }

        public static string GetRSASign(Dictionary<string, string> data)
        {
            var so = data.OrderBy(s => Ascii(s.Key));
            var signstr = "";
            foreach (var s in so)
            {
                signstr += s.Key + "=" + s.Value + "&";
            }
            signstr = signstr.TrimEnd('&');
            return RSAEncrypt(signstr);
        }


        #region 第三方类库


        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        //字符转ASCII码：
        static string Ascii(string str)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            var bys = asciiEncoding.GetBytes(str);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bys.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                sb.Append(bys[i].ToString("X2"));//ToString("x");
            }
            return sb.ToString();
        }


        static string PostUnicode2String(string addr, Dictionary<string, string> dic)
        {
            return Unicode2String(Post(addr, dic));
        }

        /// <summary>
        /// Unicode转字符串
        /// </summary>
        /// <param name="source">经过Unicode编码的字符串</param>
        /// <returns>正常字符串</returns>
        static string Unicode2String(string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                         source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }

        static string Post(string addr, Dictionary<string, string> dic)
        {
            var data = string.Empty;
            foreach (var item in dic)
            {
                data += item.Key + "=" + item.Value + "&";
            }
            data = data.TrimEnd('&');
            return Post(addr, data);
        }
        static string Post(string addr, string data)
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
                Niu.Cabinet.Logging.LogRecord log = new Cabinet.Logging.LogRecord("");
                log.WriteSingleLog("", "访问网络错误！地址：{0}；参数：{1}；错误：{2}" + addr, data, ex.Message);
                // LogRecord.writeLogsingle("访问网络错误！地址：{0}；参数：{1}；错误：{2}" + addr, data, ex.Message);
                return "";
            }

        }

        static dynamic Deserialize(string json)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            return serializer.Deserialize<object>(json);
        }
        static string RSAEncrypt(string content)
        {
            var publickey = @"<RSAKeyValue><Modulus>nu13laDOGLlqOQE00XvSdOT90CltT7v+t+HWfcwiWxGpLggYvFbrCOIQ1kW1hg/5TSSr51sy+ijN0LkOl0A88F7JkDxQExl4E/tVH+USfKtoutVTGjky7nbORYJKh9zlZmBS/1tYVSc4GLqxnMhc2Rf8gigNljiYDqoWNl1p+uE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            return Convert.ToBase64String(cipherbytes);
        }
        #endregion

    }
}
