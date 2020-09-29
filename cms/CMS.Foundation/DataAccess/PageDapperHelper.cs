using System;
using System.Collections.Generic;
using System.Dynamic;
using Foundation.DataAccess.Interface;
using Foundation.Modal;
using Foundation.Modal.RequestModal;

namespace Foundation.DataAccess
{
    public abstract class PageDapperHelper : IPageSqlHelper
    {
        public string OutDefaultParams(List<IQuery> queries,
            out IDictionary<string, object> whereParams,
            Action<IQuery, List<string>, IDictionary<string, object>> setAction = null)
        {
            return OutDefaultParams(queries, out whereParams, out var whereSql, setAction);
        }

        public string OutDefaultParams(List<IQuery> queries,
            out IDictionary<string, object> whereParams,
            out List<string> whereSql,
            Action<IQuery, List<string>, IDictionary<string, object>> setAction = null)
        {
            whereSql = new List<string>();
            whereParams = new ExpandoObject();
            if (queries != null)
                foreach (var query in queries)
                    if (setAction != null)
                    {
                        setAction(query, whereSql, whereParams);
                    }
                    else
                    {
                        whereSql.Add(query.QuerySql.ToWhereString());
                        whereParams.Add(query.QuerySql.ParamName, query.Value);
                    }

            return whereSql.Count > 0 ? $"AND {string.Join(" AND ", whereSql)}" : "";
        }


        /// <summary>
        ///     获取分页查询总数SQL
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract string GetPageCountSql(string whereSql, string tableName);

        /// <summary>
        ///     获取分页查询数据SQL
        /// </summary>
        /// <param name="req"></param>
        /// <param name="whereSql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract string GetPageDataSql(IPageRequest req, string whereSql, string tableName);
    }
}