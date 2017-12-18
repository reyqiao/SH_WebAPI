using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.IModel.UserInfo
{
    /// <summary>
    /// 用户实体
    /// </summary>
    public class UserInfo:IUserInfo
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 渠道id
        /// </summary>
        public int channelId { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 类型：0 Pc  1 Wap
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 状态： -1 禁用 0 正常
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime addTime { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string logoPhotoUrl { get; set; }
    }
}
