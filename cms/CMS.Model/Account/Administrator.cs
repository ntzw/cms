/**
 * author：created by zw on 2020-06-20 14:28:17
 * email：ntzw.geek@gmail.com
 */

using System.ComponentModel;
using CMS.React.Component;
using Dapper.Contrib.Extensions;

namespace Model.Account
{
    [Table("Account_Administrator")]
    public class Administrator : ModalBase
    {
        /// <summary>
        /// 账户名称
        /// </summary>
        [Description("")]
        [Input("账户名称", Required = true)]
        public string AccountName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Input("真实姓名")]
        public string TrueName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Description("")]
        [Password("密码", Required = true)]
        public string Password { get; set; }

        [Description("")] public string PermissionsCode { get; set; }

        [Description("")] public string GroupNum { get; set; }
    }
}