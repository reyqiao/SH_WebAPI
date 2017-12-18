using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.LiveRoom.Utils
{
    public class EnumManager
    {
        /// <summary>
        /// 图片类型
        /// </summary>
        public enum PhotoType
        {
            /// <summary>
            /// 未知
            /// </summary>
            None = 0,
            /// <summary>
            /// 网站logo
            /// </summary>
            SitLogo = 1,
            /// <summary>
            /// 直播模块
            /// </summary>
            LiveModule = 3,
            /// <summary>
            /// 页面图片
            /// </summary>
            PagePhoto = 4,

            NewsPhoto =5
        }
    }
}
