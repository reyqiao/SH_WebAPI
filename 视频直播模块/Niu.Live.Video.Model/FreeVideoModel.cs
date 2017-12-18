using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.Model
{
    public class FreeVideoModel
    {
        public long FreeId { get; set; }
        public long VideoId { get; set; }
        public long LiveId { get; set; }

        public VideoModel Video { get; set; }
    }
}
