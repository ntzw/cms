//
// author：created by zw on 2020-06-20 14:28:17
// email：ntzw.geek@gmail.com
//

using CMS.React.Component;
using Dapper.Contrib.Extensions;
using Foundation.DataAccess;

namespace CMS.Modules.Content.Abstractions.Model
{
    [Table("CMS_Column")]
    public class Column : ModalBase
    {
        /// <summary>
        /// 站点编号
        /// </summary>
        public string SiteNum { get; set; }

        /// <summary>
        /// 所属栏目
        /// </summary>
        [Cascader("所属栏目", DataAction = "/Api/CMS/Column/CascaderData", ChangeOnSelect = true)]
        public string ParentNum { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Input("名称", Required = true, AllowClear = true)]
        public string Name { get; set; }

        /// <summary>
        /// 模型编号
        /// </summary>
        [Select("所属模型", DataAction = "/Api/CMS/Model/SelectData", AllowClear = true)]
        public string ModelNum { get; set; }

        /// <summary>
        /// 列表页模板
        /// </summary>
        [Select("列表页模板", DataAction = "/Api/CMS/Site/TemplatePath", AllowClear = true)]
        public string ListTemplatePath { get; set; }

        /// <summary>
        /// 详情页模板
        /// </summary>
        [Select("详情页模板", DataAction = "/Api/CMS/Site/TemplatePath", AllowClear = true)]
        public string InfoTemplatePath { get; set; }

        /// <summary>
        /// 是否启用分类
        /// </summary>
        [Switch("是否启用分类")]
        public bool IsCategory { get; set; }

        /// <summary>
        /// 是否单篇
        /// </summary>
        [Switch("是否单篇")]
        public bool IsSingle { get; set; }

        /// <summary>
        /// 是否开启内容置顶功能
        /// </summary>
        [Switch("开启置顶", CheckedChildren = "已开启", UnCheckedChildren = "未开启")]
        public bool IsAllowTop { get; set; }

        /// <summary>
        /// 开启回收站功能
        /// </summary>
        [Switch("开启回收站", CheckedChildren = "已开启", UnCheckedChildren = "未开启", Extra = "关闭回收站功能，数据已经删除将不可恢复，请谨慎操作")]
        public bool IsAllowRecycle { get; set; }

        /// <summary>
        /// 是否启用Seo
        /// </summary>
        [Switch("是否启用SEO")]
        public bool IsSeo { get; set; }

        /// <summary>
        /// Seo标题
        /// </summary>
        [Input("Seo标题")]
        public string SeoTitle { get; set; }

        /// <summary>
        /// Seo关键词
        /// </summary>
        [Input("Seo关键词")]
        public string SeoKeyword { get; set; }

        /// <summary>
        /// Seo描述
        /// </summary>
        [Input("Seo描述")]
        public string SeoDesc { get; set; }
    }
}