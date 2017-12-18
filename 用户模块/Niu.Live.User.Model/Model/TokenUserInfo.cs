using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Niu.Live.User.Model
{
    /// <summary>
    /// userToken 实体
    /// </summary>
    public class TokenUserInfo
    {
        /// <summary>
        /// token 类型
        /// </summary>
        public tokenType tokenType { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long userId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string nickName { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public userType type { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 渠道ID
        /// </summary>
        public int channelId { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public int roleId { get; set; }
        /// <summary>
        /// 角色名字
        /// </summary>
        public string roleName { get; set; }
        /// <summary>
        /// 是否管理员
        /// </summary>
        public int isManage { get; set; }

        /// <summary>
        /// 重写 tostring 方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// 用户类型
    /// </summary>
    public enum userType
    {
        /// <summary>
        /// pc端用户
        /// </summary>
        pc = 0,
        /// <summary>
        /// wap端用户
        /// </summary>
        wap = 1
    }

    /// <summary>
    /// token 类型 0 匿名用户  1 正式用户  
    /// </summary>
    public enum tokenType
    {
        /// <summary>
        /// 匿名用户
        /// </summary>
        Anonymous = 0,
        /// <summary>
        /// 正式用户
        /// </summary>
        Formal = 1
    }
}
