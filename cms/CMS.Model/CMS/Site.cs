/**
 * author：created by zw on 2020-07-08 15:54:52
 * email：ntzw.geek@gmail.com
 */

using System.ComponentModel;
using CMS.React.Component;
using Dapper.Contrib.Extensions;

namespace Model.CMS
{
    /// <summary>
    /// 站点表
    /// </summary>
    [Table("CMS_Site")]
    public class Site : ModalBase
    {
        [Description("")] 
        [Input("站点名称", Required = true)] 
        public string Name { get; set; }

        [Description("")]
        [Select("站点域名", Mode = "tags")]
        public string Host { get; set; }

        [Description("")]
        [Switch("是否默认站点")]
        public bool IsDefault { get; set; }
    }
}