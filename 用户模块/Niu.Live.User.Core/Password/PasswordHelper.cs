using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Niu.Live.User.Core.Password
{
    /// <summary>
    /// 密码处理类
    /// </summary>
    public class PasswordHelper
    {
        #region 取ASCII码表范围是33-126

        /// <summary>
        /// 取ASCII码表范围是33-126，具体代码如下：
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CheckPassword(string password, out char errorChar)
        {
            bool result = true;
            errorChar = new char();
            for (int i = 0; i < password.Length; i++)
            {
                if ((int)password[i] < 33 && (int)password[i] > 126)
                {
                    errorChar = password[i];
                    result = false;
                    break;
                }
            }
            return result;
        }

        #endregion

        #region 密码检查

        /// <summary>
        /// 密码为6-16个英文,数字或符号，不能是纯数字
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CheckPassword(string password)
        {
            if (password.Length < 6 || password.Length > 16) return false;

            bool result = true;
            for (int i = 0; i < password.Length; i++)
            {
                if ((int)password[i] < 33 || (int)password[i] > 126)
                {
                    result = false;
                    break;
                }
            }

            int intpwd = 0;
            if (int.TryParse(password, out intpwd))
            {
                result = false;
            }
            return result;
        }

        #endregion

        #region 检查密码等级

        /// <summary>
        /// 检查密码等级
        /// </summary>
        /// <param name="password"></param>
        /// <param name="securityLevel"></param>
        /// <returns></returns>
        public static bool CheckPassword(string password, out int securityLevel)
        {
            securityLevel = 0;
            if (password.Length < 6 || password.Length > 16) return false;
            int[] levelarr = new int[4];
            bool result = true;

            for (int i = 0; i < password.Length; i++)
            {
                int intchar = (int)password[i];
                if (intchar < 33 || intchar > 126)
                {
                    result = false;
                    break;
                }
                else
                {
                    if (intchar > 47 || intchar < 58)
                    {
                        levelarr[0] = 1;
                    }
                    else if (intchar > 64 || intchar < 91)
                    {
                        levelarr[1] = 1;
                    }
                    else if (intchar > 96 || intchar < 123)
                    {
                        levelarr[2] = 1;
                    }
                    else
                    {
                        levelarr[3] = 1;
                    }
                }
            }

            int intpwd = 0;
            if (int.TryParse(password, out intpwd))
            {
                result = false;
            }
            if (result == true)
            {
                for (int i = 0; i < levelarr.Length; i++)
                {
                    securityLevel += levelarr[i];
                }
                securityLevel = securityLevel == 4 ? 3 : securityLevel;
            }
            return result;
        }

        /// <summary>
        /// 获取密码等级
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static int GetPasswordLevel(string password)
        {
            int securityLevel = 0;
            int[] levelArray = new int[4];

            for (int i = 0; i < password.Length; i++)
            {
                int intChar = (int)password[i];
                if (intChar > 47 && intChar < 58)
                {
                    levelArray[0] = 1;//数字
                }
                else if (intChar > 64 && intChar < 91)
                {
                    levelArray[1] = 1;//大写
                }
                else if (intChar > 96 && intChar < 123)
                {
                    levelArray[2] = 1;//小写
                }
                else
                {
                    levelArray[3] = 1;//符号
                }
            }

            for (int i = 0; i < levelArray.Length; i++)
            {
                securityLevel += levelArray[i];
            }
            securityLevel = securityLevel == 4 ? 3 : securityLevel;
            return securityLevel;
        }

        #endregion

        #region 字符串MD5加密

        /// <summary>
        /// 字符串MD5加密，返回大写字母的结果。
        /// </summary>
        /// <param name="str">输入值</param>
        /// <returns>返回大写字母MD5加密串</returns>
        public static string GetMD5(string str)
        {
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                sb.Append(s[i].ToString("X2"));//ToString("x");
            }
            return sb.ToString();
        }

        #endregion
    }
}
