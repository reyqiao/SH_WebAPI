using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.Core.String
{
    /// <summary>
    /// 字符串处理类
    /// </summary>
    public class StringHelper
    {
        #region 生成指定长度的随机字符串

        /// <summary>
        /// 生成指定长度的随机字符串
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string RandomString(int len)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[len];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[Random() % chars.Length];
            }

            var finalString = new System.String(stringChars);
            return finalString;
        }

        #endregion

        #region 返回非负随机数

        /// <summary>
        /// 返回非负随机数（多线程下也严格随机）
        /// </summary>
        /// <returns></returns>
        public static int Random()
        {
            return RandomGen3.Next();
        }

        #endregion

        #region 随机生成类

        /// <summary>
        /// 随机生成类
        /// </summary>
        private static class RandomGen3
        {
            private static RNGCryptoServiceProvider _global = new RNGCryptoServiceProvider();

            [ThreadStatic]
            private static Random _local;

            public static int Next()
            {
                Random inst = _local;
                if (inst == null)
                {
                    byte[] buffer = new byte[4];
                    _global.GetBytes(buffer);
                    _local = inst = new Random(BitConverter.ToInt32(buffer, 0));
                }
                return inst.Next();
            }

            public static int Next(int min, int max)
            {
                Random inst = _local;
                if (inst == null)
                {
                    byte[] buffer = new byte[4];
                    _global.GetBytes(buffer);
                    _local = inst = new Random(BitConverter.ToInt32(buffer, 0));
                }
                return inst.Next(min, max);
            }
        }

        #endregion
    }
}
