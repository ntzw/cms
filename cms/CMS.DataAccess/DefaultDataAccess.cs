using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Extension;

namespace DataAccess
{
    public abstract class DefaultDataAccess<T> where T : class, new()
    {
        protected abstract IDbConnection Connection();

        public Task<T> GetById(int id)
        {
            return Connection().GetAsync<T>(id);
        }

        public Task<T> GetByNum(string num)
        {
            string sql = $"SELECT * FROM {GetTableName()} WHERE Num = @Num";
            return Connection().QueryFirstAsync<T>(sql, new {Num = num});
        }

        public Task<IEnumerable<T>> GetByNum(string[] num)
        {
            string sql = $"SELECT * FROM {GetTableName()} WHERE Num IN @Num";
            return Connection().QueryAsync<T>(sql, new {Num = num});
        }

        public async Task<bool> Delete(string num)
        {
            string sql = $"DELETE FROM {GetTableName()} WHERE Num = @Num";
            int count = await Connection().ExecuteAsync(sql, new {Num = num});
            return count > 0;
        }

        public async Task<bool> Delete(string[] num)
        {
            string sql = $"DELETE FROM {GetTableName()} WHERE Num IN @Num";
            int count = await Connection().ExecuteAsync(sql, new {Num = num});
            return count > 0;
        }

        public Task<bool> Delete(T t)
        {
            return Connection().DeleteAsync(t);
        }

        public Task<bool> Delete(List<T> t)
        {
            return Connection().DeleteAsync(t);
        }

        public Task<int> Add(T t)
        {
            return Connection().InsertAsync(t);
        }

        public Task<int> Add(List<T> t)
        {
            return Connection().InsertAsync(t);
        }

        public Task<bool> Update(T t)
        {
            return Connection().UpdateAsync(t);
        }

        public Task<bool> Update(List<T> t)
        {
            return Connection().UpdateAsync(t);
        }

        protected static string GetTableName()
        {
            return new T().GetAttribute<TableAttribute>()?.Name;
        }
    }
}