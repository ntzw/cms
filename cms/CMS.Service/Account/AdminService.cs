using DataAccess.Interface.Account;
using DataAccess.SqlServer.Account;
using Model.Account;

namespace Service.Account
{
    public class AdminService : DefaultService<Administrator, IAdminDapper>
    {
        private AdminDapper _dapper;

        protected override IAdminDapper GetDapper() => _dapper ??= new AdminDapper();

        private AdminService()
        {
        }

        private static AdminService _interface;
        public static AdminService Interface => _interface ??= new AdminService();
    }
}