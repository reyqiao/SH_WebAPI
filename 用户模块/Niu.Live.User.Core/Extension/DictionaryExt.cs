using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.Core.Extension
{
    public static class DictionaryExt
    {
        #region 获取字典值

        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TReturn SafeGetValue<TKey, TValue, TReturn>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TReturn r = default(TReturn);
            TValue v = default(TValue);

            dict.TryGetValue(key, out v);

            if (v != null && !string.IsNullOrEmpty(v.ToString()))
            {
                return (TReturn)Convert.ChangeType(v, typeof(TReturn));
            }
            else
            {
                return r;
            }
        }

        #endregion
    }
}
