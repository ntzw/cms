using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Foundation.Application
{
    public static class SessionExtension
    {
        public static void SetObject<T>(this ISession session, string key, T t) where T : class
        {
            if (t != null && session.IsAvailable)
                session.SetString(key, JsonConvert.SerializeObject(t));
        }

        public static T GetObject<T>(this ISession session, string key) where T : class
        {
            try
            {
                if (!session.IsAvailable) return default;

                var value = session.GetString(key);
                return string.IsNullOrEmpty(value) ? default : JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public static List<T> GetListObject<T>(this ISession session, string key)
        {
            if (!session.IsAvailable) return default;

            var value = session.GetString(key);
            return string.IsNullOrEmpty(value) ? default : JsonConvert.DeserializeObject<List<T>>(value);
        }

        public static void SetObject<T>(this ISession session, string key, List<T> ids)
        {
            if (ids != null && session.IsAvailable) session.SetString(key, JsonConvert.SerializeObject(ids));
        }

        public static bool IsExistsValue(this ISession session, string key)
        {
            return session.IsAvailable && !string.IsNullOrEmpty(session.GetString(key));
        }
    }
}