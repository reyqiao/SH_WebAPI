using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.LiveRoom.Utils
{
    public class JsonHelp
    {
        public static string CamelCaseSerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings() { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
        }
    }
}
