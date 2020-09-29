/**
 * author：created by zw on 2020-06-20 14:28:18
 * email：ntzw.geek@gmail.com
 */

using CMS.Modules.Account.Abstractions.Interface.Dapper;
using CMS.Modules.Account.Abstractions.Model;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Account.DataAccess.MySql
{
    public class PermissionsDapper : DefaultDataAccess<Permissions>, IPermissionsDapper
    {
        public PermissionsDapper(IPageSqlHelper pageSqlHelper) : base(pageSqlHelper)
        {
        }
    }
}