using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Chat.Core.Provider
{
    public enum ChatroomRole
    {
        Admin = 1,
        Normal = 2,
        Disabled = -1,
        Mute = -2
    }
}