using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Niu.Live.Video.Core.Utils
{
    public class JsonSerialize
    {
        public static dynamic Deserialize(string json)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            return serializer.Deserialize<object>(json);
        }
    }
}
