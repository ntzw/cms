using System;
using System.Collections.Generic;
using System.Linq;
using Extension;
using Foundation.Application;
using Helper;

namespace Foundation.Utils
{
    public class UploadImageUtil : UploadUtil, IUploadUtil
    {
        protected override bool ExistWhite(string ext)
        {
            string whitelist = ConfigHelper.GetAppSetting("whitelist:uploadimage");
            if (whitelist.IsEmpty()) return true;

            List<string> white = whitelist.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            return white.IndexOf(ext.ToLower().TrimStart('.')) > -1 && ExistCurrentWhiteList(ext);
        }

        private bool ExistCurrentWhiteList(string ext)
        {
            return CurrentWhiteList == null ||
                   CurrentWhiteList.Count <= 0 ||
                   CurrentWhiteList.Exists(item => string.Equals(item.ToLower().TrimStart('.'),
                       ext.ToLower().TrimStart('.'), StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 自定义白名单
        /// </summary>
        public List<string> CurrentWhiteList { get; set; }
    }
}