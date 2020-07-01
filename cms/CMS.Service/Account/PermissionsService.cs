/**
 * author：created by zw on 2020-06-20 14:28:19
 * email：ntzw.geek@gmail.com
 */

using DataAccess.Interface.Account;
using DataAccess.SqlServer.Account;
using Model.Account;

namespace Service.Account
{
    public class PermissionsService : DefaultService<Permissions, IPermissionsDapper>
    {
        private IPermissionsDapper _dapper;

        protected override IPermissionsDapper GetDapper() => _dapper ??= new PermissionsDapper();

        private PermissionsService()
        {
        }

        private static PermissionsService _interface;
        public static PermissionsService Interface => _interface ??= new PermissionsService();
    }
}