using System;
using System.Linq;

namespace Extension
{
    public static class ObjectExtension
    {
        public static T GetAttribute<T>(this object o, bool inherit = false) where T : class
        {
            return o.GetType().GetCustomAttributes(inherit).FirstOrDefault(item => item.GetType() == typeof(T)) as T;
        }
        
        /// <summary>
        ///     重写ToString()的方法，该方法首先判断object是否为空，如果对象为空则返回string.Empty，否则返回该对象的字符串表现形式
        /// </summary>
        /// <param name="obj">需要转换的对象</param>
        /// <returns>字符串</returns>
        public static string ToStr(this object obj)
        {
            return obj == null || obj is DBNull ? String.Empty : obj.ToString();
        }
    }
}