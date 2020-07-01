/**
 * author：created by zw on 2020-06-20 14:28:19
 * email：ntzw.geek@gmail.com
 */

using DataAccess.Interface.Account;
using DataAccess.SqlServer.Account;
using Model.Account;

namespace Service.Account
{
    public class RoleService : DefaultService<Role, IRoleDapper>
    {
        private IRoleDapper _dapper;

        protected override IRoleDapper GetDapper() => _dapper ??= new RoleDapper();

        private RoleService()
        {
        }

        private static RoleService _interface;
        public static RoleService Interface => _interface ??= new RoleService();
    }
}