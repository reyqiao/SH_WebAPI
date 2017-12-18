using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Niu.Cabinet.Cryptography;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Niu.Live.User.Core.Cryptogram
{
    public class Cryptogram
    {
        private static string desKey = "niugu123niugu456";
        private static string desIV = "12312300";

        public static bool Verify(string json, string sign)
        {
            string md5 = MD5Service.Create(json);
            return (md5 == sign);
        }

        //
        public static byte[] HexStringToBytes(string value)
        {
            SoapHexBinary shb = SoapHexBinary.Parse(value);
            return shb.Value;
        }

        //
        public static string BytesToHexString(byte[] value)
        {
            SoapHexBinary shb = new SoapHexBinary(value);
            return shb.ToString();
        }

        public static string EncodeParam(string jsonString)
        {
            return EncodeParam(jsonString, desKey, desIV);
        }

        public static string EncodeParam(string jsonString, string desKey, string desIV)
        {
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonString);
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;

                tdes.Key = Encoding.UTF8.GetBytes(desKey);
                tdes.IV = Encoding.UTF8.GetBytes(desIV);

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);
                return BytesToHexString(resultArray);
            }
        }

        /// <summary>
        /// 根据Hex字符串，DES解密出json格式的参数
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="desKey"></param>
        /// <returns></returns>
        public static string DecodeParam(string hexString, string desKey, string desIV)
        {

            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                byte[] data = HexStringToBytes(hexString);

                tdes.Key = Encoding.UTF8.GetBytes(desKey);
                tdes.IV = Encoding.UTF8.GetBytes(desIV);

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
        }

        public static string DecodeParam(string hexString)
        {
            return DecodeParam(hexString, desKey, desIV);
        }
    }
}