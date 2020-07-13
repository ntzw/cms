/**
 * author：created by zw on 2020-07-10 13:51:32
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.CMS;
using Model.CMS;

namespace DataAccess.SqlServer.CMS
{
    public class ColumnFieldDapper : DefaultDataAccess<ColumnField>, IColumnFieldDapper
    {
        public Task<IEnumerable<ColumnField>> GetByModelFieldNum(string columnNum, List<string> modelFieldNum)
        {
            string sql =
                $"SELECT * FROM {GetTableName()} WHERE ColumnNum = @ColumnNum AND ModelFieldNum IN @ModelFieldNum ";
            return Connection()
                .QueryAsync<ColumnField>(sql, new {ColumnNum = columnNum, ModelFieldNum = modelFieldNum});
        }

        public Task<IEnumerable<ColumnField>> GetByColumnNum(string columnNum)
        {
            string sql = $"SELECT * FROM {GetTableName()} WHERE ColumnNum = @ColumnNum ";
            return Connection().QueryAsync<ColumnField>(sql, new {ColumnNum = columnNum});
        }
    }
}