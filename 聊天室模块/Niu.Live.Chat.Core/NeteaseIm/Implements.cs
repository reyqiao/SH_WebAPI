using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Chat.Core.NeteaseIm
{
    public class Implements
    {


        #region 云信ID

        /// <summary>
        /// 创建云信ID
        /// 
        /// 接口描述
        /// 1.第三方帐号导入到云信平台；
        /// 2.注意accid，name长度以及考虑管理token。
        /// 
        /// http://dev.netease.im/docs?doc=server&#创建云信ID
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="name"></param>
        /// <param name="icon"></param>
        /// <param name="props"></param>
        /// <param name="token"></param>
        public static string User_Create(string accId, string name, string icon, string props = "", string token = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "accid", accId }, { "name", name }, { "icon", icon } };
            if (!string.IsNullOrEmpty(token)) parameters.Add("token", token);
            if (!string.IsNullOrEmpty(token)) parameters.Add("props", props);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImUserCreate, parameters);
            return json;
        }

        /// <summary>
        /// 云信ID更新
        /// 
        /// 接口描述
        /// 云信ID基本信息更新
        /// 
        /// http://dev.netease.im/docs?doc=server&#云信ID更新
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="props"></param>
        /// <param name="token"></param>
        public static string User_Update(string accId, string props, string token = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "accid", accId } };
            if (!string.IsNullOrEmpty(token)) parameters.Add("token", token);
            if (!string.IsNullOrEmpty(token)) parameters.Add("props", props);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImUserUpdate, parameters);
            return json;
        }


        /// <summary>
        /// 更新并获取新token
        /// 
        /// 接口描述
        /// 1.webserver更新云信ID的token，同时返回新的token；
        /// 2.一般用于云信ID修改密码，找回密码或者第三方有需求获取新的token。
        /// 
        /// http://dev.netease.im/docs?doc=server&#更新并获取新token
        /// </summary>
        /// <param name="accId"></param>
        public static string User_RefreshToken(string accId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "accid", accId } };
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImUserRefreshToken, parameters);
            return json;
        }

        /// <summary>
        /// 封禁云信ID
        ///
        /// 接口描述
        /// 1.第三方禁用某个云信ID的IM功能；
        /// 2.封禁云信ID后，此ID将不能登陆云信imserver。
        /// 
        /// http://dev.netease.im/docs?doc=server&#封禁云信ID
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="needkick">是否踢掉被禁用户，true或false，默认false</param>
        public static string User_Block(string accId, string needkick = "false")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "accid", accId } };
            if (!string.IsNullOrEmpty(needkick)) parameters.Add("needkick", needkick);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImUserBlock, parameters);
            return json;
        }

        /// <summary>
        /// 解禁云信ID
        ///
        /// 接口描述
        /// 解禁被封禁的云信ID
        /// 
        /// http://dev.netease.im/docs?doc=server&#解禁云信ID
        /// </summary>
        /// <param name="accId"></param>
        public static string User_Unblock(string accId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "accid", accId } };
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImUserUnblock, parameters);
            return json;
        }

        #endregion


        #region 用户名片


        /// <summary>
        /// 更新用户名片
        /// 
        /// 接口描述
        /// 更新用户名片
        /// 
        /// http://dev.netease.im/docs?doc=server&#更新用户名片
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="props"></param>
        /// <param name="token"></param>
        public static string User_UpdateUserInfo(string accId, string name = "", string icon = "", string sign = "", string email = "", string birth = "", string mobile = "", string gender = "", string ex = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "accid", accId } };
            if (!string.IsNullOrEmpty(name)) parameters.Add("name", name);
            if (!string.IsNullOrEmpty(icon)) parameters.Add("icon", icon);
            if (!string.IsNullOrEmpty(sign)) parameters.Add("sign", sign);
            if (!string.IsNullOrEmpty(email)) parameters.Add("email", email);
            if (!string.IsNullOrEmpty(birth)) parameters.Add("birth", birth);
            if (!string.IsNullOrEmpty(mobile)) parameters.Add("mobile", mobile);
            if (!string.IsNullOrEmpty(gender)) parameters.Add("gender", gender);
            if (!string.IsNullOrEmpty(ex)) parameters.Add("ex", ex);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImUserUpdateUinfo, parameters);
            return json;
        }

        /// <summary>
        /// 获取用户名片
        /// 
        /// 接口描述
        /// 获取用户名片，可批量
        /// 
        /// http://dev.netease.im/docs?doc=server&#获取用户名片
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="props"></param>
        /// <param name="token"></param>
        public static string User_GetUserInfos(params string[] accids)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (accids != null && accids.Length > 0)
            {
                List<string> list = accids.Select(t => "\"" + t + "\"").ToList();
                parameters.Add("accids", string.Format("[{0}]", string.Join(",", list)));
            }

            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImUserGetUinfos, parameters);
            return json;
        }

        #endregion


        #region 聊天室



        /// <summary>
        /// 创建聊天室
        /// 
        /// 接口描述
        /// 创建聊天室
        /// 
        /// http://dev.netease.im/docs?doc=server&#创建聊天室
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="name"></param>
        /// <param name="announcement"></param>
        /// <param name="broadcasturl"></param>
        /// <param name="ext"></param>
        public static string Chatroom_Create(string creator, string name, string announcement = "", string broadcasturl = "", string ext = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "creator", creator }, { "name", name } };
            if (!string.IsNullOrEmpty(announcement)) parameters.Add("announcement", announcement);
            if (!string.IsNullOrEmpty(broadcasturl)) parameters.Add("broadcasturl", broadcasturl);
            if (!string.IsNullOrEmpty(ext)) parameters.Add("ext", ext);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomCreate, parameters);
            return json;
        }

        /// <summary>
        /// 查询聊天室信息
        /// 
        /// 接口描述
        /// 查询聊天室信息
        /// 
        /// http://dev.netease.im/docs?doc=server&#查询聊天室信息
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="needOnlineUserCount"></param>
        public static string Chatroom_Get(long roomId, string needOnlineUserCount = "false")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() } };
            if (!string.IsNullOrEmpty(needOnlineUserCount)) parameters.Add("needOnlineUserCount", needOnlineUserCount);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomGet, parameters);
            return json;
        }

        /// <summary>
        /// 更新聊天室信息
        /// 
        /// 接口描述
        /// 更新聊天室信息
        /// 
        /// http://dev.netease.im/docs?doc=server&#更新聊天室信息
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="name"></param>
        /// <param name="announcement"></param>
        /// <param name="broadcasturl"></param>
        /// <param name="ext"></param>
        /// <param name="needNotify"></param>
        /// <param name="notifyExt"></param>
        public static string Chatroom_Update(long roomId, string name, string announcement = "", string broadcasturl = "", string ext = "", string needNotify = "", string notifyExt = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() } };
            if (!string.IsNullOrEmpty(name)) parameters.Add("name", name);
            parameters.Add("announcement", announcement);
            parameters.Add("broadcasturl", broadcasturl);
            if (!string.IsNullOrEmpty(ext)) parameters.Add("ext", ext);
            if (!string.IsNullOrEmpty(needNotify)) parameters.Add("needNotify", needNotify);
            if (!string.IsNullOrEmpty(notifyExt)) parameters.Add("notifyExt", notifyExt);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomUpdate, parameters);
            return json;
        }

        /// <summary>
        /// 修改聊天室开/关闭状态
        /// 
        /// 接口描述
        /// 修改聊天室开/关闭状态
        /// 
        /// http://dev.netease.im/docs?doc=server&#修改聊天室开/关闭状态
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="operatorAccId"></param>
        /// <param name="valid"></param>
        public static string Chatroom_ToggleCloseStat(long roomId, string operatorAccId, string valid = "true")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "operator", operatorAccId }, { "valid", valid } };
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomToggleCloseStat, parameters);
            return json;
        }

        /// <summary>
        /// 设置聊天室内用户角色
        /// 
        /// 接口描述
        /// 设置聊天室内用户角色
        /// 
        /// http://dev.netease.im/docs?doc=server&#设置聊天室内用户角色
        /// </summary>
        public static string Chatroom_SetMemberRole(long roomId, string operatorAccId, string target, string opt, string optvalue, string notifyExt = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "operator", operatorAccId }, { "target", target }, { "opt", opt }, { "optvalue", optvalue } };
            if (!string.IsNullOrEmpty(notifyExt)) parameters.Add("notifyExt", notifyExt);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomSetMemberRole, parameters);
            return json;
        }

        /// <summary>
        /// 请求聊天室地址
        /// 
        /// 接口描述
        /// 请求聊天室地址与令牌
        /// 
        /// http://dev.netease.im/docs?doc=server&#请求聊天室地址
        /// </summary>
        public static string Chatroom_RequestAddr(long roomId, string accId, string clienttype = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "accid", accId }, { "clienttype", clienttype } };
            if (!string.IsNullOrEmpty(clienttype)) parameters.Add("clienttype", clienttype);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomRequestAddr, parameters);
            return json;
        }

        /// <summary>
        /// 发送聊天室消息
        /// 
        /// 接口描述
        /// 往聊天室内发消息
        /// 
        /// http://dev.netease.im/docs?doc=server&#发送聊天室消息
        /// </summary>
        public static string Chatroom_SendMsg(long roomId, string msgId, string fromAccId, int msgType, int resendFlag = 0, string attach = "", string ext = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "msgId", msgId }, { "fromAccid", fromAccId }, { "msgType", msgType.ToString() } };
            if (resendFlag != 0) parameters.Add("resendFlag", resendFlag.ToString());
            if (!string.IsNullOrEmpty(attach)) parameters.Add("attach", attach);
            if (!string.IsNullOrEmpty(ext)) parameters.Add("ext", ext);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomSendMsg, parameters);
            return json;
        }

        /// <summary>
        /// 往聊天室内添加机器人
        /// 
        /// 接口描述
        /// 往聊天室内添加机器人
        /// 
        /// http://dev.netease.im/docs?doc=server&#往聊天室内添加机器人
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="accIds"></param>
        /// <param name="roleExt"></param>
        /// <param name="notifyExt"></param>
        public static string Chatroom_AddRobot(long roomId, string accIds, string roleExt = "", string notifyExt = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "accIds", accIds } };
            if (!string.IsNullOrEmpty(roleExt)) parameters.Add("roleExt", roleExt);
            if (!string.IsNullOrEmpty(notifyExt)) parameters.Add("notifyExt", notifyExt);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomAddRobot, parameters);
            return json;
        }

        /// <summary>
        /// 从聊天室内删除机器人
        /// 
        /// 接口描述
        /// 从聊天室内删除机器人
        /// 
        /// http://dev.netease.im/docs?doc=server&#从聊天室内删除机器人
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="accIds"></param>
        public static string Chatroom_RemoveRobot(long roomId, string accIds)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "accIds", accIds } };
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomRemoveRobot, parameters);
            return json;
        }

        /// <summary>
        /// 设置临时禁言状态
        /// 
        /// 接口描述
        /// 将聊天室内成员设置为临时禁言
        /// 
        /// http://dev.netease.im/docs?doc=server&#设置临时禁言状态
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="operatorAccId"></param>
        /// <param name="target"></param>
        /// <param name="muteDuration"></param>
        /// <param name="needNotify"></param>
        /// <param name="notifyExt"></param>
        public static string Chatroom_TemporaryMute(long roomId, string operatorAccId, string target, long muteDuration, string needNotify = "", string notifyExt = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "operator", operatorAccId }, { "target", target }, { "muteDuration", muteDuration.ToString() } };
            if (!string.IsNullOrEmpty(needNotify)) parameters.Add("needNotify", needNotify);
            if (!string.IsNullOrEmpty(notifyExt)) parameters.Add("notifyExt", notifyExt);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomTemporaryMute, parameters);
            return json;
        }
        public static string Chatroom_Mute(long roomId, string operatorAccId, string mute = "false", string needNotify = "", string notifyExt = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "operator", operatorAccId }, { "mute", mute } };
            if (!string.IsNullOrEmpty(needNotify)) parameters.Add("needNotify", needNotify);
            if (!string.IsNullOrEmpty(notifyExt)) parameters.Add("notifyExt", notifyExt);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomMute, parameters);
            return json;
        }

        /// <summary>
        /// 往聊天室有序队列中新加或更新元素
        /// 
        /// 接口描述
        /// 往聊天室有序队列中新加或更新元素
        /// 
        /// http://dev.netease.im/docs?doc=server&#往聊天室有序队列中新加或更新元素
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static string Chatroom_QueueOffer(long roomId, string key, string value)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "key", key }, { "value", value } };
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomQueueOffer, parameters);
            return json;
        }

        /// <summary>
        /// 从队列中取出元素
        /// 
        /// 接口描述
        /// 从队列中取出元素
        /// 
        /// http://dev.netease.im/docs?doc=server&#从队列中取出元素
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="key"></param>
        public static string Chatroom_QueuePoll(long roomId, string key = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() } };
            if (!string.IsNullOrEmpty(key)) parameters.Add("key", key);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomQueuePoll, parameters);
            return json;
        }

        /// <summary>
        /// 排序列出队列中所有元素
        /// 
        /// 接口描述
        /// 排序列出队列中所有元素
        /// 
        /// http://dev.netease.im/docs?doc=server&#排序列出队列中所有元素
        /// </summary>
        /// <param name="roomId"></param>
        public static string Chatroom_QueueList(long roomId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() } };
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomQueueList, parameters);
            return json;
        }

        /// <summary>
        /// 删除清理整个队列
        /// 
        /// 接口描述
        /// 删除清理整个队列
        /// 
        /// http://dev.netease.im/docs?doc=server&#删除清理整个队列
        /// </summary>
        /// <param name="roomId"></param>
        public static string Chatroom_QueueDrop(long roomId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() } };
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomQueueDrop, parameters);
            return json;
        }

        /// <summary>
        /// 初始化队列
        /// 
        /// 接口描述
        /// 初始化队列
        /// 
        /// http://dev.netease.im/docs?doc=server&#初始化队列
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="sizeLimit">队列长度限制，0~1000</param>
        public static string Chatroom_QueueInit(long roomId, long sizeLimit)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "sizeLimit", sizeLimit.ToString() } };
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomQueueInit, parameters);
            return json;
        }

        /// <summary>
        /// 将聊天室整体禁言
        /// 
        /// 接口描述
        /// 设置聊天室整体禁言状态（仅创建者和管理员能发言）
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="operatorAccId"></param>
        /// <param name="needNotify"></param>
        /// <param name="notifyExt"></param>
        /// <param name="mute"></param>
        /// <returns></returns>
        public static string Chatroom_RoomMute(long roomId, string operatorAccId, string needNotify, string notifyExt, string mute = "false")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { "roomid", roomId.ToString() }, { "operator", operatorAccId } };
            if (!string.IsNullOrEmpty(needNotify)) parameters.Add("needNotify", needNotify);
            if (!string.IsNullOrEmpty(notifyExt)) parameters.Add("notifyExt", notifyExt);
            string json = RequestModel.DataInterface(NeteaseImAction.NeteaseImChatroomMute, parameters);
            return json;
        }

        #endregion














    }
}