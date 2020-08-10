using Helper;

namespace Foundation.Application
{
    public class ConfigHelper
    {
        public static string GetAppSetting(string path)
        {
            if (GlobalApplication.Configuration == null) return "";
            return GlobalApplication.Configuration[path];
        }
    }
}