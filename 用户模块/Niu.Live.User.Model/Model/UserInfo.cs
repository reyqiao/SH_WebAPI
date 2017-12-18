using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.Model
{
    /// <summary>
    /// 用户实体表
    /// </summary>
    public class UserInfo : IUserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long  userId  { get ; set; }
        private int _channelId = 1;
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int  channelId  
        { 
           get {
               return _channelId;
           } 
           set{
              _channelId = value;
           }
        }
        /// <summary>
        /// 昵称
        /// </summary>
        public string  nickName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string  mobile   { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public  string password { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int  type { get; set; }
        /// <summary>
        /// T头像ID
        /// </summary>
        public long logoPhotoId { get; set;}
        /// <summary>
        /// 头像url
        /// </summary>
        public  string logoPhotoUrl { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int gender { get ;set; }
        /// <summary>
        /// 状态 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime lastVisitTime { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime addTime { get ; set; }
        /// <summary>
        /// 密码等级
        /// </summary>
        public int pwdSecurityLevel { get; set; }
        /// <summary>
        /// rcode
        /// </summary>
        public string rcode { get; set; }
        /// <summary>
        /// securityCode
        /// </summary>
        public string securityCode { get; set; }
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
