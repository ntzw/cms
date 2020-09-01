/**
 * author：created by zw on 2020-06-20 14:28:18
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.CMS;
using Model.CMS;

namespace DataAccess.MySql.CMS
{
    public class ColumnDapper : DefaultDataAccess<Column>, IColumnDapper
    {
        public Task<IEnumerable<Column>> GetByParentNum(string parentNum)
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE ParentNum = @ParentNum ";
            return Connection().QueryAsync<Column>(sql, new {ParentNum = parentNum});
        }

        public Task<IEnumerable<dynamic>> GetCascaderDataByParentNum(string parentNum)
        {
            var sql =
                $"SELECT *,(SELECT COUNT(Id) FROM {GetTableName()} WHERE ParentNum = temp.Num) AS ChildCount FROM {GetTableName()} AS temp WHERE ParentNum = @ParentNum ";
            return Connection().QueryAsync<dynamic>(sql, new {ParentNum = parentNum});
        }

        public Task<IEnumerable<Column>> GetBySiteNum(string siteNum)
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE SiteNum = @SiteNum ";
            return Connection().QueryAsync<Column>(sql, new {SiteNum = siteNum});
        }
    }
}