using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Foundation.DataAccess.Interface;

namespace Foundation.DataAccess
{
    public abstract class DynamicDataAccess<T> : DefaultDataAccess<T>
        where T : class, new()
    {
        public DynamicDataAccess(IPageSqlHelper pageSqlHelper) : base(pageSqlHelper)
        {
        }

        protected abstract IDbConnection Connection(string dbBaseName);

        public Task<T> GetById(string dbBaseName, int id)
        {
            return Connection(dbBaseName).GetAsync<T>(id);
        }

        public Task<T> GetByNum(string dbBaseName, string num)
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE Num = @Num";
            return Connection(dbBaseName).QueryFirstAsync<T>(sql, new {Num = num});
        }

        public Task<IEnumerable<T>> GetByNum(string dbBaseName, string[] num)
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE Num IN @Num";
            return Connection(dbBaseName).QueryAsync<T>(sql, new {Num = num});
        }

        public async Task<bool> Delete(string dbBaseName, string num)
        {
            var sql = $"DELETE FROM {GetTableName()} WHERE Num = @Num";
            var count = await Connection(dbBaseName).ExecuteAsync(sql, new {Num = num});
            return count > 0;
        }

        public async Task<bool> Delete(string dbBaseName, string[] num)
        {
            var sql = $"DELETE FROM {GetTableName()} WHERE Num IN @Num";
            var count = await Connection(dbBaseName).ExecuteAsync(sql, new {Num = num});
            return count > 0;
        }

        public Task<bool> Delete(string dbBaseName, T t)
        {
            return Connection(dbBaseName).DeleteAsync(t);
        }

        public Task<bool> Delete(string dbBaseName, List<T> t)
        {
            return Connection(dbBaseName).DeleteAsync(t);
        }

        public Task<int> Add(string dbBaseName, T t)
        {
            return Connection(dbBaseName).InsertAsync(t);
        }

        public Task<int> Add(string dbBaseName, List<T> t)
        {
            return Connection(dbBaseName).InsertAsync(t);
        }

        public Task<bool> Update(string dbBaseName, T t)
        {
            return Connection(dbBaseName).UpdateAsync(t);
        }

        public Task<bool> Update(string dbBaseName, List<T> t)
        {
            return Connection(dbBaseName).UpdateAsync(t);
        }
    }
}