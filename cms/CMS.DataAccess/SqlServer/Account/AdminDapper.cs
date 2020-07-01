using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.Account;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Model.Account;

namespace DataAccess.SqlServer.Account
{
    public class AdminDapper : DefaultDataAccess<Administrator>, IAdminDapper
    {
        public async Task<PageResponse> Page(IPageRequest req)
        {
            string whereSql = OutDefaultPageParams(req.Queries, out IDictionary<string, object> whereParams);
            string dataSql =
                $"SELECT * FROM (SELECT *,ROW_NUMBER() OVER({req.Sort.ToSql()}) AS RowNum FROM [{GetTableName()}] WHERE 1=1 {whereSql}) AS temp WHERE RowNum BETWEEN {req.Begin} AND {req.End} ";
            string countSql =
                $"SELECT COUNT(Id) FROM {GetTableName()} WHERE 1=1 {whereSql}";

            return new PageResponse(await Connection().QueryAsync<dynamic>(dataSql, whereParams),
                await Connection().QueryFirstAsync<long>(countSql, whereParams));
        }

        public Task<Administrator> GetByAccountName(string accountName)
        {
            string sql = $"SELECT * FROM {GetTableName()} WHERE AccountName = @AccountName";
            return Connection().QueryFirstAsync<Administrator>(sql, new {AccountName = accountName});
        }
    }
}