using Extension;
using Newtonsoft.Json;

namespace Foundation.ColumnFieldOptions
{
    public class ColumnFieldOptionsHelper
    {
        public static T Parsing<T>(string options) where T : class
        {
            if (options.IsEmpty()) return null;
            return JsonConvert.DeserializeObject<T>(options);
        }
    }
}