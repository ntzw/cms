using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CMS.Enums;
using Dapper;
using DataAccess.Interface.CMS;
using Foundation.DataAccess.Connections;
using Foundation.DataAccess.Interface;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;

namespace DataAccess.SqlServer.CMS
{
    public class ContentDapper : IContentDapper
    {
        public Task<dynamic> GetByItem(string tableName, string itemNum)
        {
            string sql = $"SELECT * FROM {tableName} WHERE Num = @Num";
            return Connection().QueryFirstOrDefaultAsync(sql, new {Num = itemNum});
        }

        public Task<int> Execute(string sql, object data)
        {
            return Connection().ExecuteAsync(sql, data);
        }

        public async Task<PageResponse> Page(string tableName, IPageRequest req)
        {
            string whereSql =
                PageSqlHelper().OutDefaultParams(req.Queries, out IDictionary<string, object> whereParams);
            string dataSql = PageSqlHelper().GetPageDataSql(req, whereSql, tableName);
            string countSql = PageSqlHelper().GetPageCountSql(whereSql, tableName);

            return new PageResponse(
                await Connection().QueryAsync<dynamic>(dataSql, whereParams),
                await Connection().QueryFirstOrDefaultAsync<long>(countSql, whereParams));
        }

        public Task<int> UpdateClickCount(string tableName, int id, int count)
        {
            string sql = $"UPDATE [{tableName}] SET [ClickCount] = @ClickCount WHERE Id = @Id";
            return Connection().ExecuteAsync(sql, new {Id = id, ClickCount = count});
        }

        public Task<dynamic> GetNext(string tableName, string columnNum, int id)
        {
            string sql =
                $"SELECT * FROM {tableName} WHERE Id = (SELECT MAX(Id) FROM {tableName} WHERE IsDel = 0 AND ColumnNum = @ColumnNum AND Id < @Id)";
            return Connection()
                .QueryFirstOrDefaultAsync(sql, new {Id = id, ColumnNum = columnNum});
        }

        public Task<dynamic> GetPrev(string tableName, string columnNum, int id)
        {
            string sql =
                $"SELECT * FROM {tableName} WHERE Id = (SELECT MIN(Id) FROM {tableName} WHERE IsDel = 0 AND ColumnNum = @ColumnNum AND Id > @Id)";
            return Connection()
                .QueryFirstOrDefaultAsync(sql, new {Id = id, ColumnNum = columnNum});
        }

        public Task<IEnumerable<dynamic>> GetByConditions(string tableName, ISelectRequest request)
        {
            string whereSql = PageSqlHelper()
                .OutDefaultParams(request.Queries, out IDictionary<string, object> whereParams);

            string sql =
                $"SELECT {(request.TopCount > 0 ? $"TOP {request.TopCount}" : "")} * FROM [{tableName}] WHERE 1=1 {whereSql} {request.Sort?.ToSql()}";
            
            return Connection().QueryAsync(sql, whereParams);
        }


        public Task<int> UpdateIsTop(string tableName, string num, bool isTop)
        {
            string sql = $"UPDATE [{tableName}] SET IsTop = @IsTop WHERE Num = @Num";
            return Connection().ExecuteAsync(sql, new {IsTop = isTop, Num = num});
        }

        public Task<int> Delete(string tableName, List<int> ids)
        {
            string sql = $"DELETE FROM [{tableName}] WHERE Id IN @Id";
            return Connection().ExecuteAsync(sql, new {Id = ids});
        }

        public Task<int> Recycle(string tableName, List<int> ids, bool isDel)
        {
            string sql = $"UPDATE [{tableName}] SET IsDel = @IsDel WHERE Id IN @Id";
            return Connection().ExecuteAsync(sql, new {IsDel = isDel, Id = ids});
        }

        public IDbConnection Connection()
        {
            return MainConnection.Interface.GetConnection();
        }

        public IPageSqlHelper PageSqlHelper()
        {
            return SqlServerPageHelper.Instance;
        }
    }
}