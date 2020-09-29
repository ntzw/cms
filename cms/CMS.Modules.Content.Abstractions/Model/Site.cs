/**
 * author：created by zw on 2020-07-08 15:54:52
 * email：ntzw.geek@gmail.com
 */

using CMS.Enums;
using CMS.React.Component;
using Dapper.Contrib.Extensions;
using Foundation.DataAccess;

namespace CMS.Modules.Content.Abstractions.Model
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
        /// 是否启用手机站
        /// </summary>
        [Switch("是否启用手机站")]
        public bool IsEnableMobileSite { get; set; }
        
        /// <summary>
        /// 手机站文件夹
        /// </summary>
        [Input("手机站文件夹")]
        public string MobileSiteFolder { get; set; }
        
        /// <summary>
        /// 手机站域名
        /// </summary>
        [Select("手机站域名", Mode = "tags")]
        public string MobileHost { get; set; }
        
        /// <summary>
        /// 版权信息
        /// </summary>
        [Input("版权信息")]
        public string Copyright { get; set; }
        
        /// <summary>
        /// 备案号
        /// </summary>
        [Input("备案号")]
        public string RecordNo { get; set; }
        
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
        
        /// <summary>
        /// 是否手机站点
        /// </summary>
        [Computed]
        [Write(false)]
        public bool IsMobileSite { get; set; }
    }
}