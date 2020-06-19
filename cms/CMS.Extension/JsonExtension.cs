using System.Collections.Generic;
using Newtonsoft.Json;

namespace Extension
{
    public static class JsonExtension
    {
        public static string ToJson<T>(this T t, JsonSerializerSettings settings = null) where T : new()
        {
            return JsonConvert.SerializeObject(t, settings ?? new JsonSerializerSettings());
        }

        public static string ToJson<T>(this List<T> t) where T : new()
        {
            return JsonConvert.SerializeObject(t);
        }

        public static T ToObject<T>(this string json) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static List<T> ToArray<T>(this string json) where T : new()
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
    }
}