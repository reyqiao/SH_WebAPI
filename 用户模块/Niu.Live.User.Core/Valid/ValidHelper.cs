using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.Core.Valid
{
    /// <summary>
    /// 验证工具
    /// </summary>
    public class ValidHelper
    {
        #region 判断是否是手机号码

        /// <summary>
        /// 判断是否是手机号码
        /// </summary>
        /// <param name="str">测试字符串</param>
        /// <returns>true 是 false 不就</returns>
        public static bool isMobile(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^((13|15|18)\d{9})|((14[57]|17[0135678])\d{8})$");

            return reg.IsMatch(str);
        }

        #endregion

        #region 判断是否是整数

        /// <summary>
        /// 判断是否是整数
        /// </summary>
        /// <param name="str">测试字符串</param>
        /// <returns>true/false</returns>
        public static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg
                = new System.Text.RegularExpressions.Regex(@"^-?\d+$");
            return reg.IsMatch(str);
        }

        #endregion
    }
}
