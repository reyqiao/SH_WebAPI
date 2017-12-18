using Niugu.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 基本返回信息
    /// </summary>
    [DataContract]
    public class BaseResult
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public BaseResult()
        {
            result = 0;
            code = -1;
            message = string.Empty;
            method = string.Empty;
        }
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
        [DataMember]
        public string method { get; set; }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JSONHelper.Serialize<BaseResult>(this);
        }
    }// End public class BaseResult.
}// End namespace Niugu.User.Model.

