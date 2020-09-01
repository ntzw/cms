using System;
using System.Collections.Generic;
using Foundation.Modal;
using Foundation.Modal.RequestModal;

namespace Foundation.DataAccess.Interface
{
    public interface IPageSqlHelper
    {
        string OutDefaultParams(List<IQuery> queries,
            out IDictionary<string, object> whereParams,
            Action<IQuery, List<string>, IDictionary<string, object>> setAction = null);

        string OutDefaultParams(List<IQuery> queries,
            out IDictionary<string, object> whereParams,
            out List<string> whereSql,
            Action<IQuery, List<string>, IDictionary<string, object>> setAction = null);

        string GetPageCountSql(string whereSql, string tableName);

        string GetPageDataSql(IPageRequest req, string whereSql, string tableName);
    }
}