using Niu.Live.Video.DataAccess;
using Niu.Live.Video.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.BusinessLogic
{
    public class FreeVideo
    {
        public static readonly FreeVideo Instance = new FreeVideo();
        public IEnumerable<FreeVideoModel> GetAllByLiveId(long liveid)
        {
            return FreeVideoAccess.GetAllByLiveId(liveid);
        }

        public bool AddFreeVideo(FreeVideoModel freevideo)
        {
            return FreeVideoAccess.AddFreeVideo(freevideo);
        }

        public bool UpdateFreeVideo(FreeVideoModel freevideo)
        {
            return FreeVideoAccess.UpdateFreeVideo(freevideo);
        }
    }
}
