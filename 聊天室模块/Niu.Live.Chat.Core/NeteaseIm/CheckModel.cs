using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Niu.Live.Chat.Core.NeteaseIm
{
    public class CheckModel
    {

        public static string GetCheckSum(string appSecret, string nonce, string curTime)
        {
            string kv = string.Format("{0}{1}{2}", appSecret, nonce, curTime);
            return GetSHA1(kv).ToLower();
        }



        public static string GetNonce()
        {
            char[] chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            Random r = new Random(DateTime.Now.Millisecond);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                sb.Append(chars[r.Next(0, chars.Length)]);
            }
            return sb.ToString();
        }


        public static long GetUtcTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }


        public static string GetSHA1(string str)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] s = sha1.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                sb.Append(s[i].ToString("X2"));
            }
            return sb.ToString();
        }


    }
}