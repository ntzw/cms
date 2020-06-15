using System.Data;
using DataAccess.Interface.Account;
using Foundation.DataAccess.Connections;
using Model.Account;

namespace DataAccess.SqlServer.Account
{
    public class AdminDapper : DefaultDataAccess<Administrator>, IAdminDapper
    {
        protected override IDbConnection Connection()
        {
            return MainConnection.Interface.GetConnection();
        }
    }
}