using System.Collections.Generic;
using System.Dynamic;
using Foundation.Modal;
using Foundation.Modal.RequestModal;

namespace DataAccess
{
    public abstract class PageDapperHelper
    {
        protected virtual string OutDefaultPageParams(List<IQuery> queries, out IDictionary<string, object> whereParams)
        {
            return OutDefaultPageParams(queries, out whereParams, out List<string> whereSql);
        }

        protected virtual string OutDefaultPageParams(List<IQuery> queries, out IDictionary<string, object> whereParams,
            out List<string> whereSql)
        {
            whereSql = new List<string>();
            whereParams = new ExpandoObject();
            if (queries != null)
            {
                foreach (var query in queries)
                {
                    SetQueryWhereSqlAndParams(query, whereParams, whereSql);
                }
            }

            return whereSql.Count > 0 ? $"AND {string.Join(" AND ", whereSql)}" : "";
        }

        protected virtual void SetQueryWhereSqlAndParams(IQuery query, IDictionary<string, object> whereParams,
            List<string> whereSql)
        {
            whereSql.Add(query.QuerySql.ToWhereString());
            whereParams.Add(query.QuerySql.ParamName, query.Value);
        }

        /// <summary>
        /// 获取分页查询总数SQL
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected virtual string GetPageCountSql(string whereSql, string tableName)
        {
            return $"SELECT COUNT(Id) FROM {tableName} WHERE 1=1 {whereSql}";
        }

        /// <summary>
        /// 获取分页查询数据SQL
        /// </summary>
        /// <param name="req"></param>
        /// <param name="whereSql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected virtual string GetPageDataSql(IPageRequest req, string whereSql, string tableName)
        {
            return
                $"SELECT * FROM (SELECT *,ROW_NUMBER() OVER({req.Sort.ToSql()}) AS RowNum FROM [{tableName}] WHERE 1=1 {whereSql}) AS temp WHERE RowNum BETWEEN {req.Begin} AND {req.End} ";
        }
    }
}