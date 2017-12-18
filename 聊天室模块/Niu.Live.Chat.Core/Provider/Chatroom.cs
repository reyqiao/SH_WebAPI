using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;

namespace Niu.Live.Chat.Core.Provider
{
    public class Chatroom
    {


        public static Model.Im_Chatroom NeteaseIm_Create(string creator, string name, string announcement = "", string broadcastUrl = "", string ext = "")
        {
            Model.Im_Chatroom chatroom = new Model.Im_Chatroom();

            string json = NeteaseIm.Implements.Chatroom_Create(creator, name, announcement, broadcastUrl, ext);

            var jObject = JObject.Parse(json);
            if (jObject != null && jObject["code"] != null)
            {
                string code = (string)jObject["code"];
                string desc = string.Empty;
                if (code == "200")
                {
                    long roomId = (long)jObject["chatroom"]["roomid"];
                    chatroom.roomid = roomId;
                    chatroom.creator = creator;
                    chatroom.name = name;
                    chatroom.announcement = announcement;
                    chatroom.broadcasturl = broadcastUrl;
                    chatroom.valid = true;
                    chatroom.ext = ext;
                    chatroom.onlineusercount = 0;
                    Access.Chatroom.ChatroomInsert(roomId, creator, name, true, announcement, broadcastUrl, ext);
                }
            }
            return chatroom;
        }

        public static Model.Im_Chatroom NeteaseIm_Get(long roomId, bool needOnlineUserCount = false)
        {
            Model.Im_Chatroom chatroom = new Model.Im_Chatroom();

            string json = NeteaseIm.Implements.Chatroom_Get(roomId, needOnlineUserCount.ToString().ToLower());

            var jObject = JObject.Parse(json);
            if (jObject != null && jObject["code"] != null)
            {
                string code = (string)jObject["code"];
                string desc = string.Empty;
                if (code == "200")
                {
                    chatroom.roomid = roomId;
                    chatroom.creator = jObject["chatroom"]["creator"].ToString();
                    chatroom.name = (string)jObject["chatroom"]["name"];
                    chatroom.announcement = (string)jObject["chatroom"]["announcement"];
                    chatroom.broadcasturl = (string)jObject["chatroom"]["broadcastUrl"];
                    chatroom.valid = (bool)jObject["chatroom"]["valid"];
                    chatroom.ext = jObject["chatroom"]["ext"].ToString();
                    if (jObject["chatroom"]["onlineusercount"] != null) chatroom.onlineusercount = (int)jObject["chatroom"]["onlineusercount"];
                }
            }
            return chatroom;
        }

        public static Model.Im_Chatroom NeteaseIm_Update(long roomId, string name = "", string announcement = "", string broadcastUrl = "", string ext = "", string needNotify = "", string notifyExt = "")
        {
            Model.Im_Chatroom chatroom = new Model.Im_Chatroom();

            string json = NeteaseIm.Implements.Chatroom_Update(roomId, name, announcement, broadcastUrl, ext, needNotify, notifyExt);
            var jObject = JObject.Parse(json);
            if (jObject != null && jObject["code"] != null)
            {
                string code = (string)jObject["code"];
                string desc = string.Empty;
                if (code == "200")
                {
                    chatroom.roomid = roomId;
                    chatroom.creator = jObject["chatroom"]["creator"].ToString();
                    chatroom.name = (string)jObject["chatroom"]["name"];
                    chatroom.announcement = (string)jObject["chatroom"]["announcement"];
                    chatroom.broadcasturl = (string)jObject["chatroom"]["broadcasturl"];
                    chatroom.valid = (bool)jObject["chatroom"]["valid"];
                    chatroom.ext = jObject["chatroom"]["ext"].ToString();

                    Access.Chatroom.ChatroomUpdate(roomId, chatroom.name, chatroom.announcement, chatroom.broadcasturl, chatroom.ext);
                }
            }
            return chatroom;
        }

        public static Model.Im_Chatroom NeteaseIm_ToggleCloseStat(long roomId, string operatorAccId, bool valid, long niuguAdmin = 0)
        {
            Model.Im_Chatroom chatroom = new Model.Im_Chatroom();

            string json = NeteaseIm.Implements.Chatroom_ToggleCloseStat(roomId, operatorAccId, valid.ToString().ToLower());
            var jObject = JObject.Parse(json);
            if (jObject != null && jObject["code"] != null)
            {
                string code = (string)jObject["code"];
                string desc = string.Empty;
                if (code == "200")
                {
                    chatroom.roomid = roomId;
                    chatroom.creator = jObject["desc"]["creator"].ToString();
                    chatroom.name = (string)jObject["desc"]["name"];
                    chatroom.announcement = (string)jObject["desc"]["announcement"];
                    chatroom.broadcasturl = (string)jObject["desc"]["broadcasturl"];
                    chatroom.valid = (bool)jObject["desc"]["valid"];
                    chatroom.ext = jObject["desc"]["ext"].ToString();

                    Access.Chatroom.ChatroomToggleCloseStat(roomId, operatorAccId, valid.ToString().ToLower(), niuguAdmin);
                }
                if (code == "417" && jObject["desc"] != null && (string)jObject["desc"] == "duplicated operation")
                {
                    chatroom = Access.Chatroom.ChatroomGet(roomId);
                    chatroom.valid = valid;
                }
            }
            return chatroom;
        }

        public static bool NeteaseIm_SetMemberRole(long roomId, string operatorAccId, string target, ChatroomRole opt, bool optvalue, string notifyExt = "")
        {
            string json = NeteaseIm.Implements.Chatroom_SetMemberRole(roomId, operatorAccId, target, ((int)opt).ToString(), optvalue.ToString().ToLower(), notifyExt);
            var jObject = JObject.Parse(json);
            if (jObject != null && jObject["code"] != null)
            {
                string code = (string)jObject["code"];
                string desc = string.Empty;
                if (code == "200")
                {
                    Access.Chatroom.ChatroomSetMemberRole(roomId, operatorAccId, target, ((int)opt).ToString(), optvalue.ToString().ToLower(), notifyExt);
                    return true;
                }
                //{"desc": "duplicate operation on MANAGER!","code": 417}
                if (code == "417" && jObject["desc"] != null && (string)jObject["desc"] == "duplicate operation on MANAGER!")
                {
                    return true;
                }
            }
            return false;
        }

        public static List<string> NeteaseIm_RequestAddr(long roomId, string accId, string clienttype = "")
        {
            List<string> returnList = new List<string>();
            string json = NeteaseIm.Implements.Chatroom_RequestAddr(roomId, accId, clienttype);
            var jObject = JObject.Parse(json);
            if (jObject != null && jObject["code"] != null)
            {
                string code = (string)jObject["code"];
                string desc = string.Empty;
                if (code == "200")
                {
                    JArray jArray = (JArray)jObject["addr"];
                    returnList = jArray.ToObject<List<string>>();
                }
            }
            return returnList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId">聊天室id</param>
        /// <param name="msgId">客户端消息id，使用uuid等随机串，msgId相同的消息会被客户端去重</param>
        /// <param name="fromAccId">消息发出者的账号accid</param>
        /// <param name="msgType"></param>
        /// <param name="resendFlag">重发消息标记，0：非重发消息，1：重发消息，如重发消息会按照msgid检查去重逻辑</param>
        /// <param name="attach">消息内容，格式同消息格式示例中的body字段,长度限制2048字符</param>
        /// <param name="ext">消息扩展字段，内容可自定义，请使用JSON格式，长度限制4096</param>
        public static dynamic NeteaseIm_SendMsg(string msgId, ChatroomMessageType msgType, long roomId, string fromAccId, int resendFlag = 0, string attach = "", string ext = "", int isAudit = 0, int ismager = 0, int InserDb = 1, int roleId = 0)
        {
            string json = NeteaseIm.Implements.Chatroom_SendMsg(roomId, msgId, fromAccId, (int)msgType, resendFlag, attach, ext);
            Niu.Cabinet.Logging.LogRecord log = new Cabinet.Logging.LogRecord("logpath");
            log.WriteSingleLog("auditsendmsg.txt", string.Format("请求云信结果为:{0}-----", json));
            var jObject = JObject.Parse(json);
            dynamic returnValue = new ExpandoObject();
            if (jObject != null && jObject["code"] != null)
            {
                string code = (string)jObject["code"];
                if (code == "200")
                {
                    if (InserDb == 1)
                    {
                        Access.Chatroom.ChatroomSendMsg(msgId, (int)msgType, roomId, fromAccId, resendFlag, attach, ext,isAudit, ismager, roleId);
                    }

                    returnValue.code = 0;
                    returnValue.desc = "success";
                }
                else
                {
                    string desc = (string)jObject["desc"];
                    returnValue.code = code;
                    returnValue.desc = desc;
                }
            }
            else
            {
                returnValue.code = "";
                returnValue.desc = "json is empty";
            }
            return returnValue;
        }
        public static bool InserDB(string msgId, ChatroomMessageType msgType, long roomId, string fromAccId, int resendFlag = 0, string attach = "", string ext = "", int ismager = 0, int roleId = 0)
        {
            return Access.Chatroom.ChatroomSendMsg(msgId, (int)msgType, roomId, fromAccId, resendFlag, attach, ext, ismager, roleId) > 0;
        }

        public static bool NeteaseIm_TemporaryMute(long roomId, string operatorAccId, string target, long muteDuration, string needNotify = "", string notifyExt = "", long niuguAdmin = 0)
        {
            string json = NeteaseIm.Implements.Chatroom_TemporaryMute(roomId, operatorAccId, target, muteDuration, needNotify, notifyExt);
            var jObject = JObject.Parse(json);
            if (jObject != null && jObject["code"] != null)
            {
                string code = (string)jObject["code"];
                if (code == "200")
                {
                    Access.Chatroom.ChatroomTemporaryMute(roomId, operatorAccId, target, muteDuration, needNotify, notifyExt, niuguAdmin);
                    return true;
                }
            }
            return false;
        }
        public static bool NeteaseIm_Mute(long roomId, string operatorAccId, string mute = "false", string needNotify = "", string notifyExt = "")
        {
            string json = NeteaseIm.Implements.Chatroom_Mute(roomId, operatorAccId, mute, needNotify, notifyExt);
            if (json.Contains("200"))
            {
                Access.Chatroom.ChatroomMute(roomId, operatorAccId, mute, needNotify, notifyExt);
            }
            return false;
        }
        public static dynamic NeteaseIm_BlockList(long roomId)
        {
            return Access.Chatroom.QueryBlockList(roomId);
        }
        /// <summary>
        /// 取当前房间禁言列表
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> QueryMuteList(long roomId)
        {
            return Access.Chatroom.QueryMuteList(roomId);
        }
        /// <summary>
        /// 加入禁言列表
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public static bool AddMuteUser(dynamic model)
        {
            return Access.Chatroom.AddMuteUser(model);
        }
        public static void NeteaseIm_QueueOffer(long roomId, string key, string value)
        {
            string json = NeteaseIm.Implements.Chatroom_QueueOffer(roomId, key, value);
        }

        public static void NeteaseIm_QueuePoll(long roomId, string key = "")
        {
            string json = NeteaseIm.Implements.Chatroom_QueuePoll(roomId, key);
        }

        public static void NeteaseIm_QueueList(long roomId)
        {
            string json = NeteaseIm.Implements.Chatroom_QueueList(roomId);
            var jObject = JObject.Parse(json);
            if (jObject != null && jObject["code"] != null)
            {
                string code = (string)jObject["code"];
                if (code == "200")
                {





                }
            }
        }

        public static void NeteaseIm_QueueDrop(long roomId)
        {
            string json = NeteaseIm.Implements.Chatroom_QueueDrop(roomId);

        }

        public static void NeteaseIm_QueueInit(long roomId, long sizeLimit)
        {
            string json = NeteaseIm.Implements.Chatroom_QueueInit(roomId, sizeLimit);

        }

        public static List<string> GetMsgList(int Index = 1, int page = 20, int audit = 0)
        {
            var temp = Niu.Live.Chat.Core.Access.Chatroom.GetMsgList(Index, 20, audit);
            List<string> list = new List<string>();
            foreach (var item in temp)
            {
                var t1 = item.sendtime;
                string item_ext = item.ext;
                item_ext = item_ext.Insert(1, string.Format("\"name\":\"{1}\",\"userlogo\":\"{2}\",\"addtime\":\"{3}\",\"ismanger\":\"{4}\",", item.id, item.name, item.userlog, ConvertDataTimeLong(t1), item.IsManger));
                list.Add(item_ext);
            }
            return list;
        }
        static long ConvertDataTimeLong(DateTime dt)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = dt.Subtract(dtStart);
            long timeStamp = toNow.Ticks;
            timeStamp = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
            return timeStamp;
        }


    }
}