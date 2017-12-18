using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Chat.Core.Provider
{
    public enum ChatroomMessageType
    {
        /// <summary>
        /// 0: 表示文本消息
        /// </summary>
        Text = 0,
        /// <summary>
        /// 1: 表示图片
        /// </summary>
        Image = 1,
        /// <summary>
        /// 2: 表示语音
        /// </summary>
        Audio = 2,
        /// <summary>
        /// 3: 表示视频
        /// </summary>
        Video = 3,
        /// <summary>
        /// 4: 表示地理位置信息
        /// </summary>
        Position = 4,
        /// <summary>
        /// 6: 表示文件
        /// </summary>
        File = 6,
        /// <summary>
        /// 10: 表示Tips消息
        /// </summary>
        Tips = 10,
        /// <summary>
        /// 100: 自定义消息类型
        /// </summary>
        Custom = 100

    }
}