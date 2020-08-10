using System;
using System.Collections.Generic;
using CMS.Enums;
using Helper;
using Microsoft.Extensions.Caching.Memory;

namespace Foundation.Application
{
    public static class CacheHelper
    {
        /// <summary>
        ///     设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="saveDay">缓存天数（默认缓存一天）</param>
        public static void Set(string key, string value, int saveDay = 1)
        {
            SetObject(key, value, saveDay);
        }

        public static void Set<T>(string key, T t, int saveDay = 1) where T : new()
        {
            SetObject(key, t, saveDay);
        }

        public static void Set<T>(string key, List<T> t, int saveDay = 1) where T : new()
        {
            SetObject(key, t, saveDay);
        }

        public static void SetObject(string key, object obj, int saveDay = 1)
        {
            GlobalApplication.Cache.Set(key, obj,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddDays(saveDay)));
        }

        public static string Get(string key)
        {
            return GetObject(key) as string;
        }

        public static object GetObject(string key)
        {
            return GlobalApplication.Cache.Get(key);
        }

        public static T Get<T>(string key) where T : class, new()
        {
            return GetObject(key) as T;
        }

        public static List<T> GetArray<T>(string key) where T : class, new()
        {
            return GetObject(key) as List<T>;
        }

        public static void Remove(string key)
        {
            GlobalApplication.Cache.Remove(key);
        }

        public static string GetKey(CacheType type, params string[] ps)
        {
            return type + "_" + string.Join("_", ps);
        }

        public static string AdminPowerKey(string adminNum)
        {
            return GetKey(CacheType.AdminAuth, adminNum);
        }
    }
}