using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Niu.Live.User.Core.Json
{
    /// <summary>
    /// JSON帮助类
    /// </summary>
    public class JSONHelper
    {
        public static DataContractJsonSerializerSettings GetSettings(Type[] knownTypes)
        {
            return GetSettings(knownTypes, "yyyy-MM-dd HH:mm:ss");
        }
        public static DataContractJsonSerializerSettings GetSettings(Type[] knownTypes, string dateTimeFormat)
        {
            if (string.IsNullOrEmpty(dateTimeFormat)) dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            DataContractJsonSerializerSettings setting = new DataContractJsonSerializerSettings();
            setting.DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat(dateTimeFormat);
            setting.KnownTypes = knownTypes;
            return setting;
        }

        public static string Serialize<T>(T objectToSerialize, DataContractJsonSerializerSettings settings)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), settings);
                serializer.WriteObject(ms, objectToSerialize);
                ms.Position = 0;
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        public static T Deserialize<T>(string jsonString, DataContractJsonSerializerSettings settings)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), settings);
                return (T)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// 序列化 对象转化为Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string Serialize<T>(T objectToSerialize)
        {
            DataContractJsonSerializerSettings setting = new DataContractJsonSerializerSettings();
            setting.DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat("yyyy-MM-dd HH:mm:ss");
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), setting);
                serializer.WriteObject(ms, objectToSerialize);
                ms.Position = 0;
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// 替换Json的Date字符串
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string ConvertDateTime(string jsonStr)
        {
            string result = string.Empty;
            //替换Json的Date字符串
            string p = @"\\/Date\(((-\d+)|(\d+))(\+|\-)\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            result = reg.Replace(jsonStr, matchEvaluator);
            return result;
        }
        /// <summary>    
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串    
        /// \/Date(-62135539200000+0800)\/
        /// </summary>    
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        } 
        /// <summary>
        /// 反序列化 Json字符串转化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }

        public static string ObjectToJSON(object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                return jss.Serialize(obj);
            }
            catch (Exception ex)
            {
				string ss = ex.ToString();
                return string.Empty;
            }
        }

        public static object DeserializeObject(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.DeserializeObject(input);
        }

		public static Dictionary<string, object> ParseJson(string input)
		{
			Dictionary<string, object> sData = JSONHelper.DeserializeObject(input) as Dictionary<string, object>;
			return sData;
		}

    }
}
