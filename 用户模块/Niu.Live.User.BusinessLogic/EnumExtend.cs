using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Niu.Live.User.BusinessLogic
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static class EnumExtend
    {
        #region 属性

        private static readonly Dictionary<System.Enum, string> enumDescriptions = new Dictionary<System.Enum, string>();
        private static readonly object syncRoot = new object();

        #endregion

        #region 获取枚举的标注

        /// <summary>
        /// 获取枚举的标注
        /// </summary>
        /// <param name="enumSubitem"></param>
        /// <returns></returns>
        public static string GetDescription(this System.Enum enumSubitem)
        {
            try
            {
                if (enumDescriptions.ContainsKey(enumSubitem))
                {
                    return enumDescriptions[enumSubitem];
                }
                lock (syncRoot)
                {
                    string name = enumSubitem.ToString();
                    object[] customAttributes = enumSubitem.GetType().GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        DescriptionAttribute attribute = (DescriptionAttribute)customAttributes[0];
                        name = attribute.Description;
                    }
                    enumDescriptions[enumSubitem] = name;
                    return name;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return string.Empty;
            }
        }

        #endregion
    }
}