using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.Model
{
    /// <summary>
    /// 短信实体类
    /// </summary>
    public class VerificationCode
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public long mobile { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string verificationCode { get; set; }
        /// <summary>
        /// 是否已验证
        /// </summary>
        public int isVerification { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }
        /// <summary>
        /// 发送次数
        /// </summary>
        public int sendNumber { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime addTime { get; set; }
        /// <summary>
        /// 用户IP
        /// </summary>
        public string userIP { get; set; }
        /// <summary>
        /// 接口类型
        /// </summary>
        public int interfaceType { get; set; }
        /// <summary>
        /// 检查次数
        /// </summary>
        public int checkTime { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int channelId { get; set; }
    }
}
