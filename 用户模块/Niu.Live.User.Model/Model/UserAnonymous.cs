using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.Model
{
    /// <summary>
    /// 用户匿名实体
    /// </summary>
    public class UserAnonymous : IUserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int channelId { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime addTime { get; set; }
        /// <summary>
        /// 令牌类型
        /// </summary>
        public tokenType tokenType { get; set; }
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
    }
}
