using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Niu.Live.User.Core.Http
{
	public class HttpWebResponseUtility
	{
		private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0";

		static HttpWebResponseUtility()
		{
		}

		/// <summary>  
		/// 创建GET方式的HTTP请求  
		/// </summary>  
		/// <param name="url">请求的URL</param>  
		/// <param name="timeout">请求的超时时间，毫秒</param>  
		/// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
		/// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
		/// <returns></returns>  
		public static HttpWebResponse Get(string url, int? timeout, string userAgent, CookieCollection cookies, IWebProxy proxy = null)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentNullException("url");
			}
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			request.Method = "GET";
			request.Proxy = proxy;
			request.UserAgent = DefaultUserAgent;
			if (!string.IsNullOrEmpty(userAgent))
			{
				request.UserAgent = userAgent;
			}
			if (timeout.HasValue)
			{
				request.Timeout = timeout.Value;
				request.ReadWriteTimeout = timeout.Value;
			}
			if (cookies != null)
			{
				request.CookieContainer = new CookieContainer();
				request.CookieContainer.Add(cookies);
			}
			return request.GetResponse() as HttpWebResponse;
		}
		/// <summary>  
		/// 创建POST方式的HTTP请求。失败抛异常或返回null
		/// </summary>  
		/// <param name="url">请求的URL</param>  
		/// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
		/// <param name="timeout">请求的超时时间</param>  
		/// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
		/// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
		/// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
		/// <returns></returns>  
		public static HttpWebResponse Post(
			string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies, IWebProxy proxy = null)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentNullException("url");
			}
			if (requestEncoding == null)
			{
				requestEncoding = Encoding.UTF8;
				//throw new ArgumentNullException("requestEncoding");
			}
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			//如果是发送HTTPS请求  
			if (true == url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
			{
				ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(checkValidationResult);
				request = WebRequest.Create(url) as HttpWebRequest;
				request.ProtocolVersion = HttpVersion.Version10;
			}
			request.Method = "POST";
			request.Proxy = proxy;
			request.ContentType = "application/x-www-form-urlencoded";
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
			request.Headers["Accept-Language"] = "zh-CN,zh;q=0.";
			request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
			request.AllowAutoRedirect = false;
			request.KeepAlive = true;

			request.UserAgent = string.IsNullOrEmpty(userAgent) ? DefaultUserAgent : userAgent;
			if (timeout.HasValue)
			{
				request.Timeout = timeout.Value;
			}
			if (cookies != null)
			{
				request.CookieContainer = new CookieContainer();
				request.CookieContainer.Add(cookies);
			}
			//如果需要POST数据  
			if (!(parameters == null || parameters.Count == 0))
			{
				StringBuilder buffer = new StringBuilder();
				int i = 0;
				foreach (string key in parameters.Keys)
				{
					if (i > 0)
					{
						buffer.AppendFormat("&{0}={1}", key, parameters[key]);
					}
					else
					{
						buffer.AppendFormat("{0}={1}", key, parameters[key]);
					}
					i++;
				}
				byte[] data = requestEncoding.GetBytes(buffer.ToString());
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

		public static string PostData(string url, Dictionary<string, string> parameters, int timeout = 1000000, string userAgent = "", Encoding requestEncoding = null, CookieCollection cookies = null, IWebProxy proxy = null)
		{
			HttpWebResponse response = Post(url, parameters, timeout, userAgent, requestEncoding, cookies, proxy);
			StringBuilder strRet = new StringBuilder();
			if (response != null)
			{
				System.IO.Stream respStream = response.GetResponseStream();
				if (requestEncoding == null)
				{
					requestEncoding = Encoding.UTF8;
				}
				using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, requestEncoding))
				{
					System.String oneline;
					while ((oneline = reader.ReadLine()) != null)
					{
						strRet.Append(oneline);
					}
				}
			}
			return strRet.ToString();
		}

		public static string PostDataHttps(string url, Dictionary<string, string> parameters, int timeout = 1000000, string userAgent = "", Encoding requestEncoding = null, CookieCollection cookies = null)
		{
			HttpWebResponse response = PostHttps(url, parameters, timeout, userAgent, requestEncoding, cookies);
			StringBuilder strRet = new StringBuilder();
			if (response != null)
			{
				System.IO.Stream respStream = response.GetResponseStream();
				if (requestEncoding == null)
				{
					requestEncoding = Encoding.UTF8;
				}
				using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, requestEncoding))
				{
					System.String oneline;
					while ((oneline = reader.ReadLine()) != null)
					{
						strRet.Append(oneline);
					}
				}
			}
			return strRet.ToString();
		}
		public static HttpWebResponse PostHttps(
		   string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentNullException("url");
			}
			if (requestEncoding == null)
			{
				requestEncoding = Encoding.UTF8;
				//throw new ArgumentNullException("requestEncoding");
			}
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			//如果是发送HTTPS请求  
			if (true == url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
			{
				ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(checkValidationResult);
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				request = WebRequest.Create(url) as HttpWebRequest;
				request.ProtocolVersion = HttpVersion.Version10;

			}
			request.Method = "POST";
			request.Proxy = null;
			request.ContentType = "application/x-www-form-urlencoded";
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
			request.Headers["Accept-Language"] = "zh-CN,zh;q=0.";
			request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
			request.AllowAutoRedirect = false;
			request.KeepAlive = true;

			request.UserAgent = string.IsNullOrEmpty(userAgent) ? DefaultUserAgent : userAgent;
			if (timeout.HasValue)
			{
				request.Timeout = timeout.Value;
			}
			if (cookies != null)
			{
				request.CookieContainer = new CookieContainer();
				request.CookieContainer.Add(cookies);
			}
			//如果需要POST数据  
			if (!(parameters == null || parameters.Count == 0))
			{
				StringBuilder buffer = new StringBuilder();
				int i = 0;
				foreach (string key in parameters.Keys)
				{
					if (i > 0)
					{
						buffer.AppendFormat("&{0}={1}", key, parameters[key]);
					}
					else
					{
						buffer.AppendFormat("{0}={1}", key, parameters[key]);
					}
					i++;
				}
				byte[] data = requestEncoding.GetBytes(buffer.ToString());
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

		public static string PostData(string url, Dictionary<string, string> parameters, out int statusCode, int timeout = 1000000, string userAgent = "", Encoding requestEncoding = null, CookieCollection cookies = null)
		{
			HttpWebResponse response = Post(url, parameters, timeout, userAgent, requestEncoding, cookies);
			statusCode = (int)(response.StatusCode);
			StringBuilder strRet = new StringBuilder();
			if (response != null)
			{
				System.IO.Stream respStream = response.GetResponseStream();
				if (requestEncoding == null)
				{
					requestEncoding = Encoding.UTF8;
				}
				using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, requestEncoding))
				{
				    System.String oneline;
					while ((oneline = reader.ReadLine()) != null)
					{
						strRet.Append(oneline);
					}
				}
			}
			return strRet.ToString();
		}

		public static string PostData(string url, List<Tuple<string, string>> parameters, int timeout = 1000000, string userAgent = "", Encoding requestEncoding = null, HttpFileCollection fileList = null)
		{
			//StringBuilder sb = new StringBuilder();
			if (requestEncoding == null)
			{
				requestEncoding = Encoding.UTF8;
			}
			string responseContent;
			var memStream = new MemoryStream();
			var webRequest = (HttpWebRequest)WebRequest.Create(url);
			// 边界符
			var boundary = DateTime.Now.Ticks.ToString("x");
			var defaultBoundaryStr = "--" + boundary + "\r\n";
			var defaultBoundary = requestEncoding.GetBytes(defaultBoundaryStr);
			var lastBoundaryStr = "\r\n--" + boundary + "--\r\n";
			var lastBoundary = requestEncoding.GetBytes(lastBoundaryStr);

			// 设置属性
			webRequest.Method = "POST";
			webRequest.Proxy = null;
			webRequest.Timeout = timeout;
			string contenType = "multipart/form-data; boundary=" + boundary + "\r\n";
			webRequest.ContentType = contenType;
			//sb.Append(contenType);

			if (fileList != null && fileList.Count > 0)
			{
				for (int i = 0; i < fileList.Count; i++)
				{
					if (fileList[i].InputStream != null)
					{
						string filePartHeader = "\r\n" + defaultBoundaryStr + "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
						var header = string.Format(filePartHeader, "upload", fileList[i].FileName);
						var headerbytes = requestEncoding.GetBytes(header);
						//sb.Append(header);

						memStream.Write(headerbytes, 0, headerbytes.Length);

						var buffer = new byte[1024];
						int bytesRead; // =0

						while ((bytesRead = fileList[i].InputStream.Read(buffer, 0, buffer.Length)) != 0)
						{
							memStream.Write(buffer, 0, bytesRead);
						}
					}
				}
			}

			// 写入字符串的Key
			var stringKeyHeader = "\r\n" + defaultBoundaryStr + "Content-Disposition: form-data; name=\"{0}\"" + "\r\n\r\n{1}";

			foreach (byte[] formitembytes in from Tuple<string, string> t in parameters
											 select string.Format(stringKeyHeader, t.Item1, t.Item2)
												 into formitem
												 select requestEncoding.GetBytes(formitem))
			{
				memStream.Write(formitembytes, 0, formitembytes.Length);
				//sb.Append(requestEncoding.GetString(formitembytes));
			}

			// 写入最后的结束边界符
			memStream.Write(lastBoundary, 0, lastBoundary.Length);
			//sb.Append(requestEncoding.GetString(lastBoundary));

			webRequest.ContentLength = memStream.Length;

			var requestStream = webRequest.GetRequestStream();

			memStream.Position = 0;
			var tempBuffer = new byte[memStream.Length];
			memStream.Read(tempBuffer, 0, tempBuffer.Length);
			memStream.Close();

			requestStream.Write(tempBuffer, 0, tempBuffer.Length);
			requestStream.Close();

			var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

			using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), requestEncoding))
			{
				responseContent = httpStreamReader.ReadToEnd();
			}


			httpWebResponse.Close();
			webRequest.Abort();

			return responseContent;
		}


		public static string PostData(string url, IDictionary<string, string> parameters, int timeout = 1000000, string userAgent = "", Encoding requestEncoding = null, HttpFileCollection fileList = null)
		{
			//StringBuilder sb = new StringBuilder();
			if (requestEncoding == null)
			{
				requestEncoding = Encoding.UTF8;
			}
			string responseContent;
			var memStream = new MemoryStream();
			var webRequest = (HttpWebRequest)WebRequest.Create(url);
			// 边界符
			var boundary = DateTime.Now.Ticks.ToString("x");
			var defaultBoundaryStr = "--" + boundary + "\r\n";
			var defaultBoundary = requestEncoding.GetBytes(defaultBoundaryStr);
			var lastBoundaryStr = "\r\n--" + boundary + "--\r\n";
			var lastBoundary = requestEncoding.GetBytes(lastBoundaryStr);

			// 设置属性
			webRequest.Method = "POST";
			webRequest.Proxy = null;
			webRequest.Timeout = timeout;
			string contenType = "multipart/form-data; boundary=" + boundary + "\r\n";
			webRequest.ContentType = contenType;
			//sb.Append(contenType);

			if (fileList != null && fileList.Count > 0)
			{
				for (int i = 0; i < fileList.Count; i++)
				{
					if (fileList[i].InputStream != null)
					{
						string filePartHeader = "\r\n" + defaultBoundaryStr + "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
						var header = string.Format(filePartHeader, "upload", fileList[i].FileName);
						var headerbytes = requestEncoding.GetBytes(header);
						//sb.Append(header);

						memStream.Write(headerbytes, 0, headerbytes.Length);

						var buffer = new byte[1024];
						int bytesRead; // =0

						while ((bytesRead = fileList[i].InputStream.Read(buffer, 0, buffer.Length)) != 0)
						{
							memStream.Write(buffer, 0, bytesRead);
						}
					}
				}
			}

			// 写入字符串的Key
			var stringKeyHeader = "\r\n" + defaultBoundaryStr + "Content-Disposition: form-data; name=\"{0}\"" + "\r\n\r\n{1}";

			foreach (byte[] formitembytes in from string key in parameters.Keys
											 select string.Format(stringKeyHeader, key, parameters[key])
												 into formitem
												 select requestEncoding.GetBytes(formitem))
			{
				memStream.Write(formitembytes, 0, formitembytes.Length);
				//sb.Append(requestEncoding.GetString(formitembytes));
			}

			// 写入最后的结束边界符
			memStream.Write(lastBoundary, 0, lastBoundary.Length);
			//sb.Append(requestEncoding.GetString(lastBoundary));

			webRequest.ContentLength = memStream.Length;

			var requestStream = webRequest.GetRequestStream();

			memStream.Position = 0;
			var tempBuffer = new byte[memStream.Length];
			memStream.Read(tempBuffer, 0, tempBuffer.Length);
			memStream.Close();

			requestStream.Write(tempBuffer, 0, tempBuffer.Length);
			requestStream.Close();

			var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

			using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), requestEncoding))
			{
				responseContent = httpStreamReader.ReadToEnd();
			}


			httpWebResponse.Close();
			webRequest.Abort();

			return responseContent;
		}


		public static string PostData(string url, IDictionary<string, string> parameters, int timeout = 1000000, string userAgent = "", Encoding requestEncoding = null, string fileKeyName = "", string filePath = "", Stream stream = null)
		{
			if (requestEncoding == null)
			{
				requestEncoding = Encoding.UTF8;
			}
			string responseContent;
			var memStream = new MemoryStream();
			var webRequest = (HttpWebRequest)WebRequest.Create(url);
			// 边界符
			var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
			// 边界符
			var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
			//var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			// 最后的结束符
			var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

			// 设置属性
			webRequest.Method = "POST";
			webRequest.Proxy = null;
			webRequest.Timeout = timeout;
			webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

			if (stream != null)
			{
				// 写入文件
				const string filePartHeader =
					"Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
					 "Content-Type: application/octet-stream\r\n\r\n";
				var header = string.Format(filePartHeader, fileKeyName, filePath);
				var headerbytes = requestEncoding.GetBytes(header);

				memStream.Write(beginBoundary, 0, beginBoundary.Length);
				memStream.Write(headerbytes, 0, headerbytes.Length);

				var buffer = new byte[1024];
				int bytesRead; // =0

				while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
				{
					memStream.Write(buffer, 0, bytesRead);
				}
			}
			// 写入字符串的Key
			var stringKeyHeader = "\r\n--" + boundary +
								   "\r\nContent-Disposition: form-data; name=\"{0}\"" +
								   "\r\n\r\n{1}\r\n";

			foreach (byte[] formitembytes in from string key in parameters.Keys
											 select string.Format(stringKeyHeader, key, parameters[key])
												 into formitem
												 select requestEncoding.GetBytes(formitem))
			{
				memStream.Write(formitembytes, 0, formitembytes.Length);
			}

			// 写入最后的结束边界符
			memStream.Write(endBoundary, 0, endBoundary.Length);

			webRequest.ContentLength = memStream.Length;

			var requestStream = webRequest.GetRequestStream();

			memStream.Position = 0;
			var tempBuffer = new byte[memStream.Length];
			memStream.Read(tempBuffer, 0, tempBuffer.Length);
			memStream.Close();

			requestStream.Write(tempBuffer, 0, tempBuffer.Length);
			requestStream.Close();

			var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

			using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), requestEncoding))
			{
				responseContent = httpStreamReader.ReadToEnd();
			}

			if (stream != null) stream.Close();
			httpWebResponse.Close();
			webRequest.Abort();

			return responseContent;
		}



		private static bool checkValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true; //总是接受  
		}


		/// <summary>
		/// http:获取页面信息，失败返回string.Empty
		/// </summary>
		/// <param name="url"></param>
		/// <param name="encoding">可以为null，那么会用系统默认的encoding</param>
		/// <returns></returns>
		public static string GetDataByUrl(string url, System.Text.Encoding encoding, out string errorMessage)
		{
			return GetDataByUrl(url, "GET", encoding, out errorMessage);
		}

		/// <summary>
		/// 失败返回string.Empty
		/// </summary>
		/// <param name="url"></param>
		/// <param name="requestMethod">GET、POST、HEAD等</param>
		/// <param name="encoding">可以为null，那么会用系统默认的encoding</param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		public static string GetDataByUrl(string url, string requestMethod, System.Text.Encoding encoding, out string errorMessage)
		{
			errorMessage = string.Empty;
			string strReturn = string.Empty;

			StringBuilder strRet = new StringBuilder("");
			if (null == encoding)
			{
				encoding = System.Text.Encoding.Default;
			}

			try
			{
				if (url.ToLower().StartsWith("http"))
				{
					string t = DateTime.Now.ToString();
					System.Net.HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(url);
					wReq.Method = requestMethod;
					wReq.Timeout = 10 * 1000;
					wReq.UserAgent = DefaultUserAgent;
					wReq.Proxy = null;
					System.Net.WebResponse wResp = wReq.GetResponse();
					System.IO.Stream respStream = wResp.GetResponseStream();
					using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, encoding))
					{
						string oneline;
						while ((oneline = reader.ReadLine()) != null)
						{
							strRet.Append(oneline);
						}
					}
				}
				else
				{
					using (System.IO.StreamReader reader = new System.IO.StreamReader(url, encoding))
					{
						string oneline;
						while ((oneline = reader.ReadLine()) != null)
						{
							strRet.Append(oneline);
						}
					}
				}

				strReturn = strRet.ToString();

			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}
			return strReturn;
		}

		/// <summary>
		/// 异步post请求
		/// </summary>
		/// <param name="callback">回调函数</param>
		/// <param name="url">请求的URL</param>  
		/// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
		/// <param name="timeout">请求的超时时间</param>  
		/// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
		/// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
		/// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
		/// <returns></returns>
		public static bool PostAsync(AsyncCallback callback, string url, IDictionary<string, string> parameters, int? timeout, string userAgent, string authorization, Encoding requestEncoding, CookieCollection cookies)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentNullException("url");
			}
			if (requestEncoding == null)
			{
				requestEncoding = Encoding.UTF8;
				//throw new ArgumentNullException("requestEncoding");
			}
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			//如果是发送HTTPS请求  
			if (true == url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
			{
				ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(checkValidationResult);
				request = WebRequest.Create(url) as HttpWebRequest;
				request.ProtocolVersion = HttpVersion.Version10;
			}
			request.Method = "POST";
			request.Proxy = null;
			request.ContentType = "application/x-www-form-urlencoded";
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
			request.Headers["Accept-Language"] = "zh-CN,zh;q=0.";
			request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
			request.AllowAutoRedirect = false;
			request.KeepAlive = true;

			request.UserAgent = string.IsNullOrEmpty(userAgent) ? DefaultUserAgent : userAgent;
			request.Headers.Add(HttpRequestHeader.Authorization, authorization);

			if (timeout.HasValue)
			{
				request.Timeout = timeout.Value;
			}
			if (cookies != null)
			{
				request.CookieContainer = new CookieContainer();
				request.CookieContainer.Add(cookies);
			}
			//如果需要POST数据  
			if (!(parameters == null || parameters.Count == 0))
			{
				StringBuilder buffer = new StringBuilder();
				int i = 0;
				foreach (string key in parameters.Keys)
				{
					if (i > 0)
					{
						buffer.AppendFormat("&{0}={1}", key, parameters[key]);
					}
					else
					{
						buffer.AppendFormat("{0}={1}", key, parameters[key]);
					}
					i++;
				}
				byte[] data = requestEncoding.GetBytes(buffer.ToString());
				request.ContentLength = data.Length;
				using (Stream stream = request.GetRequestStream())
				{
					stream.Write(data, 0, data.Length);
				}
			}
			try
			{
				request.BeginGetResponse(callback, request);

				return true;
			}
			catch (WebException e)
			{
				if (e.Status == WebExceptionStatus.ProtocolError)
				{
					//return e.Response as HttpWebResponse;
					return false;
				}
				else
				{
					return false;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// 从full里面，取出介于start和end之间的文本。失败返回null
		/// </summary>
		/// <param name="html"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static string GetText(string full, string start, string end)
		{
			int idx1 = full.IndexOf(start);
			if (idx1 < 0)
			{
				return null;
			}
			int idx2 = full.IndexOf(end, idx1 + 1);
			if (idx2 < 0)
			{
				return null;
			}

			string sub = full.Substring(idx1 + start.Length, idx2 - idx1 - start.Length);
			return sub;
		}


        /// <summary>
        /// Http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据</returns>
        public static string HttpGet(string url)
        {
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (Exception e)
            {
                
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

	}
}
