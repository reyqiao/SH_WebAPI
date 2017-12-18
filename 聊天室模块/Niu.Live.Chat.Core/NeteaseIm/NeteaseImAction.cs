using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Chat.Core.NeteaseIm
{
    /// <summary>
    /// http://172.17.5.8/hqy/live/#g=1&p=21视频_聊天
    /// http://dev.netease.im/docs?doc=server
    /// </summary>
    internal class NeteaseImAction
    {
        private static readonly string NeteaseImDomainUrl = "https://api.netease.im";


        #region 云信ID
        //http://dev.netease.im/docs?doc=server&#云信ID


        /// <summary>
        /// 创建云信ID
        /// 
        /// 接口描述
        /// 1.第三方帐号导入到云信平台；
        /// 2.注意accid，name长度以及考虑管理token。
        /// </summary>
        public static readonly string NeteaseImUserCreate = NeteaseImDomainUrl + "/nimserver/user/create.action";

        /// <summary>
        /// 云信ID更新
        /// 
        /// 接口描述
        /// 云信ID基本信息更新
        /// </summary>
        public static readonly string NeteaseImUserUpdate = NeteaseImDomainUrl + "/nimserver/user/update.action";

        /// <summary>
        /// 更新并获取新token
        /// 
        /// 接口描述
        /// 1.webserver更新云信ID的token，同时返回新的token；
        /// 2.一般用于云信ID修改密码，找回密码或者第三方有需求获取新的token。
        /// </summary>
        public static readonly string NeteaseImUserRefreshToken = NeteaseImDomainUrl + "/nimserver/user/refreshToken.action";

        /// <summary>
        /// 封禁云信ID
        ///
        /// 接口描述
        /// 1.第三方禁用某个云信ID的IM功能；
        /// 2.封禁云信ID后，此ID将不能登陆云信imserver。
        /// </summary>
        public static readonly string NeteaseImUserBlock = NeteaseImDomainUrl + "/nimserver/user/block.action";

        /// <summary>
        /// 解禁云信ID
        ///
        /// 接口描述
        /// 解禁被封禁的云信ID
        /// </summary>
        public static readonly string NeteaseImUserUnblock = NeteaseImDomainUrl + "/nimserver/user/unblock.action";

        #endregion

        #region 用户名片

        /// <summary>
        /// 更新用户名片
        /// 
        /// 接口描述
        /// 更新用户名片
        /// </summary>
        public static readonly string NeteaseImUserUpdateUinfo = NeteaseImDomainUrl + "/nimserver/user/updateUinfo.action";

        /// <summary>
        /// 获取用户名片
        /// 
        /// 接口描述
        /// 获取用户名片，可批量
        /// </summary>
        public static readonly string NeteaseImUserGetUinfos = NeteaseImDomainUrl + "/nimserver/user/getUinfos.action";

        #endregion

        #region 聊天室
        //http://dev.netease.im/docs?doc=server&#聊天室

        /// <summary>
        /// 创建聊天室
        /// 
        /// 接口描述
        /// 创建聊天室
        /// </summary>
        public static readonly string NeteaseImChatroomCreate = NeteaseImDomainUrl + "/nimserver/chatroom/create.action";
        
        /// <summary>
        /// 查询聊天室信息
        /// 
        /// 接口描述
        /// 查询聊天室信息
        /// </summary>
        public static readonly string NeteaseImChatroomGet = NeteaseImDomainUrl + "/nimserver/chatroom/get.action";
        
        /// <summary>
        /// 更新聊天室信息
        /// 
        /// 接口描述
        /// 更新聊天室信息
        /// </summary>
        public static readonly string NeteaseImChatroomUpdate = NeteaseImDomainUrl + "/nimserver/chatroom/update.action";

        /// <summary>
        /// 修改聊天室开/关闭状态
        /// 
        /// 接口描述
        /// 修改聊天室开/关闭状态
        /// </summary>
        public static readonly string NeteaseImChatroomToggleCloseStat = NeteaseImDomainUrl + "/nimserver/chatroom/toggleCloseStat.action";

        /// <summary>
        /// 设置聊天室内用户角色
        /// 
        /// 接口描述
        /// 设置聊天室内用户角色
        /// </summary>
        public static readonly string NeteaseImChatroomSetMemberRole = NeteaseImDomainUrl + "/nimserver/chatroom/setMemberRole.action";

        /// <summary>
        /// 请求聊天室地址
        /// 
        /// 接口描述
        /// 请求聊天室地址与令牌
        /// </summary>
        public static readonly string NeteaseImChatroomRequestAddr = NeteaseImDomainUrl + "/nimserver/chatroom/requestAddr.action";

        /// <summary>
        /// 发送聊天室消息
        /// 
        /// 接口描述
        /// 往聊天室内发消息
        /// </summary>
        public static readonly string NeteaseImChatroomSendMsg = NeteaseImDomainUrl + "/nimserver/chatroom/sendMsg.action";

        /// <summary>
        /// 往聊天室内添加机器人
        /// 
        /// 接口描述
        /// 往聊天室内添加机器人
        /// </summary>
        public static readonly string NeteaseImChatroomAddRobot = NeteaseImDomainUrl + "/nimserver/chatroom/addRobot.action";

        /// <summary>
        /// 从聊天室内删除机器人
        /// 
        /// 接口描述
        /// 从聊天室内删除机器人
        /// </summary>
        public static readonly string NeteaseImChatroomRemoveRobot = NeteaseImDomainUrl + "/nimserver/chatroom/removeRobot.action";

        /// <summary>
        /// 设置临时禁言状态
        /// 
        /// 接口描述
        /// 将聊天室内成员设置为临时禁言
        /// </summary>
        public static readonly string NeteaseImChatroomTemporaryMute = NeteaseImDomainUrl + "/nimserver/chatroom/temporaryMute.action";

        /// <summary>
        /// 设置禁言
        /// 
        /// 接口描述
        /// 设置聊天室整体禁言状态（仅创建者和管理员能发言）
        /// </summary>
        public static readonly string NeteaseImChatroomMute = NeteaseImDomainUrl + "/nimserver/chatroom/muteRoom.action";
        /// <summary>
        /// 往聊天室有序队列中新加或更新元素
        /// 
        /// 接口描述
        /// 往聊天室有序队列中新加或更新元素
        /// </summary>
        public static readonly string NeteaseImChatroomQueueOffer = NeteaseImDomainUrl + "/nimserver/chatroom/queueOffer.action";

        /// <summary>
        /// 从队列中取出元素
        /// 
        /// 接口描述
        /// 从队列中取出元素
        /// </summary>
        public static readonly string NeteaseImChatroomQueuePoll = NeteaseImDomainUrl + "/nimserver/chatroom/queuePoll.action";

        /// <summary>
        /// 排序列出队列中所有元素
        /// 
        /// 接口描述
        /// 排序列出队列中所有元素
        /// </summary>
        public static readonly string NeteaseImChatroomQueueList = NeteaseImDomainUrl + "/nimserver/chatroom/queueList.action";

        /// <summary>
        /// 删除清理整个队列
        /// 
        /// 接口描述
        /// 删除清理整个队列
        /// </summary>
        public static readonly string NeteaseImChatroomQueueDrop = NeteaseImDomainUrl + "/nimserver/chatroom/queueDrop.action";

        /// <summary>
        /// 初始化队列
        /// 
        /// 接口描述
        /// 初始化队列
        /// </summary>
        public static readonly string NeteaseImChatroomQueueInit = NeteaseImDomainUrl + "/nimserver/chatroom/queueInit.action";

        #endregion



    }
}