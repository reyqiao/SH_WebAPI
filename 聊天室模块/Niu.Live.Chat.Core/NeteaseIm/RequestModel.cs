using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;

namespace Niu.Live.Chat.Core.NeteaseIm
{
    internal class RequestModel
    {




        public static string DataInterface(string url, Dictionary<string, string> parameters)
        {
            string jsonResult = string.Empty;

            Dictionary<string, string> headerParameters = new Dictionary<string, string>();

            string appkey = NeteaseIm.AppKey;
            string appSecret = NeteaseIm.AppSecret;
            string nonce = CheckModel.GetNonce();
            string curTime = CheckModel.GetUtcTimeStamp().ToString();
            string checkSum = CheckModel.GetCheckSum(appSecret, nonce, curTime);

            headerParameters.Add("AppKey", appkey);
            headerParameters.Add("Nonce", nonce);
            headerParameters.Add("CurTime", curTime);
            headerParameters.Add("CheckSum", checkSum);

            using (HttpWebResponse webResponse = PostHttps(url, parameters, headerParameters))
            {
                using (var httpStreamReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    jsonResult = httpStreamReader.ReadToEnd();
                }
            }
            return jsonResult;
        }


        internal static HttpWebResponse PostHttps(string url, IDictionary<string, string> parameters, IDictionary<string, string> headerParameters = null, int timeout = 10000, Encoding requestEncoding = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                requestEncoding = Encoding.UTF8;
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //如果是发送HTTPS请求  
            if (true == url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; });
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;

            }

            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers["Accept-Charset"] = "utf-8";
            request.AllowAutoRedirect = false;
            request.KeepAlive = true;
            request.Timeout = timeout;

            if (headerParameters != null)
            {
                foreach (KeyValuePair<string, string> kv in headerParameters)
                {
                    request.Headers[kv.Key] = kv.Value;
                }
            }

            //如果需要POST数据  
            if (parameters != null && parameters.Count > 0)
            {
                string parametersbuffer = string.Join("&", parameters.Select(t => string.Format("{0}={1}", t.Key, HttpUtility.UrlEncode(t.Value))).ToList());
                byte[] data = requestEncoding.GetBytes(parametersbuffer);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                return request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    return e.Response as HttpWebResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }



    }
}