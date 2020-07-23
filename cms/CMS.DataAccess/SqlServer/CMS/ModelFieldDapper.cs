/**
 * author：created by zw on 2020-07-08 15:54:53
 * email：ntzw.geek@gmail.com
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Enums;
using Dapper;
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

        public Task<int> CreateField(string fieldName, string tableName, ReactFormItemType fieldType)
        {
            var sql = $"IF COL_LENGTH('{tableName}', '{fieldName}') IS NULL ";
            switch (fieldType)
            {
                case ReactFormItemType.Input:
                case ReactFormItemType.Password:
                    sql += $"ALTER TABLE [{tableName}] ADD [{fieldName}]  [nvarchar] (500) NULL";
                    break;
                case ReactFormItemType.TextArea:
                case ReactFormItemType.Editor:
                    sql += $"ALTER TABLE [{tableName}] ADD [{fieldName}]  [nvarchar] (MAX) NULL";
                    break;
                case ReactFormItemType.Radio:
                case ReactFormItemType.Select:
                case ReactFormItemType.CheckBox:
                    sql += $"ALTER TABLE [{tableName}] ADD [{fieldName}]  [nvarchar] (1000) NULL";
                    break;
                case ReactFormItemType.DataPicker:
                case ReactFormItemType.RangePicker:
                    sql += $"ALTER TABLE [{tableName}] ADD [{fieldName}]  [datetime]  NULL";
                    break;
                case ReactFormItemType.Upload:
                    sql += $"ALTER TABLE [{tableName}] ADD [{fieldName}]  [nvarchar]  (4000) NULL";
                    break;
                case ReactFormItemType.Region:
                case ReactFormItemType.Cascader:
                    sql += $"ALTER TABLE [{tableName}] ADD [{fieldName}]  [int] NULL";
                    break;
                case ReactFormItemType.Switch:
                    sql += $"ALTER TABLE [{tableName}] ADD [{fieldName}]  [bit] NULL";
                    break;
            }
            
            return sql.IsNotEmpty() ? Connection().ExecuteAsync(sql) : Task.FromResult(0);
        }

        public Task<ModelField> GetByName(string fieldName, string modelNum)
        {
            string sql = $"SELECT * FROM {GetTableName()} WHERE Name = @Name AND ModelNum = @ModelNum";
            return Connection().QueryFirstOrDefaultAsync<ModelField>(sql, new { Name = fieldName,ModelNum = modelNum  });
        }
    }
}