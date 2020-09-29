/**
 * author：created by zw on 2020-07-10 13:51:32
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Model;
using Dapper;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Content.DataAccess.MySql
{
    public class ColumnFieldDapper : DefaultDataAccess<ColumnField>, IColumnFieldDapper
    {
        public ColumnFieldDapper(IPageSqlHelper pageSqlHelper) : base(pageSqlHelper)
        {
        }

        public Task<IEnumerable<ColumnField>> GetByModelFieldNum(string columnNum, List<string> modelFieldNum)
        {
            var sql =
                $"SELECT * FROM {GetTableName()} WHERE ColumnNum = @ColumnNum AND ModelFieldNum IN @ModelFieldNum ";
            return Connection()
                .QueryAsync<ColumnField>(sql, new {ColumnNum = columnNum, ModelFieldNum = modelFieldNum});
        }

        public Task<IEnumerable<ColumnField>> GetByColumnNum(string columnNum)
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE ColumnNum = @ColumnNum ORDER BY Sort DESC";
            return Connection().QueryAsync<ColumnField>(sql, new {ColumnNum = columnNum});
        }

        public Task<int> Clear(string[] columnNums)
        {
            var sql = $"DELETE FROM {GetTableName()} WHERE ColumnNum IN @ColumnNum";
            return Connection().ExecuteAsync(sql, new {ColumnNum = columnNums});
        }
    }
}