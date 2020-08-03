/**
 * author：created by zw on 2020-07-08 15:54:52
 * email：ntzw.geek@gmail.com
 */

using System.ComponentModel;
using CMS.Enums;
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
        /// <summary>
        /// 站点名称
        /// </summary>
        [Input("站点名称", Required = true)]
        public string Name { get; set; }

        /// <summary>
        /// 站点域名
        /// </summary>
        [Select("站点域名", Mode = "tags")]
        public string Host { get; set; }

        /// <summary>
        /// 是否默认站点
        /// </summary>
        [Switch("是否默认站点")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 站点文件夹
        /// </summary>
        [Input("站点文件夹", Required = true, RegularTypes = new[] {RegularType.FolderName})]
        public string SiteFolder { get; set; }
        
        /// <summary>
        /// Seo标题
        /// </summary>
        [Input("Seo标题")]
        public string SeoTitle { get; set; }
        
        /// <summary>
        /// Seo关键词
        /// </summary>
        [Input("Seo关键词")]
        public string SeoKeywords { get; set; }
        
        /// <summary>
        /// Seo描述
        /// </summary>
        [Input("Seo描述")]
        public string SeoDescription { get; set; }
    }
}