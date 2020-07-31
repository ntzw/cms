/**
 * author：created by zw on 2020-07-08 15:54:53
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.CMS;
using Foundation.Modal;
using Model.CMS;

namespace DataAccess.SqlServer.CMS
{
    public class SiteDapper : DefaultDataAccess<Site>, ISiteDapper
    {
        public Task<int> RemoveOtherDefault(int excludeId)
        {
            string sql = $"UPDATE [{GetTableName()}] SET [IsDefault] = 0 WHERE Id <> @Id ";
            return Connection().ExecuteAsync(sql, new {Id = excludeId});
        }

        public Task<Site> GetByHost(string host)
        {
            string sql = $"SELECT * FROM {GetTableName()} WHERE ',' + Host + ',' LIKE @Host ";
            return Connection().QueryFirstOrDefaultAsync<Site>(sql, new {Host = $"%{host}%"});
        }

        public Task<Site> GetByDefault()
        {
            string sql = $"SELECT * FROM {GetTableName()} WHERE IsDefault = 1";
            return Connection().QueryFirstOrDefaultAsync<Site>(sql);
        }
    }
}