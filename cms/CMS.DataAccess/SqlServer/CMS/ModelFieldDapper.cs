/**
 * author：created by zw on 2020-07-08 15:54:53
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using DataAccess.Interface.CMS;
using Extension;
using Foundation.Modal;
using Model.CMS;

namespace DataAccess.SqlServer.CMS
{
    public class ModelFieldDapper : DefaultDataAccess<ModelField>, IModelFieldDapper
    {
        protected override void SetQueryWhereSqlAndParams(IQuery query, IDictionary<string, object> whereParams,
            List<string> whereSql)
        {
            switch (query.QuerySql.FieldName.ToLower())
            {
                case "notcolumnnum":
                    if (query.Value.ToStr().IsEmpty()) return;

                    whereSql.Add(
                        $" Num NOT IN (SELECT ModelFieldNum FROM {GetTableName<ColumnField>()} WHERE ColumnNum = @{query.QuerySql.ParamName}) ");
                    whereParams.Add(query.QuerySql.ParamName, query.Value);
                    break;
                default:
                    base.SetQueryWhereSqlAndParams(query, whereParams, whereSql);
                    break;
            }
        }
    }
}