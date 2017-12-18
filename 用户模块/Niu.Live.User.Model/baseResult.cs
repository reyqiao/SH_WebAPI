using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Niu.Live.User.Model
{
    /// <summary>
    /// 返回实体
    /// </summary>
    [DataContract]
    public class BaseResult
    {
        #region 构造函数

        /// <summary>
        /// 构造器
        /// </summary>
        public BaseResult()
        {
            result = 0;
            code = -1;
            message = string.Empty;
        }

        public BaseResult(int result, int code) : this(result, code, string.Empty) { }

        public BaseResult(int result, int code, string message)
        {
            this.result = result;
            this.code = code;
            this.message = message;
        }

        #endregion

        /// <summary>
        /// 返回状态值 0假 1真
        /// </summary>
        [DataMember]
        public int result { get; set; }
        /// <summary>
        /// 代码 小于0：异常代码编号
        /// </summary>
        [DataMember]
        public int code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        public string message { get; set; }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
