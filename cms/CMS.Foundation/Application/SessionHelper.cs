using Extension;
using Helper;
using Microsoft.AspNetCore.Http;

namespace Foundation.Application
{
    public class SessionHelper
    {
        public static T Get<T>(string key) where T : class
        {
            return GlobalApplication.ContextAccessor.HttpContext?.Session?.GetObject<T>(key);
        }

        public static string Get(string key)
        {
            return GlobalApplication.ContextAccessor.HttpContext?.Session?.GetString(key);
        }

        public static void Set<T>(string key, T t) where T : class
        {
            GlobalApplication.ContextAccessor.HttpContext?.Session?.SetObject(key, t);
        }

        public static void Set(string key, string val)
        {
            GlobalApplication.ContextAccessor.HttpContext?.Session?.SetString(key, val);
        }

        public static void Remove(string key)
        {
            GlobalApplication.ContextAccessor.HttpContext?.Session?.Remove(key);
        }
    }
}