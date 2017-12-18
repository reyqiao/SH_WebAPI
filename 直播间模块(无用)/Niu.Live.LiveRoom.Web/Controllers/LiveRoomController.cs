using Niu.Live.LiveRoom.BusinessLogic;
using Niu.Live.LiveRoom.Model;
using Niu.Live.LiveRoom.Utils;
using Niu.Live.LiveRoom.Web.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Niu.Live.LiveRoom.Web.Controllers
{

    [JsonFilter]
    public class LiveRoomController : BaseController
    {
        public string GetMyRoom(string usertoken)
        {
            var rooms = RoomBL.Instance.GetLiveRoomsByUserid(0);
            return JsonHelp.CamelCaseSerializeObject(new { code = 0, data = rooms });
        }

        public string GetRoom(string usertoken, long liveid = 0)
        {
            var room = RoomBL.Instance.GetLiveRoom(liveid);
            return JsonHelp.CamelCaseSerializeObject(new { code = 0, data = room });
        }

        [HttpPost]
        public string AddRoom(string usertoken, LiveRoomModel room)
        {
            room.AccId = userinfo.accId;
            room.UserId = userinfo.userId;
            if (RoomBL.Instance.AddLiveRoom(room))
                return JsonHelp.CamelCaseSerializeObject(new { code = 0 });
            else
                return JsonHelp.CamelCaseSerializeObject(new { code = -1 });
        }
        [HttpPost]
        public string UpdateRoom(string usertoken, LiveRoomModel room)
        {
            if (RoomBL.Instance.UpdateLiveRoom(room))
                return JsonHelp.CamelCaseSerializeObject(new { code = 0 });
            else
                return JsonHelp.CamelCaseSerializeObject(new { code = -1 });
        }
        [HttpPost]
        public string UploadLogo(string usertoken)
        {
            return JsonHelp.CamelCaseSerializeObject(new { code = 0, data = "" });
        }
        [HttpPost]
        public string AddSetting(LiveRoomSetting setting)
        {
            if (setting.Marking != null)
            {
                if (setting.Marking.GetType().IsArray)
                {
                    string temp = string.Empty;
                    foreach (var item in setting.Marking as Array)
                    {
                        temp += item;
                    }
                    setting.Marking = temp;
                    if (RoomBL.InsertRoomSetting(setting))
                        return JsonHelp.CamelCaseSerializeObject(new { code = 0 });
                    else
                        return JsonHelp.CamelCaseSerializeObject(new { code = -1 });
                }
            }
            return JsonHelp.CamelCaseSerializeObject(new { code = -1, msg = "参数不完整" });

        }
        [HttpPost]
        public string UpSetting(LiveRoomSetting setting)
        {
            if (setting.Marking != null)
            {
                if (setting.Marking.GetType().IsArray)
                {
                    string temp = string.Empty;
                    foreach (var item in setting.Marking as Array)
                    {
                        temp += item;
                    }
                    setting.Marking = temp;
                }
            }
            if (RoomBL.UpdateRoomSetting(setting))
                return JsonHelp.CamelCaseSerializeObject(new { code = 0 });
            else
                return JsonHelp.CamelCaseSerializeObject(new { code = -1 });
        }
        public string GetSetting(long Id)
        {
            var room = RoomBL.FindOneSetting(Id);
            return JsonHelp.CamelCaseSerializeObject(new { code = 0, data = room });
        }
        #region 前端接口
        /// <summary>
        /// 取直播间数据
        /// </summary>
        /// <param name="usertoken"></param>
        /// <param name="liveid"></param>
        /// <returns></returns>
        public JsonResult GetRoomInfo(long id = 0)
        {
            if (id > 0)
            {
                var room = RoomBL.Instance.GetLiveRoom(id);
                var module = LiveModuleBL.Instance.FindOneLiveModule(id).GroupBy(m => m.Moduletype);
                var set = RoomBL.FindOneSetting(id);
                set.Marking = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerQQ>>(set.Marking.ToString());
                return Json(new { code = 0, data = room, module = module, setting = set }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { code = -1, msg = "liveId无效" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoomAdmin(long Id)
        {
            if (Id > 0)
            {
                var room = RoomBL.Instance.GetLiveRoom(Id);

                var module = LiveModuleBL.Instance.FindOneLiveModule(Id).GroupBy(m => m.Moduletype);
                var set = RoomBL.FindOneSetting(Id);
                set.Marking = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerQQ>>(set.Marking.ToString());
                return Json(new { code = 0, data = room, module = module, setting = set }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { code = -1, msg = "liveId无效" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddRoomAdmin(LiveRoomAdmin model)
        {
            if (RoomBL.AddAdmin(model))
                return Json(new { code = 0 }, JsonRequestBehavior.AllowGet);
            return Json(new { code = -1 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsRoomAdmin(string accid, long liveid)
        {
            if (RoomBL.IsAdmin(new { ACCID = accid, LIVEID = liveid }))
                return Json(new { code = 0 }, JsonRequestBehavior.AllowGet);
            return Json(new { code = -1 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoomAdminList(long Id)
        {
            if (Id > 0)
                return Json(new { code = 0, data = RoomBL.GetRoomAdminList(Id) }, JsonRequestBehavior.AllowGet);
            return Json(new { code = -1 }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}