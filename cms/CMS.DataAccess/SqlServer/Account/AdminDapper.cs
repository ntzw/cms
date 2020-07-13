using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.Account;
using Model.Account;

namespace DataAccess.SqlServer.Account
{
    public class AdminDapper : DefaultDataAccess<Administrator>, IAdminDapper
    {
        public Task<Administrator> GetByAccountName(string accountName)
        {
            string sql = $"SELECT * FROM {GetTableName()} WHERE AccountName = @AccountName";
            return Connection().QueryFirstOrDefaultAsync<Administrator>(sql, new {AccountName = accountName});
        }
    }
}