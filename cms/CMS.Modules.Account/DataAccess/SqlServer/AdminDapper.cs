using System.Threading.Tasks;
using CMS.Modules.Account.Abstractions.Interface.Dapper;
using CMS.Modules.Account.Abstractions.Model;
using Dapper;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Account.DataAccess.SqlServer
{
    public class AdminDapper : DefaultDataAccess<Administrator>, IAdminDapper
    {
        public AdminDapper(IPageSqlHelper pageSqlHelper) : base(pageSqlHelper)
        {
        }

        public Task<Administrator> GetByAccountName(string accountName)
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE AccountName = @AccountName";
            return Connection().QueryFirstOrDefaultAsync<Administrator>(sql, new {AccountName = accountName});
        }
    }
}