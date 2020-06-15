using System.Linq;

namespace Extension
{
    public static class ObjectExtension
    {
        public static T GetAttribute<T>(this object o, bool inherit = false) where T : class
        {
            return o.GetType().GetCustomAttributes(inherit).FirstOrDefault(item => item.GetType() == typeof(T)) as T;
        }
    }
}