using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Chat.Core.Utils
{
    public class UniqueValue
    {
        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}