//
// author：created by zw on 2020-06-20 14:28:17
// email：ntzw.geek@gmail.com
//

using System.ComponentModel;
using CMS.React.Component;
using Dapper.Contrib.Extensions;

namespace Model.CMS
{
    [Table("CMS_Column")]
    public class Column : ModalBase
    {
        [Description("")]
        [Cascader("所属栏目", DataAction = "/Api/CMS/Column/CascaderData", ChangeOnSelect = true)]
        public string ParentNum { get; set; }

        [Description("")]
        [Input("名称", Required = true, AllowClear = true)]
        public string Name { get; set; }

        /// <summary>
        /// 模型编号
        /// </summary>
        [Select("所属模型", DataAction = "/Api/CMS/Model/SelectData", AllowClear = true)]
        public string ModelNum { get; set; }

        [Description("")] public string SiteNum { get; set; }

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