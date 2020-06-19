using DataAccess.Interface.Account;
using Model.Account;

namespace DataAccess.SqlServer.Account
{
    public class AdminDapper : DefaultDataAccess<Administrator>, IAdminDapper
    {
    }
}