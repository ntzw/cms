/**
 * author：created by zw on 2020-06-20 14:28:17
 * email：ntzw.geek@gmail.com
 */

using CMS.React.Component;
using Dapper.Contrib.Extensions;
using Foundation.DataAccess;

namespace CMS.Modules.Account.Abstractions.Model
{
    [Table("Account_Role")]
    public class Role : ModalBase
    {
        /// <summary>
        ///     所属角色
        /// </summary>
        [Cascader("所属角色", DataAction = "/Api/Account/Role/CascaderData", ChangeOnSelect = true)]
        public string ParentNum { get; set; }

        /// <summary>
        ///     名称
        /// </summary>
        [Input("名称", Required = true)]
        public string Name { get; set; }

        /// <summary>
        ///     描述
        /// </summary>
        [TextArea("描述")]
        public string Desc { get; set; }
    }
}