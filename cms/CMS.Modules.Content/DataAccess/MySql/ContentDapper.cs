using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using Dapper;
using Foundation.DataAccess.Connections;
using Foundation.DataAccess.Interface;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;

namespace CMS.Modules.Content.DataAccess.MySql
{
    public class ContentDapper : IContentDapper
    {
        private readonly IPageSqlHelper _pageSqlHelper;

        public ContentDapper(IPageSqlHelper pageSqlHelper)
        {
            _pageSqlHelper = pageSqlHelper;
        }

        public Task<dynamic> GetByItem(string tableName, string itemNum)
        {
            var sql = $"SELECT * FROM {tableName} WHERE Num = @Num";
            return Connection().QueryFirstOrDefaultAsync(sql, new {Num = itemNum});
        }

        public Task<int> Execute(string sql, object data)
        {
            return Connection().ExecuteAsync(sql, data);
        }

        public async Task<PageResponse> Page(string tableName, IPageRequest req)
        {
            var whereSql = _pageSqlHelper.OutDefaultParams(req.Queries, out var whereParams);
            var dataSql = _pageSqlHelper.GetPageDataSql(req, whereSql, tableName);
            var countSql = _pageSqlHelper.GetPageCountSql(whereSql, tableName);

            return new PageResponse(
                await Connection().QueryAsync<dynamic>(dataSql, whereParams),
                await Connection().QueryFirstOrDefaultAsync<long>(countSql, whereParams));
        }

        public Task<int> UpdateClickCount(string tableName, int id, int count)
        {
            var sql = $"UPDATE {tableName} SET ClickCount = @ClickCount WHERE Id = @Id";
            return Connection().ExecuteAsync(sql, new {Id = id, ClickCount = count});
        }

        public Task<dynamic> GetNext(string tableName, string columnNum, int id)
        {
            var sql =
                $"SELECT * FROM {tableName} WHERE Id = (SELECT MAX(Id) FROM {tableName} WHERE IsDel = 0 AND ColumnNum = @ColumnNum AND Id < @Id)";
            return Connection()
                .QueryFirstOrDefaultAsync(sql, new {Id = id, ColumnNum = columnNum});
        }

        public Task<dynamic> GetPrev(string tableName, string columnNum, int id)
        {
            var sql =
                $"SELECT * FROM {tableName} WHERE Id = (SELECT MIN(Id) FROM {tableName} WHERE IsDel = 0 AND ColumnNum = @ColumnNum AND Id > @Id)";
            return Connection()
                .QueryFirstOrDefaultAsync(sql, new {Id = id, ColumnNum = columnNum});
        }

        public Task<IEnumerable<dynamic>> GetByConditions(string tableName, ISelectRequest request)
        {
            var whereSql = _pageSqlHelper.OutDefaultParams(request.Queries, out var whereParams);

            var sql =
                $"SELECT * FROM {tableName} WHERE 1=1 {whereSql} {request.Sort?.ToSql()} {(request.TopCount > 0 ? $"limit 0,{request.TopCount}" : "")} ";
            return Connection().QueryAsync(sql, whereParams);
        }

        public Task<int> UpdateIsTop(string tableName, string num, bool isTop)
        {
            var sql = $"UPDATE {tableName} SET IsTop = @IsTop WHERE Num = @Num";
            return Connection().ExecuteAsync(sql, new {IsTop = isTop, Num = num});
        }

        public Task<int> Delete(string tableName, List<int> ids)
        {
            var sql = $"DELETE FROM {tableName} WHERE Id IN @Id";
            return Connection().ExecuteAsync(sql, new {Id = ids});
        }

        public Task<int> Recycle(string tableName, List<int> ids, bool isDel)
        {
            var sql = $"UPDATE {tableName} SET IsDel = @IsDel WHERE Id IN @Id";
            return Connection().ExecuteAsync(sql, new {IsDel = isDel, Id = ids});
        }

        public IDbConnection Connection()
        {
            return MainConnection.Interface.GetConnection();
        }
    }
}