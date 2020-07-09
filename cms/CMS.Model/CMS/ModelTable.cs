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
    [Table("CMS_ModelTable")]
    public class ModelTable : ModalBase
    {
        [Description("")]
        [Input("表名", new[] {RegularType.SqlField }, Required = true, AddonBefore = "CMS_U_", UpdateDisabled = true)]
        public string TableName { get; set; }

        [Description("")]
        [Input("说明", Required = true)]
        public string Explain { get; set; }
    }
}