/**
 * author：created by zw on 2020-06-20 14:28:17
 * email：ntzw.geek@gmail.com
 */

using System.ComponentModel;
using Dapper.Contrib.Extensions;
using Foundation.DataAccess;

namespace CMS.Modules.Account.Abstractions.Model
{
    [Table("Account_Permissions")]
    public class Permissions : ModalBase
    {
        [Description("")] public string Title { get; set; }

        [Description("")] public string Code { get; set; }

        [Description("")] public string ParentNum { get; set; }
    }
}