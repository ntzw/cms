using System.Threading.Tasks;
using DataAccess.SqlServer.Account;
using Foundation.Result;
using Model.Account;

namespace Service.Account
{
    public class AdminService : DefaultService<Administrator, AdminDapper>
    {
        private readonly AdminDapper _dapper = new AdminDapper();

        protected override AdminDapper GetDapper() => _dapper;

        private AdminService()
        {
        }

        private static AdminService _interface;
        public static AdminService Interface => _interface ??= new AdminService();
    }
}