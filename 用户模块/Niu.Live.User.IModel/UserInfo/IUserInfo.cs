using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.IModel.UserInfo
{
    /// <summary>
    /// 用户接口
    /// </summary>
    interface IUserInfo
    {
        /// <summary>
        /// 用户id
        /// </summary>
        long userId { get; set; }
        /// <summary>
        /// 渠道id
        /// </summary>
        int channelId { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        string nickName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        string mobile { get; set; }
        /// <summary>
        /// 类型：0 Pc  1 Wap
        /// </summary>
        int type { get; set; }
        /// <summary>
        /// 状态： -1 禁用 0 正常
        /// </summary>
        int status { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        DateTime addTime { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        string logoPhotoUrl { get; set; }
    }
}
