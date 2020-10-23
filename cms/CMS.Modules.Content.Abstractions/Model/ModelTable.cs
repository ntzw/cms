/**
 * author：created by zw on 2020-07-08 15:54:52
 * email：ntzw.geek@gmail.com
 */

using System.ComponentModel;
using CMS.Enums;
using CMS.React.Component;
using Dapper.Contrib.Extensions;
using Foundation.DataAccess;

namespace CMS.Modules.Content.Abstractions.Model
{
    [Table("CMS_ModelTable")]
    public class ModelTable : ModalBase
    {
        /// <summary>
        /// 表名(无前缀)
        /// </summary>
        [Description("")]
        [Input("表名", new[] {RegularType.SqlField }, Required = true, AddonBefore = "CMS_U_", UpdateDisabled = true)]
        public string TableName { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [Description("")]
        [Input("说明", Required = true)]
        public string Explain { get; set; }

        /// <summary>
        /// 表名(有前缀)
        /// </summary>
        [Computed]
        public string SqlTableName => $"CMS_U_{TableName}";

        /// <summary>
        /// 分类表名(有前缀)
        /// </summary>
        [Computed]
        public string SqlCategoryTableName => $"{SqlTableName}_Category";
    }
}