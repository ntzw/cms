using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Enums;
using DataAccess.Interface.CMS;
using DataAccess.SqlServer.CMS;
using Extension;
using Foundation.ColumnFieldOptions;
using Foundation.Modal;
using Foundation.Modal.Result;
using Helper;
using Newtonsoft.Json.Linq;

namespace Service.CMS
{
    public class ContentService
    {
        private IContentDapper _dapper = new ContentDapper();

        private ContentService()
        {
        }

        private static ContentService _interface;
        public static ContentService Interface => _interface ??= new ContentService();

        public Task<dynamic> GetByNum(string tableName, string num)
        {
            return _dapper.GetByItem(tableName, num);
        }

        public async Task<HandleResult> Edit(JObject form, string accountNum)
        {
            string itemNum = form["num"].ToStr();
            string columnNum = form["columnNum"].ToStr();

            if (columnNum.IsEmpty()) return HandleResult.Error("无效的提交数据");

            var column = await ColumnService.Interface.GetByNum(columnNum);
            if (column == null) return HandleResult.Error("无效的提交数据");

            var model = await ModelTableService.Interface.GetByNum(column.ModelNum);
            if (model == null) return HandleResult.Error("栏目未绑定模型");

            var oldData =
                (itemNum.IsEmpty() ? null : await _dapper.GetByItem(model.SqlTableName, itemNum)) as
                IDictionary<string, object>;

            var id = oldData?["Id"].ToInt() ?? 0;

            var editFieldSql = new List<string>();
            var editValue = new ExpandoObject();
            var fields = await ColumnFieldService.Interface.GetByColumnNum(columnNum);
            foreach (var columnField in fields)
            {
                var formFieldName = columnField.Name.ToFieldNameLower();
                if (!form.ContainsKey(formFieldName))
                    continue;

                editFieldSql.Add(columnField.Name);

                var value = form[formFieldName];
                switch (columnField.OptionType)
                {
                    case ReactFormItemType.Input:
                    case ReactFormItemType.Editor:
                    case ReactFormItemType.TextArea:
                    case ReactFormItemType.Select:
                        editValue.TryAdd(formFieldName, value.ToStr());
                        break;
                    case ReactFormItemType.Password:
                        editValue.TryAdd(formFieldName, Md5Helper.GetMD5_32(value.ToStr()));
                        break;
                    case ReactFormItemType.Cascader:
                        break;
                    case ReactFormItemType.Switch:
                        break;
                    case ReactFormItemType.Radio:
                        break;
                    case ReactFormItemType.CheckBox:
                        break;
                    case ReactFormItemType.DataPicker:
                        break;
                    case ReactFormItemType.RangePicker:
                        break;
                    case ReactFormItemType.Upload:
                        break;
                    case ReactFormItemType.Region:
                        break;
                }
            }

            editFieldSql.Add("SeoTitle");
            editFieldSql.Add("SeoKeyword");
            editFieldSql.Add("SeoDesc");

            editValue.TryAdd("SeoTitle", form["seoTitle"].ToStr());
            editValue.TryAdd("SeoKeyword", form["seoKeyword"].ToStr());
            editValue.TryAdd("SeoDesc", form["seoDesc"].ToStr());

            string sql = "";
            if (id > 0)
            {
                editFieldSql.Add("UpdateDate");
                editFieldSql.Add("UpdateAccountNum");

                editValue.TryAdd("UpdateDate", DateTime.Now);
                editValue.TryAdd("UpdateAccountNum", accountNum);
                editValue.TryAdd("Id", id);

                sql =
                    $"UPDATE [{model.SqlTableName}] SET {string.Join(",", editFieldSql.Select(temp => $"[{temp}] = @{temp}"))} WHERE Id = @Id";
            }
            else
            {
                editFieldSql.Add("Num");
                editFieldSql.Add("CreateDate");
                editFieldSql.Add("UpdateDate");
                editFieldSql.Add("CreateAccountNum");
                editFieldSql.Add("UpdateAccountNum");
                editFieldSql.Add("IsDel");
                editFieldSql.Add("Status");
                editFieldSql.Add("SiteNum");
                editFieldSql.Add("ColumnNum");

                editValue.TryAdd("Num", RandomHelper.CreateNum());
                editValue.TryAdd("CreateDate", DateTime.Now);
                editValue.TryAdd("UpdateDate", DateTime.Now);
                editValue.TryAdd("CreateAccountNum", accountNum);
                editValue.TryAdd("UpdateAccountNum", accountNum);
                editValue.TryAdd("IsDel", false);
                editValue.TryAdd("Status", 0);
                editValue.TryAdd("SiteNum", column.SiteNum);
                editValue.TryAdd("ColumnNum", columnNum);

                sql =
                    $"INSERT INTO [{model.SqlTableName}] ([{string.Join("],[", editFieldSql)}]) VALUES ({string.Join(",", editFieldSql.Select(temp => $"@{temp}"))}) ";
            }

            var res = await _dapper.Execute(sql, editValue);
            return res > 0 ? HandleResult.Success() : HandleResult.Error("操作失败");
        }

        public async Task<PageResponse> Page(JObject form)
        {
            var req = new SqlServerPageRequest(form);

            var columnNum = req.GetQueryField("columnNum")?.Value.ToStr();
            if (columnNum == null) return PageResponse.Error("无效数据");

            var column = await ColumnService.Interface.GetByNum(columnNum);
            if (column == null) return PageResponse.Error("无效数据");

            var model = await ModelTableService.Interface.GetByNum(column.ModelNum);
            if (model == null) return PageResponse.Error("栏目未绑定模型");

            return await _dapper.Page(model.SqlTableName, req);
        }
    }
}