using System;
namespace Niu.Live.User.Model
{
    /// <summary>
    /// 用户接口
    /// </summary>
    public interface IUserInfo
    {
        /// <summary>
        /// 昵称
        /// </summary>
        string nickName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        int status { get; set; }
        /// <summary>
        /// 令牌类型
        /// </summary>
        tokenType tokenType { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        int type { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        long userId { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        int channelId { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        int roleId { get; set; }
        /// <summary>
        /// 角色名字
        /// </summary>
        string roleName { get; set; }
        /// <summary>
        /// 是否管理员
        /// </summary>
        int isManage { get; set; }
    }
}
