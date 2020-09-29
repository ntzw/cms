using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Model;
using Dapper;
using Foundation.DataAccess.Connections;

namespace CMS.Modules.Content.DataAccess.SqlServer
{
    public class CategoryDapper : ICategoryDapper
    {
        public Task<IEnumerable<Category>> GetByColumnNum(string columnNum, string tableName)
        {
            var sql = $"SELECT * FROM [{tableName}] WHERE ColumnNum = @ColumnNum";
            return Connection().QueryAsync<Category>(sql, new {ColumnNum = columnNum});
        }

        public Task<Category> GetById(string tableName, int id)
        {
            var sql = $"SELECT * FROM [{tableName}] WHERE Id = @Id ";
            return Connection().QueryFirstOrDefaultAsync<Category>(sql, new {Id = id});
        }

        public Task<Category> GetByNum(string tableName, string num)
        {
            var sql = $"SELECT * FROM [{tableName}] WHERE Num = @Num";
            return Connection().QueryFirstOrDefaultAsync<Category>(sql, new {Num = num});
        }

        public Task<int> Execute(string sql, ExpandoObject value)
        {
            return Connection().ExecuteAsync(sql, value);
        }

        public Task<int> Delete(string tableName, string[] ids)
        {
            var sql = $"DELETE FROM {tableName} WHERE Id IN @Id";
            return Connection().ExecuteAsync(sql, new {Id = ids});
        }

        public IDbConnection Connection()
        {
            return MainConnection.Interface.GetConnection();
        }
    }
}