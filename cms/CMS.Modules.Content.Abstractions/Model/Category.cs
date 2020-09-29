using CMS.React.Component;
using Foundation.DataAccess;

namespace CMS.Modules.Content.Abstractions.Model
{
    public class Category : ModalBase
    {
        public bool IsDel { get; set; }

        public int Status { get; set; }

        public string SiteNum { get; set; }

        /// <summary>
        /// 所属栏目
        /// </summary>
        public string ColumnNum { get; set; }

        /// <summary>
        /// 上级类别
        /// </summary>
        [Cascader("上级类别", DataAction = "/Api/CMS/Category/CascaderData", ChangeOnSelect = true)]
        public string ParentNum { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Input("名称", Required = true)]
        public string Name { get; set; }
    }
}