using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.CMS;
using Foundation.DataAccess.Connections;
using Model.CMS;

namespace DataAccess.SqlServer.CMS
{
    public class CategoryDapper : ICategoryDapper
    {
        public Task<IEnumerable<Category>> GetByColumnNum(string columnNum, string tableName)
        {
            string sql = $"SELECT * FROM [{tableName}] WHERE ColumnNum = @ColumnNum";
            return MainConnection.Interface.GetConnection().QueryAsync<Category>(sql, new {ColumnNum = columnNum});
        }

        public Task<Category> GetById(string tableName, int id)
        {
            string sql = $"SELECT * FROM [{tableName}] WHERE Id = @Id ";
            return MainConnection.Interface.GetConnection().QueryFirstOrDefaultAsync<Category>(sql, new {Id = id});
        }

        public Task<Category> GetByNum(string tableName, string num)
        {
            string sql = $"SELECT * FROM [{tableName}] WHERE Num = @Num";
            return MainConnection.Interface.GetConnection().QueryFirstOrDefaultAsync<Category>(sql, new {Num = num});
        }

        public Task<int> Execute(string sql, ExpandoObject value)
        {
            return MainConnection.Interface.GetConnection().ExecuteAsync(sql, value);
        }

        public Task<int> Delete(string tableName, string[] ids)
        {
            string sql = $"DELETE FROM {tableName} WHERE Id IN @Id";
            return MainConnection.Interface.GetConnection().ExecuteAsync(sql, new {Id = ids});
        }
    }
}