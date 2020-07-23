using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.CMS;
using Foundation.DataAccess.Connections;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;

namespace DataAccess.SqlServer.CMS
{
    public class ContentDapper : PageDapperHelper, IContentDapper
    {
        public Task<dynamic> GetByItem(string tableName, string itemNum)
        {
            string sql = $"SELECT * FROM {tableName} WHERE Num = @Num";
            return MainConnection.Interface.GetConnection().QueryFirstOrDefaultAsync(sql, new {Num = itemNum});
        }

        public Task<int> Execute(string sql, object data)
        {
            return MainConnection.Interface.GetConnection().ExecuteAsync(sql, data);
        }

        public async Task<PageResponse> Page(string tableName, IPageRequest req)
        {
            string whereSql = OutDefaultPageParams(req.Queries, out IDictionary<string, object> whereParams);
            string dataSql = GetPageDataSql(req, whereSql, tableName);
            string countSql = GetPageCountSql(whereSql, tableName);

            return new PageResponse(
                await MainConnection.Interface.GetConnection().QueryAsync<dynamic>(dataSql, whereParams),
                await MainConnection.Interface.GetConnection().QueryFirstAsync<long>(countSql, whereParams));
        }
    }
}