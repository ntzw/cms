/**
 * author：created by zw on 2020-07-08 15:54:53
 * email：ntzw.geek@gmail.com
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Enums;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Model;
using Dapper;
using Extension;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;
using Foundation.Modal;

namespace CMS.Modules.Content.DataAccess.MySql
{
    public class ModelFieldDapper : DefaultDataAccess<ModelField>, IModelFieldDapper
    {
        public ModelFieldDapper(IPageSqlHelper pageSqlHelper) : base(pageSqlHelper)
        {
        }

        public Task<int> CreateField(string fieldName, string tableName, ReactFormItemType fieldType)
        {
            var sql = "";
            switch (fieldType)
            {
                case ReactFormItemType.Input:
                case ReactFormItemType.Password:
                    sql += $"ALTER TABLE {tableName} ADD {fieldName} nvarchar (500) NULL";
                    break;
                case ReactFormItemType.TextArea:
                case ReactFormItemType.Editor:
                case ReactFormItemType.Tags:
                    sql +=
                        $"ALTER TABLE {tableName} ADD {fieldName} text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL";
                    break;
                case ReactFormItemType.Radio:
                case ReactFormItemType.Select:
                case ReactFormItemType.CheckBox:
                    sql +=
                        $"ALTER TABLE {tableName} ADD {fieldName} text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL";
                    break;
                case ReactFormItemType.DataPicker:
                case ReactFormItemType.RangePicker:
                    sql += $"ALTER TABLE {tableName} ADD {fieldName} timestamp(0) NULL DEFAULT NULL";
                    break;
                case ReactFormItemType.Upload:
                    sql += $"ALTER TABLE {tableName} ADD {fieldName} nvarchar  (4000) NULL";
                    break;
                case ReactFormItemType.Region:
                case ReactFormItemType.Cascader:
                    sql += $"ALTER TABLE {tableName} ADD {fieldName} int NULL DEFAULT NULL";
                    break;
                case ReactFormItemType.Switch:
                    sql += $"ALTER TABLE {tableName} ADD {fieldName} tinyint(1) NULL DEFAULT NULL";
                    break;
                default:
                    throw new Exception("不存在字段类型");
            }

            return sql.IsNotEmpty() ? Connection().ExecuteAsync(sql) : Task.FromResult(0);
        }

        public Task<ModelField> GetByName(string fieldName, string modelNum)
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE Name = @Name AND ModelNum = @ModelNum";
            return Connection().QueryFirstOrDefaultAsync<ModelField>(sql, new {Name = fieldName, ModelNum = modelNum});
        }

        protected override Action<IQuery, List<string>, IDictionary<string, object>> GetPageSetParamsAction()
        {
            return (query, whereSql, whereParams) =>
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
                        whereSql.Add(query.QuerySql.ToWhereString());
                        whereParams.Add(query.QuerySql.ParamName, query.Value);
                        break;
                }
            };
        }
    }
}