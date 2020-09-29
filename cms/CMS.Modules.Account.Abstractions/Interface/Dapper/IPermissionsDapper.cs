/**
 * author：created by zw on 2020-06-20 14:28:20
 * email：ntzw.geek@gmail.com
 */

using CMS.Modules.Account.Abstractions.Model;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Account.Abstractions.Interface.Dapper
{
    public interface IPermissionsDapper : IDefaultDataAccess<Permissions>
    {
    }
}