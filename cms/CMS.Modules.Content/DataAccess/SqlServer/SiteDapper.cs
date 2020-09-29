/**
 * author：created by zw on 2020-07-08 15:54:53
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Model;
using Dapper;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Content.DataAccess.SqlServer
{
    public class SiteDapper : DefaultDataAccess<Site>, ISiteDapper
    {
        public SiteDapper(IPageSqlHelper pageSqlHelper) : base(pageSqlHelper)
        {
        }

        public Task<int> RemoveOtherDefault(int excludeId)
        {
            var sql = $"UPDATE [{GetTableName()}] SET [IsDefault] = 0 WHERE Id <> @Id ";
            return Connection().ExecuteAsync(sql, new {Id = excludeId});
        }

        public Task<Site> GetByHost(string host)
        {
            var sql =
                $"SELECT * FROM {GetTableName()} WHERE ',' + Host + ',' LIKE @Host OR ',' + MobileHost + ',' LIKE @Host";
            return Connection().QueryFirstOrDefaultAsync<Site>(sql, new {Host = $"%,{host},%"});
        }

        public Task<Site> GetByDefault()
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE IsDefault = 1";
            return Connection().QueryFirstOrDefaultAsync<Site>(sql);
        }
    }
}