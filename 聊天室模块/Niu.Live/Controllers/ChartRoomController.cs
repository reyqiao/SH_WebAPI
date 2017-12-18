using Niu.Cabinet.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Niu.Live.Chat.Core.Provider;
using _convert = Niu.Cabinet.Conversion;
using Niu.Live.Chat.BusinessLogic;
using Niu.Live.Chat.Core.Model;
using Niu.Live.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using Niu.Live.User.Provider;
using Niu.Live.User.IModel.UserInfo;
using Niu.Live.User.Provider.TokenManager;
using Newtonsoft.Json;
namespace Niu.Live.Controllers
{
    public class ChartRoomController : Controller
    {
        //
        // GET: /ChartRoom/

        public JsonResult Index()
        {
            return Json(new { result = 1, code = 0, message = "" }, "application/json", JsonRequestBehavior.AllowGet);
        }
        public JsonResult test()
        {
            MsgContent ms = new MsgContent()
            {
                msgId = "1111",
                attach = "这是一个图片",
                ext = new Ext()
                {
                    height = 100,
                    width = 100,
                    mstype = 1
                }
            };
            return Json(ms, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StopLiveMsg(int id = 0, string username = "", string t = "")
        {
            MsgContent cont = new MsgContent()
            {
                msgId = Guid.NewGuid().ToString().Substring(0, 31),
                attach = "close",
                isRobot = 0,
                isAudit = 1,
                ismanger = "1",
                ext = new Ext()
                {
                    mstype = 10
                }
            };
            switch (id)
            {
                case 99999://停止
                    break;
                case 88888://开始
                    cont.ext.mstype = 9;
                    break;
                case 77777://发送一条喊单信息
                    cont.ext.mstype = 8;
                    cont.attach = string.Format("{0}发布了一条{1}策略", username, t);
                    cont.name = username;
                    cont.roleId = 14;//老师的roleid
                    break;
            }

            var temp = Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_SendMsg(cont.msgId, Niu.Live.Chat.Core.Provider.ChatroomMessageType.Custom, 9072833, "5e86937b-dcff-4a50-889d-86e4f1c", 0, "", Newtonsoft.Json.JsonConvert.SerializeObject(cont), 1, 0, 0);
            return Json(new { result = 1, code = 0, message = "" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// .post发送消息
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SendMsg(FormCollection collection)
        {

            if (collection == null) return Json(new { result = 0, code = 11, message = "" });
            string userToken = _convert.ObjectConvert.ChangeType<string>(Request.Headers["usertoken"] ?? collection["usertoken"]);
            if (string.IsNullOrEmpty(userToken)) return Json(new { result = 0, code = 11, message = "" });
            long roomId = _convert.ObjectConvert.ChangeType<long>(collection["roomId"]);
            if (roomId <= 0) return Json(new { result = 0, code = 11, message = "" });
            string content = _convert.ObjectConvert.ChangeType<string>(collection["content"] ?? null);
            string sourceMsgId = _convert.ObjectConvert.ChangeType<string>(collection["sourceMsgId"]);
            string msgtype = _convert.ObjectConvert.ChangeType<string>(collection["msgtype"] ?? null);
            int isRobot = _convert.ObjectConvert.ChangeType<int>(collection["isRobot"] == null ? 0 : 1);
            int _roleId = _convert.ObjectConvert.ChangeType<int>(collection["roleId"] ?? collection["roleId"], 0);
            string userIp = Niu.Cabinet.Web.ExtensionMethods.RequestExtensions.ClientIp(Request);
            string returnExt = string.Empty;
            long userID = 0;
            Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
            Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(userToken, out userInfo);
            if (userInfo == null) return Json(new { result = 1, code = 13, message = "token error" });
            userID = userInfo.userId;
            if (Niu.Live.Chat.Core.Access.Chatroom.QueryBlock(new { roomId = roomId, target = userID })) return Json(new { result = 1, code = 15, message = "您当前不能发言" });
            string attach = string.Empty;//消息主体
            string ext = string.Empty;//消息扩展字段
            #region 图片内容
            MsgContent msgcontent = new MsgContent()
            {
                userId = userInfo.userId,
                ismanger = userInfo.isManage.ToString(),
                name = userInfo.nickName,
                roleId = _roleId == 0 ? userInfo.roleId : _roleId

            };
            int[] arry = new int[] { 5, 6, 7, 13, 14 };//管理员id列表
            if (arry.Contains(userInfo.roleId) || isRobot == 1)
            {
                msgcontent.isAudit = 1;
            }
            if (isRobot == 1)
            {
                msgcontent.roleId = _roleId;
            }
            msgcontent.ext = new Ext();
            var resultdata = PhotoHandler.Upload(userID, Request.Files);
            if (resultdata != null && resultdata.data != null && resultdata.data.Count > 0)
            {
                ext = Newtonsoft.Json.JsonConvert.SerializeObject(new { w = resultdata.data[0].width, h = resultdata.data[0].height });
                attach = resultdata.data[0].url;
                msgcontent.ext.width = resultdata.data[0].width;
                msgcontent.ext.height = resultdata.data[0].height;
                msgcontent.ext.mstype = 2;
                msgcontent.ext.url = attach;//原图地址 
                msgcontent.attach = string.Format("{0}/200", attach);
            }
            #endregion
            else
            {

                #region 文本内容
                if (resultdata == null || resultdata.data == null || resultdata.data.Count == 0)
                {
                    ////发送彩条时候，重写消息体
                    //if (!string.IsNullOrEmpty(msgtype))
                    //{
                    //    if (int.Parse(msgtype) > 0)
                    //    {
                    //        content = "http://live.fxtrade888.com/chatroom/images/dingyige.gif";
                    //    }
                    //}
                    if (string.IsNullOrEmpty(content) || content.Length <= 0 || content.Length > 300)
                        return Json(new { result = 0, code = 31, message = "内容过长度不匹配" });
                    msgcontent.attach = attach = content;
                    msgcontent.ext.mstype = 1;
                    if (!string.IsNullOrEmpty(msgtype))
                    {
                        msgcontent.ext.mstype = int.Parse(msgtype);
                        //彩条信息
                        switch (msgcontent.ext.mstype)
                        {
                            case 3:
                                msgcontent.ext.url = "http://live.fxtrade888.com/chatroom/images/dingyige.gif";//顶
                                msgcontent.attach = "http://live.fxtrade888.com/chatroom/images/dingyige.gif";
                                break;
                            case 7:
                                msgcontent.ext.url = "http://live.fxtrade888.com/chatroom/images/zanyige.gif";//赞
                                msgcontent.attach = "http://live.fxtrade888.com/chatroom/images/zanyige.gif";//赞
                                break;
                            case 5:
                                msgcontent.ext.url = "http://live.fxtrade888.com/chatroom/images/zhangsheng.gif";//赞
                                msgcontent.attach = "http://live.fxtrade888.com/chatroom/images/zhangsheng.gif";//掌
                                break;
                            case 6:
                                msgcontent.ext.url = "http://live.fxtrade888.com/chatroom/images/xianhua.gif";//赞
                                msgcontent.attach = "http://live.fxtrade888.com/chatroom/images/xianhua.gif";//花
                                break;
                        }
                        msgcontent.isAudit = 1;
                    }
                }
                #endregion
            }

            long sourceId = 0;
            string sourceUserName = string.Empty;
            string sourceContent = string.Empty;
            #region 引用数据

            if (!string.IsNullOrEmpty(sourceMsgId))
            {
                Niu.Live.Chat.Core.Model.Im_Chatroom_Msg sourceMsg = Niu.Live.Chat.Core.Access.Chatroom.ChatroomGetMsg(sourceMsgId);
                if (sourceMsg != null && !string.IsNullOrEmpty(sourceMsg.attach))
                {
                    var fromuserinfo = Niu.Live.Chat.Core.Access.User.UserQuery(sourceMsg.fromAccId);
                    var touserinof = Niu.Live.Chat.Core.Access.User.QueryImUserByUserId(userInfo.userId);

                    msgcontent.ext.other = new { from = userInfo.nickName, to = fromuserinfo.name, content = sourceMsg.attach };
                    msgcontent.attach = content;
                    msgcontent.ext.mstype = 3;
                    sourceId = sourceMsg.id;
                    sourceContent = sourceMsg.attach;
                }
            }
            #endregion
            #region 判断审核
            //int auditSign = 0;
            //if (feeType > 0) auditSign = -1;
            //必审用户
            //if (Niugu.ChatLive.DataAccess.UserAudit.IsAuditUser(userID)) auditSign = -1;
            //敏感词


            //if (auditSign != -1 && Niugu.Community.Functions.FilterContent.WordsCheck.ContainsContact(content)) auditSign = -1;
            ////审核词
            //if (auditSign != -1)
            //{
            //    int checkResult = 0;
            //    string msg = string.Empty;
            //    Niugu.Community.Functions.FilterContent.AuditWordFacade.FilterWordCheck(content, out checkResult, out msg);
            //    if (checkResult == -1) auditSign = -1;
            //}
            #endregion
            #region 发送请求

            string jsonext = string.Empty;
            string msgId = Niu.Live.Chat.Core.Utils.UniqueValue.GenerateGuid();
            msgcontent.msgId = msgId;
            msgcontent.addtime = Cabinet.Time.TimeStmap.DateTimeToUnixTimeStmap(DateTime.Now);
            Im_User imUser = new Im_User();
            if (isRobot == 1)
            {
                imUser.accid = Guid.NewGuid().ToString().Substring(0, 31);
                imUser.name = _convert.ObjectConvert.ChangeType<string>(collection["Robotname"] ?? "");
                msgcontent.isRobot = 1;
                msgcontent.Robotname = imUser.name;
                Niu.Live.Chat.Core.Provider.User.NeteaseIm_CreateRobot(imUser.accid, imUser.name);
            }
            else
            {
                imUser = Niu.Live.Chat.Core.Provider.User.NeteaseIm_Create(userID.ToString(), userInfo.nickName, "");
            }

          
           //#if DEBUG
           // roomId = 9884735;
           // #endif 
          
            #endregion
            var temp = Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_SendMsg(msgId, Niu.Live.Chat.Core.Provider.ChatroomMessageType.Custom, roomId, imUser.accid, 0, attach, Newtonsoft.Json.JsonConvert.SerializeObject(msgcontent), msgcontent.isAudit, userInfo.isManage, 1, _roleId == 0 ? userInfo.roleId : _roleId);
            return Json(new { result = 0, code = temp.code, msg = temp.desc });
 

        }

        // GET: Chatroom/Detail
        /// <summary>
        /// 获取图文直播间所以消息
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <param name="direction"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ContentResult Detail(FormCollection collection, long id = 0, int direction = 0, int order = 0)
        {
            string userToken = _convert.ObjectConvert.ChangeType<string>(Request.Headers["usertoken"] ?? collection["usertoken"]);
            long roomId = _convert.ObjectConvert.ChangeType<long>(collection["roomId"], 0);

            // id = (id == 0 ? long.MaxValue : id);
            direction = (direction == 0 ? -1 : direction);
            order = (order == 0 ? 1 : order);
            int size = 20;

            if (roomId <= 0) return Content("{\"result\":0,\"code\":11,\"message\":\"\"}", "application/json");

            Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
            Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(userToken, out userInfo);

            if (userInfo == null || userInfo.userId <= 0) return Content("{\"result\":0,\"code\":12,\"message\":\"userId不正确\"}", "application/json");
            List<string> list = Niu.Live.Chat.BusinessLogic.ChatroomExtCache.Detail(userInfo.userId, roomId, id, direction, size, order, userInfo.isManage);

            string im_data = string.Format("[{0}]", string.Join(",", list));

            return Content(string.Format("{{\"result\":1,\"code\":0,\"message\":\"\",\"im_data\":{0}}}", im_data), "application/json");

        }

        /// <summary>
        /// 添加用户到黑名单
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public JsonResult BlockUser(FormCollection collection)
        {
            if (collection == null) return Json(new { result = 0, code = 11, message = "" });
            string userToken = _convert.ObjectConvert.ChangeType<string>(Request.Headers["usertoken"] ?? collection["usertoken"]);
            Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
            Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(userToken, out userInfo);
            if (collection == null) return Json(new { result = 0, code = 11, message = "" });
            if (userInfo == null || userInfo.userId < 0) return Json(new { result = 1, code = 13, message = "token error" });
            string Target = _convert.ObjectConvert.ChangeType<string>(collection["target"]);
            string TargetName = _convert.ObjectConvert.ChangeType<string>(collection["TargetName"]);
            if (userInfo.roleId > 1)
            {
                // var accid = Niu.Live.Chat.Core.Access.User.UserQueryById(userInfo.userId).accid;
                Niu.Live.Chat.Core.Access.Chatroom.BlockUser(new { RoomId = 9072833, AccId = userInfo.userId, Target = Target, TargetName = TargetName });
                return Json(new { result = 1, code = 0, message = "" });
            }
            else
            {
                return Json(new { result = 1, code = -1, message = "authorization failed" });
            }

        }
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public JsonResult Del(FormCollection collection)
        {
            if (collection == null) return Json(new { result = 0, code = 11, message = "" });
            string userToken = _convert.ObjectConvert.ChangeType<string>(Request.Headers["usertoken"] ?? collection["usertoken"]);
            Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
            Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(userToken, out userInfo);
            if (userInfo == null) return Json(new { result = 1, code = 13, message = "token error" });
            string msgid = _convert.ObjectConvert.ChangeType<string>(Request.Headers["msgid"] ?? collection["msgid"]);
            if (Niu.Live.Chat.DataAccess.Chatroom.DelMsg(msgid))
            {
                return Json(new { result = 1, code = 0, message = "" });
            }
            return Json(new { result = 0, code = -1, message = "" });
        }

        public JsonResult Audit(FormCollection collection)
        {
            if (collection == null) return Json(new { result = 0, code = 11, message = "" });
            string userToken = _convert.ObjectConvert.ChangeType<string>(Request.Headers["usertoken"] ?? collection["usertoken"]);
            string msgid = _convert.ObjectConvert.ChangeType<string>(Request.Headers["msgid"] ?? collection["msgid"]);
            Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
            Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(userToken, out userInfo);
            if (userInfo == null) return Json(new { result = 0, code = 13, message = "token error" });
            if (userInfo.userId > 0 & userInfo.roleId > 1)
            {
                if (Niu.Live.Chat.DataAccess.Chatroom.AuditMsg(new { admin = userInfo.userId, msgid = msgid }))
                {
                    Niu.Cabinet.Logging.LogRecord log = new Cabinet.Logging.LogRecord("logpath");
                    log.WriteSingleLog("auditsendmsg.txt", string.Format("审核完成:开始请求云信:{0}-----", msgid));
                    //审核完成再发一条信息过去.
                    var temp = Chat.Core.Access.Chatroom.ChatroomGetMsg(msgid);
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<MsgContent>(temp.ext);
                    obj.isAudit = 1;
                    obj.msgId = Guid.NewGuid().ToString().Substring(0, 31);
                    var _ext = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    Niu.Live.Chat.DataAccess.Chatroom.UpContent(new { msgid = msgid, ext = _ext, newmsgid = obj.msgId });
                    Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_SendMsg(obj.msgId, ChatroomMessageType.Custom, temp.roomId, temp.fromAccId, 0, temp.attach, _ext, temp.IsManger, 0, temp.RoleId);
                    log.WriteSingleLog("auditsendmsg.txt", string.Format("请求云信完成:{0}-----", _ext));
                    return Json(new { result = 1, code = 0, message = "" });
                }
            }
            return Json(new { result = 0, code = 0, message = "auth failed" });
        }
        /// <summary>
        /// 取当前房间所有黑名单用户
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public string GetRoomBlockUser(long Id = 0)
        {
            if (Id > 0)
            {
                var result = Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_BlockList(Id);
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { result = 1, code = 0, data = result, message = "" });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { result = 1, code = 0, data = "", message = "" });
        }
        /// <summary>
        /// 移除房间黑名单用户
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public JsonResult RemoveBlockUser(FormCollection collection)
        {
            string userToken = _convert.ObjectConvert.ChangeType<string>(Request.Headers["usertoken"] ?? collection["usertoken"]);
            string target = _convert.ObjectConvert.ChangeType<string>(Request.Headers["target"] ?? collection["target"]);
            Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
            Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(userToken, out userInfo);
            if (userInfo == null) return Json(new { result = 1, code = 13, message = "token error" });
            if (Niu.Live.Chat.Core.Access.Chatroom.RemoveBlockUser(new { roomId = 9072833, target = target }) > 0)
            {
                return Json(new { result = 1, code = 0, message = "" });
            }
            return Json(new { result = 0, code = 11, message = "房间参数不对" });
        }
        /// <summary>
        /// 临时禁言用户最大时间
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public JsonResult TempMuteUser(FormCollection collection)
        {

            if (collection == null) return Json(new { result = 0, code = 11, message = "" });
            var temp = new TempMuteRoom();
            if (TryUpdateModel<TempMuteRoom>(temp, collection))
            {
                Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_TemporaryMute(temp.roomid, temp.accid, temp.target, temp.muteDuration, temp.needNotify, temp.notifyExt);
                return Json(new { result = 1, code = 0, message = "" });
            }
            else
                return Json(new { result = 0, code = 11, message = "参数转化失败" });
        }
        [HttpGet]
        public JsonResult MuteUserList(long Id)
        {
            if (Id <= 0) return Json(new { result = 0, code = 11, message = "房间参数有误" });
            var result = Niu.Live.Chat.Core.Provider.Chatroom.QueryMuteList(Id);
            return Json(new { result = 1, code = 0, data = result, message = "" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 禁言聊天室
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public JsonResult MuteRoom(FormCollection collection)
        {
            if (collection == null) return Json(new { result = 0, code = 11, message = "" });
            var roomId = _convert.ObjectConvert.ChangeType<long>(collection["roomId"]);
            var accoperator = _convert.ObjectConvert.ChangeType<string>(collection["operator"]);
            string mute = _convert.ObjectConvert.ChangeType<string>(collection["mute"]);
            string needNotify = _convert.ObjectConvert.ChangeType<string>(collection["needNotify"]);
            string notifyExt = _convert.ObjectConvert.ChangeType<string>(collection["notifyExt"]);
            var result = Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_Mute(roomId, accoperator, mute, needNotify, notifyExt);
            if (result)
                return Json(new { result = 1, code = 0, message = "" });
            return Json(new { result = 0, code = 11, message = "" });
        }
        public JsonResult MuteUser(FormCollection collection)
        {
            if (collection == null) return Json(new { result = 0, code = 11, message = "no args" });
            string userToken = _convert.ObjectConvert.ChangeType<string>(Request.Headers["usertoken"] ?? collection["usertoken"]);
            Chatroom.AddMuteUser(new { });
            return null;
        }
        /// <summary>
        /// 创建一个聊天室(创建直播间)
        /// </summary>
        /// <returns></returns>
        public JsonResult CreateChatroom(FormCollection collection)
        {
            long userId = 10000;
            string userName = "公共直播间";
            string userLogoUrl = string.Empty;
            Niu.Live.Chat.Core.Provider.User.NeteaseIm_Create(userId.ToString(), userName, userLogoUrl);
            // Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_Create(Niu.Live.Chat.Core.Provider.User.CreateImeAccId(0, 0, userId), userName);
            return null;
        }
        // GET: Chatroom/ReplyMe
        public JsonResult ReplyMe(FormCollection collection, long id = 0, int direction = 0, int order = 0)
        {
            string userToken = _convert.ObjectConvert.ChangeType<string>(Request.Headers["usertoken"] ?? collection["roomId"]);
            long roomId = _convert.ObjectConvert.ChangeType<long>(Request.Headers["roomId"] ?? collection["roomId"]);
            id = (id == 0 ? long.MaxValue : id);
            direction = (direction == 0 ? -1 : direction);
            order = (order == 0 ? 1 : order);
            int size = 20;
            if (roomId <= 0) return Json(new { result = 0, code = 11, message = "" });

            bool isMaster = false;
            long masterID = 0;
            int niuguRoomType = 0;
            string userid = string.Empty;
            //UserInfo user = null;
            //if (TokenManager.ValidateUserToken(ref userToken, out user) && user != null && user.ID > 0) userID = user.ID;

            //if (userID <= 0) return Content("{\"result\":0,\"code\":12,\"message\":\"\"}", "application/json");
            //if (userID > 0 && user.Type == Niugu.Common.UserType.NiuGuWangDevice) return Content("{\"result\":0,\"code\":13,\"message\":\"\"}", "application/json");

            //Niugu.Community.Common.Model.UserViewModel uvm = Niugu.Community.Functions.UserModel.Get(userID);

            //if (uvm.UserID <= 0) return Json(new { result = 0, code = 14, message = "" });


            List<string> list = Niu.Live.Chat.BusinessLogic.ChatroomExtCache.ReplyMe(roomId, masterID.ToString(), userid, id, direction, size, order);

            string im_data = string.Format("[{0}]", string.Join(",", list));
            return Json(new { result = 1, code = 0, message = "", im_data = im_data });

        }
        /// <summary>
        /// 查找一个聊天室
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult FindChatroom(long Id = 0)
        {
            if (Id <= 0)
                return Json(new { result = 0, code = 1, message = "参数错误" }, JsonRequestBehavior.AllowGet);
            var result = Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_Get(Id, true);
            if (result != null && result.roomid > 0)
                return Json(new { result = 1, code = 0, message = "", data = result }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { result = 0, code = 2, message = "暂无数据" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 更新一个聊天室
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateChatroom(FormCollection collection)
        {
            if (collection == null) return Json(new { result = 0, code = 1, message = "参数错误" });
            var temp = new ChatRoom();
            if (TryUpdateModel<ChatRoom>(temp, collection))
            {
                if (temp.roomid > 0)
                {
                    Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_Update(temp.roomid, temp.name, temp.announcement, temp.broadcasturl, temp.ext, temp.needNotify);
                    return Json(new { result = 1, code = 0, message = "" });
                }
                else
                    return Json(new { result = 0, code = 1, message = "房间号不正确" });

            }
            else
                return Json(new { result = 0, code = 1, message = "参数转化失败" });
        }
        /// <summary>
        /// 切换房间开关状态
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public JsonResult ToggleCloseStat(FormCollection collection)
        {
            if (collection == null) return Json(new { result = 0, code = 1, message = "参数错误" });
            var temp = new ChatRoom();
            if (TryUpdateModel<ChatRoom>(temp, collection))
            {
                if (temp.roomid > 0)
                {
                    Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_ToggleCloseStat(temp.roomid, temp.operatoraccid, temp.valid);
                    return Json(new { result = 1, code = 0, message = "" });
                }
                else
                    return Json(new { result = 0, code = 1, message = "房间号不正确" });
            }
            else
                return Json(new { result = 0, code = 1, message = "参数转化失败" });
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="dyn"></param>
        /// <returns></returns>
        public JsonResult CreateImUser(dynamic dyn)
        {
            var result = Niu.Live.Chat.Core.Provider.User.NeteaseIm_Create(dyn.userid, dyn.userName, dyn.userLogoUrl, dyn.channid, dyn.usertype);
            if (result != null)
                return Json(new { result = 0, code = 1, message = "操作成功" });
            return Json(new { result = 1, code = 0, message = "" });
        }
        public JsonResult TopN()
        {
            return null;
        }
        public JsonResult GetInfo(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
                Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(token, out userInfo);
                var temp = Niu.Live.Chat.Core.Access.User.QueryImUserByUserId(userInfo.userId);
                return Json(new { result = 1, code = 0, accId = temp, userId = userInfo.userId, chatRoomId = 100 }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { result = 0, code = 0, message = "用户token不正确!" }, JsonRequestBehavior.AllowGet);

        }
        // POST: Chatroom/RequestAddr
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="roomId">9072833公共roomid </param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult RequestAddr(FormCollection collection, long roomId = 9072833)
        {

            string userToken = Niu.Cabinet.Conversion.ObjectConvert.ChangeType<string>(Request.Headers["usertoken"] ?? collection["usertoken"]);

            if (roomId <= 0) return Content("{\"result\":0,\"code\":11,\"message\":\"\"}", "application/json");

            long userID = 0;
            Live.User.IModel.TokenManager.TokenUserInfo user = null;

            if (TokenManager.ValidateUserToken(userToken, out user) && user != null && user.userId > 0) userID = user.userId;
            if (userID <= 0) return Content("{\"result\":0,\"code\":12,\"message\":\"\"}", "application/json");
            var temp = Niu.Live.Chat.Core.Provider.User.NeteaseIm_Create(userID.ToString(), user.nickName, "");
            if (temp == null)
            {
                return Content("{\"result\":0,\"code\":14,\"message\":\"\"}", "application/json");
            }
            else
            {

                List<string> list = Niu.Live.Chat.Core.Provider.Chatroom.NeteaseIm_RequestAddr(roomId, temp.accid);
                list = list.Select(t => "\"" + t + "\"").ToList();
                string im_addr = string.Join(",", list);
                return Content(string.Format("{{\"result\":1,\"code\":0,\"message\":\"\",\"accId\":\"{1}\",\"accToken\":\"{2}\",\"roomId\":\"{3}\",\"im_addr\":[{0}]}}", im_addr, temp.accid, temp.token, roomId), "application/json");
            }
        }
        public JsonResult AddRobot(string usertoken, string robotname, int roleId)
        {
            if (!string.IsNullOrEmpty(usertoken))
            {
                Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
                Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(usertoken, out userInfo);
                if (userInfo.roleId > 1)
                {
                    if (Niu.Live.Chat.DataAccess.Chatroom.AddlRobot(robotname, userInfo.userId, roleId))
                        return Json(new { result = 1, code = 0, msg = "" });
                }
            }
            return Json(new { result = 0, code = 0, msg = "auth failed" });
        }
        public string RobotList(string usertoken)
        {
            if (!string.IsNullOrEmpty(usertoken))
            {
                Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
                Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(usertoken, out userInfo);
                if (userInfo.roleId > 1)
                {
                    var result = Niu.Live.Chat.DataAccess.Chatroom.GetRobotList(userInfo.userId);
                    return Newtonsoft.Json.JsonConvert.SerializeObject((new { result = 1, code = 0, data = result }));
                }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject((new { result = 0, code = 0, msg = "auth failed" }));
        }
        public JsonResult DelRobot(string usertoken, int Id)
        {

            if (!string.IsNullOrEmpty(usertoken))
            {
                Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
                Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(usertoken, out userInfo);
                if (userInfo.roleId > 1)
                {
                    if (Niu.Live.Chat.DataAccess.Chatroom.DelRobot(Id))
                        return Json(new { result = 1, code = 0, msg = "" });
                }
            }
            return Json(new { result = 0, code = 0, msg = "auth failed" });
        }

        #region 取私信列表
        public string MsgList(string usertoken, int Index = 1, int audit = 0)
        {
            if (!string.IsNullOrEmpty(usertoken))
            {
                Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
                Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(usertoken, out userInfo);
                var temp = Niu.Live.Chat.Core.Access.Chatroom.GetMsgList(Index, 20, audit);
                List<string> list = Niu.Live.Chat.Core.Provider.Chatroom.GetMsgList(Index, 20, audit);
                string im_data = string.Format("[{0}]", string.Join(",", list));
                return string.Format("{{\"result\":1,\"code\":0,\"message\":\"\",\"data\":{0}}}", im_data);

            }
            else
                return JsonConvert.SerializeObject(new { result = 0, code = 0, msg = "auth failed" });
        }
        #endregion
        #region 云信基础信息
        public string GetNetInfo(string usertoken)
        {
            if (!string.IsNullOrEmpty(usertoken))
            {
                Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo;
                if (Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(usertoken, out userInfo) && userInfo != null)
                {
                    var temp = Niu.Live.Chat.Core.Provider.User.NeteaseIm_Create(userInfo.userId.ToString(), userInfo.nickName, "");
                    return JsonConvert.SerializeObject(new { result = 1, code = 0, data = new { accid = temp.accid, acctoken = temp.token } });
                }
            }
            return JsonConvert.SerializeObject(new { result = 0, code = 0, msg = "auth failed" });

        }

        public string GetCount()
        {
            int result = 0;
            int hours = DateTime.Now.Hour;
            if (hours >= 10 && hours <= 12)
            {
                result = 100;
            }
            else if (hours > 12 && hours <= 18)
            {
                result = 200;
            }
            else if (hours > 18 && hours <= 22)
            {
                result = 500;
            }
            //int temp = int.Parse(HttpContext.Application["OnLineUserCount"].ToString());
            // int hours = DateTime.Now.Hour / 10;
            //switch (hours)
            //{
            //    case 0:
            //        result = temp;
            //        break;
            //    case 1:
            //        result = temp + 40;
            //        break;
            //    case 2:
            //        result = temp + 60;
            //        break;
            //}
            return JsonConvert.SerializeObject(new { result = 1, code = 0, data = result });
        }
        #endregion
    }
}
