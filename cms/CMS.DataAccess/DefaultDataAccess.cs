using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Extension;
using Foundation.DataAccess.Connections;
using Foundation.Modal;
using Foundation.Modal.RequestModal;

namespace DataAccess
{
    public class DefaultDataAccess<T> where T : class, new()
    {
        protected virtual IDbConnection Connection()
        {
            return MainConnection.Interface.GetConnection();
        }

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

        protected string OutDefaultPageParams(List<IQuery> queries, out IDictionary<string, object> whereParams,
            out List<string> whereSql)
        {
            whereSql = new List<string>();
            whereParams = new ExpandoObject();
            if (queries != null)
            {
                foreach (var query in queries)
                {
                    whereSql.Add(query.QuerySql.ToWhereString());
                    whereParams.Add(query.QuerySql.ParamName, query.Value);
                }
            }

            return whereSql.Count > 0 ? $"AND {string.Join(" AND ", whereSql)}" : "";
        }

        protected string OutDefaultPageParams(List<IQuery> queries, out IDictionary<string, object> whereParams)
        {
            return OutDefaultPageParams(queries, out whereParams, out List<string> whereSql);
        }
        
        public virtual async Task<PageResponse> Page(IPageRequest req)
        {
            string whereSql = OutDefaultPageParams(req.Queries, out IDictionary<string, object> whereParams);
            string dataSql =
                $"SELECT * FROM (SELECT *,ROW_NUMBER() OVER({req.Sort.ToSql()}) AS RowNum FROM [{GetTableName()}] WHERE 1=1 {whereSql}) AS temp WHERE RowNum BETWEEN {req.Begin} AND {req.End} ";
            string countSql =
                $"SELECT COUNT(Id) FROM {GetTableName()} WHERE 1=1 {whereSql}";

            return new PageResponse(await Connection().QueryAsync<dynamic>(dataSql, whereParams),
                await Connection().QueryFirstAsync<long>(countSql, whereParams));
        }
    }
}