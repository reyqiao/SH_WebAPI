using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.Model
{
    /// <summary>
    /// 基本返回信息
    /// </summary>
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
        }

        /// <summary>
        /// 返回状态值 0假 1真
        /// </summary>
        public int result { get; set; }

        /// <summary>
        /// 代码 小于0：异常代码编号 (-10：登录失败)
        /// </summary>
        public int code { get; set; }


        /// <summary>
        /// 消息
        /// </summary>
        public virtual string message { get; set; }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
       // public override string ToString()
        //{
           // return  JSONHelper.Serialize<BaseResult>(this);
       // }

    }
}
