using Niu.Live.LiveRoom.DataAccess;
using Niu.Live.LiveRoom.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.LiveRoom.BusinessLogic
{
    public class RoomBL
    {
        public static readonly RoomBL Instance = new RoomBL();
        public LiveRoomModel GetLiveRoom(long liveid)
        {
            return LiveRoomAccess.GetLiveRoom(liveid) ?? new LiveRoomModel();
        }

        public bool AddLiveRoom(LiveRoomModel room)
        {
            return LiveRoomAccess.AddLiveRoom(room);
        }

        public bool UpdateLiveRoom(LiveRoomModel room)
        {
            return LiveRoomAccess.UpdateLiveRoom(room);
        }

        public List<LiveRoomModel> GetLiveRoomsByUserid(long userid)
        {
            return new List<LiveRoomModel>() { new LiveRoomModel() { LiveId = 1, Title = "111" }, new LiveRoomModel() { LiveId = 2, Title = "222" } };
        }
        public static LiveRoomSetting FindOneSetting(long liveId)
        {
            return LiveRoomAccess.FindOneSetting(liveId) ?? new LiveRoomSetting();
        }
        public static bool InsertRoomSetting(LiveRoomSetting model)
        {
            return LiveRoomAccess.InsertRoomSetting(model);
        }
        public static bool UpdateRoomSetting(LiveRoomSetting model)
        {
            return LiveRoomAccess.UpdateRoomSetting(model);
        }
        #region 直播间admin
        public static bool AddAdmin(LiveRoomAdmin model)
        {
            return LiveRoomAccess.AddAdmin(model);
        }
        public static bool IsAdmin(dynamic model)
        {
            return LiveRoomAccess.IsAdmin(model);
        }
        public static IEnumerable<LiveRoomAdmin> GetRoomAdminList(long liveId)
        {
            return LiveRoomAccess.GetRoomAdminList(liveId);
        }
        #endregion

    }
}
