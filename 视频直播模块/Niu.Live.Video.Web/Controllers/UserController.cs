using Niu.Live.Video.BusinessLogic;
using Niu.Live.Video.Model;
using Niu.Live.Video.Web.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Niu.Live.Video.Web.Controllers
{
    [JsonFilter]
    public class UserController : Controller
    {
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }
        // 登录
        /// </summary>
        /// <param name="usertoken"></param>
        /// <returns></returns>
        public ActionResult Login(string usertoken)
        {
            Niu.Cabinet.Logging.LogRecord log = new Cabinet.Logging.LogRecord(Niu.Cabinet.Config.AppSetting.AppSettingString("logpath"));
            log.WriteSingleLog("video.txt", "用户登陆"+ usertoken);
            BaseResult br = new BaseResult();
            br.result = 1;
            br.message = "";
            br.code = 0;
            Niu.Live.User.IModel.TokenManager.TokenUserInfo userinfo;
            if (Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(usertoken, out userinfo) && userinfo.userId > 0)
            {
                //  new VideoLive().Login(userinfo.userId);
                log.WriteSingleLog("video.txt", userinfo.nickName+"登陆成功");
                br.message = Newtonsoft.Json.JsonConvert.SerializeObject(VideoList.GetUser(userinfo.userId));
            }
            return Json(br, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建直播
        /// </summary>
        /// <param name="usertoken"></param>
        /// <param name="desc"></param>
        /// <param name="title"></param>
        /// <param name="cover"></param>
        /// <param name="mainId"></param>
        /// <param name="closeother"></param>
        /// <returns></returns>
        public ActionResult CreateVideo(string usertoken, string desc, string title, string cover, string mainId, string closeother)
        {
            Niu.Cabinet.Logging.LogRecord log = new Cabinet.Logging.LogRecord(Niu.Cabinet.Config.AppSetting.AppSettingString("logpath"));
            log.WriteSingleLog("video.txt", string.Format("创建直播 ，usertoken：{0}；title：{1}；mainId：{2}", usertoken, title, mainId));
            VideoResult br = new VideoResult();
            br.result = 0;
            br.message = "用户错误！";
            br.code = 0;
            cover = "";//封面后台维护

            title = HttpUtility.JavaScriptStringEncode(title);
            desc = HttpUtility.JavaScriptStringEncode(desc);

            Niu.Live.User.IModel.TokenManager.TokenUserInfo userinfo;
            if (Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(usertoken, out userinfo))
            {
                var temp = new VideoLive().CreateVideo(userinfo.userId, desc, title, cover, mainId);
                br.data = Newtonsoft.Json.JsonConvert.SerializeObject(new { rtmpurl = temp.RTMPUrl, scid = temp.RStreamId });
                var url = "https://live.fxtrade888.com/chatroom/ChartRoom/StopLiveMsg?id=88888";
                WebClient wc = new WebClient();
                var res = wc.DownloadString(url);
                log.WriteSingleLog("SendMsg.txt", string.Format("云信发送关闭:{0}", temp));
            }
            if (br is VideoResult && !string.IsNullOrWhiteSpace(((VideoResult)br).data))
            {
                log.WriteSingleLog("video.txt", string.Format("IsNullOrWhiteSpace--CreateVideo返回结果 ，{0}", ((VideoResult)br).ToFormatString()));
                return Content(((VideoResult)br).ToFormatString());
            }
            log.WriteSingleLog("video.txt", string.Format("CreateVideo返回结果 ，{0}", Newtonsoft.Json.JsonConvert.SerializeObject(br)));
            return Json(br, JsonRequestBehavior.AllowGet);
            //UserInfo userInfo = new UserInfo();
            //if (TokenManager.ValidateUserToken(ref usertoken, out userInfo) && userInfo.ID > 0)
            //{
            //    br = UserBL.Instance.CreateVideo(userInfo.ID, desc, title, cover, mainId, closeother);
            //}
            //if (br is VideoResult && !string.IsNullOrWhiteSpace(((VideoResult)br).data))
            //    return Content(((VideoResult)br).ToFormatString());
        }

        /// <summary>
        /// 关闭直播
        /// </summary>
        /// <param name="usertoken"></param>
        /// <param name="scid"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        public ActionResult CloseVideo(string usertoken, string scid, string mainId)
        {
            Niu.Cabinet.Logging.LogRecord log = new Cabinet.Logging.LogRecord(Niu.Cabinet.Config.AppSetting.AppSettingString("logpath"));
            log.WriteSingleLog("video.txt", string.Format("obs关闭直播 ，usertoken：{0}；scid：{1}；mainId：{2}", usertoken, scid, mainId));
            VideoResult br = new VideoResult();
            br.result = 0;
            br.message = "用户错误！";
            Niu.Live.User.IModel.TokenManager.TokenUserInfo userinfo;
            if (Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(usertoken, out userinfo))
            {
                new VideoLive().CloseVideo(userinfo.userId, scid);
                var url = "https://live.fxtrade888.com/chatroom/ChartRoom/StopLiveMsg?id=99999";
                WebClient wc = new WebClient();
                var temp = wc.DownloadString(url);
                log.WriteSingleLog("SendMsg.txt", string.Format("云信发送关闭:{0}", temp));
                //  var dt = VideoList.GetLiveLeftJoinLiveVideoByLiveId(Convert.ToInt32(mainId));
                //int roomId = 0;
                //if (dt != null)
                //{
                //    roomId = dt.VideoChatRoom;
                //    string content = "";
                //    string jsonExt = string.Format("{{\"streamId\":\"{0}\",\"liveChannel\":\"{1}\",\"livestate\":\"{2}\",\"liveid\":\"{3}\"}}",
                //              dt.streamId, dt.liveChannel, 1, mainId);
                //   // dynamic d = Niugu.ChatLive.Core.Provider.Chatroom.NeteaseIm_SendMsg(msgId, Niugu.ChatLive.Core.Provider.ChatroomMessageType.Tips, roomId, userInfo.ID.ToString(), 0, content, jsonExt);
                //}
                // var dt = VideoDA.GetLiveLeftJoinLiveVideoByLiveId(Convert.ToInt32(mainId));
                // br = Niu.Live.Video.
            }
            //UserInfo userInfo = new UserInfo();
            //if (TokenManager.ValidateUserToken(ref usertoken, out userInfo) && userInfo.ID > 0)
            //{
            //    br = UserBL.Instance.CloseVideo(userInfo.ID, scid, mainId);
            //    var dt = VideoDA.GetLiveLeftJoinLiveVideoByLiveId(Convert.ToInt32(mainId));
            //    int roomId = 0;
            //    if (dt != null && dt.Rows.Count > 0)
            //    {
            //        roomId = int.Parse(dt.Rows[0]["VideoChatRoom"].ToString());
            //        string content = "";
            //        string jsonExt = string.Format("{{\"streamId\":\"{0}\",\"liveChannel\":\"{1}\",\"livestate\":\"{2}\",\"liveid\":\"{3}\"}}",
            //                    DataUtility.DataToString(dt.Rows[0]["streamId"].ToString()), DataUtility.DataToString(dt.Rows[0]["liveChannel"].ToString()), 1, mainId);
            //        string msgId = Niugu.ChatLive.Core.Utils.UniqueValue.GenerateGuid();
            //        dynamic d = Niugu.ChatLive.Core.Provider.Chatroom.NeteaseIm_SendMsg(msgId, Niugu.ChatLive.Core.Provider.ChatroomMessageType.Tips, roomId, userInfo.ID.ToString(), 0, content, jsonExt);
            //        if (d.code != "200")
            //        {
            //            LogRecord.writeLogsingle("SendMsg.txt", "code:" + d.code + "--desc:" + d.desc);
            //        }
            //    }
            //}

            return Json(br, JsonRequestBehavior.AllowGet);
        }
        //检查直播状态
        public string CheckStatus(string usertoken, string scid)
        {

            Niu.Cabinet.Logging.LogRecord log = new Cabinet.Logging.LogRecord(Niu.Cabinet.Config.AppSetting.AppSettingString("logpath"));
            log.WriteSingleLog("video.txt", string.Format("CheckStatus检查直播状态 ，usertoken：{0}；scid：{1}；", usertoken, scid));
            VideoResult br = new VideoResult();
            br.result = 0;
            br.code = -10;
            br.message = "用户错误！";
            Niu.Live.User.IModel.TokenManager.TokenUserInfo userinfo;
            bool close = false;
            if (Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(usertoken, out userinfo) && userinfo.userId > 0)
            {
                var dt = VideoList.GetVideo(scid);
                if (dt != null)
                {
                    string cmd = dt.obscommand == null ? "" : dt.obscommand.ToString();
                    var message = "";
                    switch (cmd)
                    {
                        case "toClose":
                            message = "直播因网络问题中断，请关闭程序重新登录";
                            break;
                        case "toPMClose":
                            message = "您的直播因合规问题已被停止，请联系客服";
                            cmd = "toClose";
                            break;
                        default:
                            break;
                    }

                    if (dt.stopTime == null || string.IsNullOrWhiteSpace(dt.stopTime.ToString()))
                    {
                        //心跳检测
                        LiveMonitor.AddLive(scid, DateTime.Now);
                        if (!LiveMonitor.IsRun)
                        {
                            LiveMonitor.Start();
                        }

                        br.result = 1;
                        br.code = 1;
                        br.data = string.Format("{{\"status\": \"Run\",\"command\": \"{0}\",\"message\": \"{1}\"}}", cmd, message);
                    }
                    else
                    {
                        br.result = 1;
                        br.code = 0;
                        br.data = string.Format("{{\"status\": \"Close\",\"command\": \"{0}\",\"message\": \"{1}\"}}", cmd, message);
                        var url = "https://live.fxtrade888.com/chatroom/ChartRoom/StopLiveMsg?id=99999";
                        WebClient wc = new WebClient();
                        var temp = wc.DownloadString(url);
                        log.WriteSingleLog("video.txt", string.Format("CheckStatus->云信发送关闭:{0}", temp));
                    }
                }
                else
                {
                    br.result = 1;
                    br.code = 1;
                    br.data = string.Format("{{\"status\": \"Run\",\"command\": \"{0}\",\"message\": \"{1}\"}}", "toRun", "OK");
                }
            }
            return br.ToFormatString();
        }



        /// <summary>
        /// 推流软件获取直播间接口 9072833
        /// </summary>
        /// <param name="usertoken"></param>
        /// <returns></returns>
        public string GetLiveMain(string usertoken)
        {
            Niu.Cabinet.Logging.LogRecord log = new Cabinet.Logging.LogRecord(Niu.Cabinet.Config.AppSetting.AppSettingString("logpath"));
            List<object> obj = new List<object>();
            obj.Add(new { liveid = 9072833, livename = "公共直播间", liveswitch = 1, livetype = 0, isvideo = 1 });
            VideoResult br = new VideoResult();
            br.result = 1;
            br.code = 0;
            br.data = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            log.WriteSingleLog("video.txt", string.Format("obs获取直播间相关 ，usertoken：{0}", br.ToFormatString()));

            return br.ToFormatString();
        }

        #region 回放地址

        public string replay()
        {
            Niu.Cabinet.Logging.LogRecord log = new Cabinet.Logging.LogRecord(Niu.Cabinet.Config.AppSetting.AppSettingString("logpath"));
            log.WriteSingleLog("replay.txt", "replay");
            var s = "";
            foreach (var item in HttpContext.Request.Params.AllKeys)
            {
                s += item + "=" + HttpContext.Request.Params.Get(item) + ";";
            }
            log.WriteSingleLog("replay.txt", string.Format("请求replay接口参数为:{0}------", s));
            var userid = HttpContext.Request.Params.Get("publish_id");

            var replay_url = HttpContext.Request.Params.Get("replay_url");
            var stream_alias = HttpContext.Request.Params.Get("stream_alias");
            var start = GetBjTimeByTimeStamp(double.Parse(HttpContext.Request.Params.Get("start")));
            var end = GetBjTimeByTimeStamp(double.Parse(HttpContext.Request.Params.Get("end")));
            var t = VideoList.UpdateReply(replay_url, end, start, stream_alias);
            log.WriteSingleLog("replay.txt", string.Format("更新数据结果:{0}------", t));
            return "1";
        }
        public static DateTime GetBjTimeByTimeStamp(double seconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(seconds).AddHours(8);
        }
        #endregion
    }
}