using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CMS.React.Model;
using DataAccess.Interface.CMS;
using DataAccess.SqlServer.CMS;
using Extension;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;
using Helper;
using Model.CMS;
using Newtonsoft.Json.Linq;

namespace Service.CMS
{
    public class ContentService
    {
        private readonly IContentDapper _dapper = new ContentDapper();

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

            IDictionary<string, object> oldData = null;
            if (column.IsSingle)
            {
                oldData =
                    await _dapper.GetFirstByColumnNum(model.SqlTableName, column.Num) as IDictionary<string, object>;
            }
            else if (itemNum.IsNotEmpty())
            {
                oldData = await _dapper.GetByItem(model.SqlTableName, itemNum) as IDictionary<string, object>;
            }

            var id = oldData?["Id"].ToInt() ?? 0;

            var contentEdit = new DynamicTableSqlHelper(model.SqlTableName);
            contentEdit.SetJObjectFormData(await ColumnFieldService.Interface.GetByColumnNum(columnNum), form);
            contentEdit.AddFieldAndValue("SeoTitle", form["seoTitle"].ToStr());
            contentEdit.AddFieldAndValue("SeoKeyword", form["seoKeyword"].ToStr());
            contentEdit.AddFieldAndValue("SeoDesc", form["seoDesc"].ToStr());
            contentEdit.AddFieldAndValue("CategoryNum", form["categoryNum"].ToStr());


            if (id > 0)
            {
                contentEdit.AddFieldAndValue("UpdateAccountNum", accountNum);
                contentEdit.AddFieldAndValue("UpdateDate", DateTime.Now);
            }
            else
            {
                contentEdit.AddFieldAndValue("Num", RandomHelper.CreateNum());
                contentEdit.AddFieldAndValue("CreateDate", DateTime.Now);
                contentEdit.AddFieldAndValue("UpdateDate", DateTime.Now);
                contentEdit.AddFieldAndValue("CreateAccountNum", accountNum);
                contentEdit.AddFieldAndValue("UpdateAccountNum", accountNum);
                contentEdit.AddFieldAndValue("IsDel", false);
                contentEdit.AddFieldAndValue("Status", 0);
                contentEdit.AddFieldAndValue("SiteNum", column.SiteNum);
                contentEdit.AddFieldAndValue("ColumnNum", columnNum);
            }

            string sql = id > 0 ? contentEdit.GetUpdateSql(id) : contentEdit.GetAddSql();
            var res = await _dapper.Execute(sql, contentEdit.GetValue());
            return res > 0 ? HandleResult.Success() : HandleResult.Error("操作失败");
        }

        #region 分页查询

        public async Task<PageResponse> Page(JObject form)
        {
            var req = new SqlServerPageRequest(form);

            var columnNum = req.GetQueryField("columnNum")?.Value.ToStr();
            if (columnNum == null) return PageResponse.Error("无效数据");

            var column = await ColumnService.Interface.GetByNum(columnNum);
            if (column == null) return PageResponse.Error("无效数据");

            var model = await ModelTableService.Interface.GetByNum(column.ModelNum);
            if (model == null) return PageResponse.Error("栏目未绑定模型");

            return await Page(model.SqlTableName, req);
        }

        public async Task<PageResponse> Page(string tableName, IPageRequest req)
        {
            if (!req.ContainsQueryField("columnNum")) return PageResponse.Error("无效数据");

            return await _dapper.Page(tableName, req);
        }

        #endregion

        public Task<dynamic> GetFirstByColumnNum(string tableName, string columnNum)
        {
            return _dapper.GetFirstByColumnNum(tableName, columnNum);
        }

        public async Task<dynamic> GetFirstByColumnNum(string columnNum)
        {
            var columnModel = await ColumnService.Interface.GetModelByNum(columnNum);
            if (columnModel == null) return null;

            return await GetFirstByColumnNum(columnModel?.ModelTable.SqlTableName, columnNum);
        }

        public async Task<HandleResult> UpdateClickCount(string tableName, int id, int count)
        {
            var exCount = await _dapper.UpdateClickCount(tableName, id, count);
            return exCount > 0 ? HandleResult.Success() : HandleResult.Error("");
        }

        public Task<dynamic> GetNext(string tableName, string columnNum, int id)
        {
            return _dapper.GetNext(tableName, columnNum, id);
        }

        public Task<dynamic> GetPrev(string tableName, string columnNum, int id)
        {
            return _dapper.GetPrev(tableName, columnNum, id);
        }

        public async Task<IEnumerable<dynamic>> GetByConditions(ISelectRequest request)
        {
            var columnField = request.GetQueryField("columnNum");
            if (columnField == null) return null;

            var cm = await ColumnService.Interface.GetModelByNum(columnField.Value.ToStr());
            if (cm == null) return null;

            return await _dapper.GetByConditions(cm?.ModelTable.SqlTableName, request);
        }

        public async Task<IEnumerable<dynamic>> GetByColumnNum(string columnNum)
        {
            return await GetByConditions(new SqlServerSelectRequest
            {
                Queries = new List<IQuery>
                {
                    new DefaultQuery(columnNum, new DefaultQuerySql("columnNum"))
                }
            });
        }

        public async Task<List<Dictionary<string, object>>> GetDictionaryDataByColumnNum(string columnNum)
        {
            var data = await GetByColumnNum(columnNum);
            return data?.Select(temp => new Dictionary<string, object>(temp, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<HandleResult> GetCascaderData(string columnNum, string labelFieldName,
            string currentFieldName)
        {
            var data = await GetDictionaryDataByColumnNum(columnNum);
            var resData = new List<CascaderDataType>();
            if (data.Count > 0)
            {
                var firstItem = data[0];
                if (!firstItem.ContainsKey(labelFieldName) ||
                    !firstItem.ContainsKey(currentFieldName))
                    return HandleResult.Error("无效字段");

                resData = GetCascaderData(data, "", currentFieldName, labelFieldName);
            }

            return new HandleResult
            {
                IsSuccess = true,
                Data = resData
            };
        }

        private List<CascaderDataType> GetCascaderData(List<Dictionary<string, object>> contents, string parentNum,
            string currentFieldName, string labelFieldName)
        {
            var child = contents.FindAll(temp => string.Equals(temp[currentFieldName] ?? "", parentNum ?? ""));
            if (child.Count <= 0) return null;

            List<CascaderDataType> data = new List<CascaderDataType>();
            foreach (var item in child)
            {
                contents.Remove(item);

                string num = item["Num"].ToStr();
                data.Add(new CascaderDataType
                {
                    Label = item[labelFieldName].ToStr(),
                    Value = num,
                    Children = GetCascaderData(contents, num, currentFieldName, labelFieldName)
                });
            }

            return data;
        }
    }
}