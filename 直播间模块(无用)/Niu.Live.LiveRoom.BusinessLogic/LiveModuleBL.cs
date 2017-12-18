using Niu.Live.LiveRoom.DataAccess;
using Niu.Live.LiveRoom.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.LiveRoom.BusinessLogic
{
    public class LiveModuleBL
    {
        public static readonly LiveModuleBL Instance = new LiveModuleBL();

        public IEnumerable<LiveModule> GetAllLiveModule()
        {
            return LiveModuleAccess.GetAllLiveModule();
        }

        public bool AddLiveModule(LiveModule livemodule)
        {
            return LiveModuleAccess.AddLiveModule(livemodule);
        }

        public bool UpdateLiveModule(LiveModule livemodule)
        {
            return LiveModuleAccess.UpdateLiveModule(livemodule);
        }
        public IEnumerable<LiveModule> FindOneLiveModule(long liveId)
        {
            return LiveModuleAccess.FindOneLiveModule(liveId);
        }
    }
}
