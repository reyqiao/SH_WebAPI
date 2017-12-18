using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Niugu.Common
{
    /// <summary>
    /// 文本验证
    /// </summary>
    public class Filter
    {
		private static string filterUrl = "http://filter.niuguwang:8089/";
		//http://192.168.1.231:8089/?message=国家
		//http://192.168.2.77:8089/?message=国家
		//http://filter.niuguwang:8089/?message=本人是学生，对于股票有点感兴趣，所以来尝试一下，请多多关照&sign=1
		//http://192.168.2.112:8089/?message=本人是学生，对于股票有点感兴趣，所以来尝试一下，请多多关照&sign=1
		/// <summary>
		/// 验证文字是否包含审核词
		/// </summary>
		/// <param name="content"></param>
		/// <returns>0验证通过，-1不通过</returns>
        public static int CheckContent(string content)
        {
            int result = 0;
            string errorMessage = string.Empty;
            string resultText = string.Empty;
            FilterResult resultObject = new FilterResult();

            //try
            //{
            //    Dictionary<string, string> parameters = new Dictionary<string,string>();
            //    parameters.Add("message", HttpUtility.UrlEncode(content));
            //    resultText = HttpWebResponseUtility.PostData(filterUrl, parameters);
            //    resultObject =   JSONHelper.Deserialize<FilterResult>(resultText);
            //    result = resultObject.flag;
            //}
            //catch (Exception ex)
            //{
            //    LogRecord.writeLogsingle("Filter.log", ex.Message);
            //}
            return result;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sign">sign = 1,返回查找到的10个审核词</param>
		/// <param name="filterWordArray">返回审核词数组</param>
		/// <returns></returns>
		public static int CheckContent(string content, int sign, out string[] filterWordArray)
		{
			sign = sign == 0 ? 0 : 1;
			int result = 0;
			string errorMessage = string.Empty;
			string resultText = string.Empty;
			filterWordArray = null;
			FilterResult resultObject = new FilterResult();

            //try
            //{
            //    Dictionary<string, string> parameters = new Dictionary<string, string>();
            //    parameters.Add("message", HttpUtility.UrlEncode(content));
            //    parameters.Add("sign", sign.ToString());
            //    resultText = HttpWebResponseUtility.PostData(filterUrl, parameters);
            //    resultObject = JSONHelper.Deserialize<FilterResult>(resultText);
            //    result = resultObject.flag;
            //    filterWordArray = resultObject.text.Split(new char[] { ',' });
            //}
            //catch (Exception ex)
            //{
            //    LogRecord.writeLogsingle("Filter.log", ex.Message);
            //}
			return result;
		}

    }// End class Filter.

    public class FilterResult
    {
        //http://192.168.1.231:8089/
        //{"text": "\u52a0\u5fae\u6c34", "flag": -1}
        /// <summary>
        /// 构造器
        /// </summary>
        public FilterResult()
        {
            flag = 0;
            text = string.Empty;
            return;
        }
        
        public int flag { get; set; }
        public string text { get; set; }

        //public override string ToString()
        //{
        //    return 
        //    return Niugu.Common.JSONHelper.Serialize<FilterResult>(this);
        //}
    }// End class MaketDayDataReturnInfo.
}// End namespace Niugu.Common.
