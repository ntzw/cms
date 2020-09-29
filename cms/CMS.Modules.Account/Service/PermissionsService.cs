/**
 * author：created by zw on 2020-06-20 14:28:19
 * email：ntzw.geek@gmail.com
 */

using CMS.Modules.Account.Abstractions.Interface.Dapper;
using CMS.Modules.Account.Abstractions.Interface.Service;
using CMS.Modules.Account.Abstractions.Model;
using Foundation.DataAccess;

namespace Service.Account
{
    public class PermissionsService : DefaultService<Permissions, IPermissionsDapper>, IPermissionsService
    {
        private readonly IPermissionsDapper _dapper;
        protected override IPermissionsDapper GetDapper() => _dapper;

        public PermissionsService(IPermissionsDapper dapper)
        {
            _dapper = dapper;
        }
    }
}