using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.LiveRoom.Model
{
    public class LiveModule
    {
        public long ModuleId { get; set; }
        public long LiveId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleUrl { get; set; }
        public string ModuleGoUrl { get; set; }
        public int ModuleWeight { get; set; }
        public int Display { get; set; }
        public int Moduletype { get; set; }//模块类型

        public LiveModule()
        {
            this.ModuleId = 0;
            this.LiveId = 0;
            this.ModuleName = string.Empty;
            this.ModuleUrl = string.Empty;
            this.ModuleGoUrl = string.Empty;
            this.ModuleWeight = 0;
            this.Moduletype = 1;
            this.Display = 1;
        }
    }
}
