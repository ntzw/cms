using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using CMS.Modules.Content.Abstractions.Model.Content;
using CMS.React.Model;
using Extension;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;
using Helper;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Content.Service
{
    public class ContentService : IContentService
    {
        private readonly IColumnFieldService _columnFieldService;
        private readonly IColumnService _columnService;
        private readonly IContentDapper _dapper;
        private readonly IModelTableService _modelTableService;

        public ContentService(
            IContentDapper dapper,
            IColumnService columnService,
            IColumnFieldService columnFieldService,
            IModelTableService modelTableService)
        {
            _dapper = dapper;
            _columnService = columnService;
            _columnFieldService = columnFieldService;
            _modelTableService = modelTableService;
        }

        public Task<dynamic> GetByNum(string tableName, string num)
        {
            return _dapper.GetByItem(tableName, num);
        }

        public async Task<dynamic> GetByNumAndColumn(string columnNum, string num)
        {
            var model = await _columnService.GetModelByNum(columnNum);
            if (model?.ModelTable == null) return null;

            return GetByNum(model?.ModelTable.SqlTableName, num);
        }

        public Task<HandleResult> Update(Dictionary<string, object> form, string tableName)
        {
            if (!form.ContainsKey("id")) return Task.FromResult(HandleResult.Error());

            var id = form["id"].ToInt();
            if (id <= 0) return Task.FromResult(HandleResult.Error());

            var sqlHelper = new DynamicTableSqlHelper(tableName);
            sqlHelper.SetDictionaryData(form, new[] {"id"});

            return Update(sqlHelper, id);
        }

        public async Task<HandleResult> Update(DynamicTableSqlHelper form, int id)
        {
            var sql = form.GetUpdateSql(id);
            if (sql.IsEmpty()) return HandleResult.Error();

            var res = await _dapper.Execute(sql, form.GetValue());
            return res > 0 ? HandleResult.Success() : HandleResult.Error("操作失败");
        }

        public async Task<HandleResult> Add(DynamicTableSqlHelper form)
        {
            var sql = form.GetAddSql();
            if (sql.IsEmpty()) return HandleResult.Error();

            var res = await _dapper.Execute(sql, form.GetValue());
            return res > 0 ? HandleResult.Success() : HandleResult.Error("操作失败");
        }

        public async Task<ContentData> GetFirstByColumnNum(string tableName, string columnNum)
        {
            var list = await _dapper.GetByConditions(tableName, new SqlServerSelectRequest
            {
                TopCount = 1,
                Queries = new List<IQuery>
                {
                    new DefaultQuery(columnNum, new DefaultQuerySql("ColumnNum"))
                }
            });
            var enumerable = list.ToList();
            if (enumerable.Any()) return enumerable[0] != null ? new ContentData(enumerable[0]) : null;
            return null;
        }

        public async Task<ContentData> GetFirstByColumnNum(string columnNum)
        {
            var columnModel = await _columnService.GetModelByNum(columnNum);
            if (columnModel == null) return null;

            return await GetFirstByColumnNum(columnModel?.ModelTable.SqlTableName, columnNum);
        }

        public async Task<ContentData> GetFirstByColumnNum(string columnNum, ISelectRequest request)
        {
            var columnModel = await _columnService.GetModelByNum(columnNum);
            if (columnModel == null) return null;

            request?.Queries.Add(new DefaultQuery(columnNum, new DefaultQuerySql("ColumnNum")));

            var list = (await _dapper.GetByConditions(columnModel?.ModelTable.SqlTableName, request)).ToList();
            if (list.Count > 0) return list[0] != null ? new ContentData(list[0]) : null;
            return null;
        }

        public async Task<HandleResult> UpdateClickCount(string tableName, int id, int count)
        {
            var exCount = await _dapper.UpdateClickCount(tableName, id, count);
            return exCount > 0 ? HandleResult.Success() : HandleResult.Error("");
        }

        public async Task<ContentData> GetNext(string tableName, string columnNum, int id)
        {
            var data = await _dapper.GetNext(tableName, columnNum, id);

            return data != null ? new ContentData(data) : null;
        }

        public async Task<ContentData> GetPrev(string tableName, string columnNum, int id)
        {
            var data = await _dapper.GetPrev(tableName, columnNum, id);
            return data != null ? new ContentData(data) : null;
        }

        public async Task<IEnumerable<dynamic>> GetByConditions(ISelectRequest request)
        {
            var columnField = request.GetQueryField("columnNum");
            if (columnField == null || columnField.Value.ToStr().IsEmpty()) return null;

            var cm = await _columnService.GetModelByNum(columnField.Value.ToStr());
            if (cm == null) return null;

            return await GetByConditions(cm?.ModelTable.SqlTableName, request);
        }

        public Task<IEnumerable<dynamic>> GetByConditions(string tableName, ISelectRequest request)
        {
            return _dapper.GetByConditions(tableName, request);
        }

        public async Task<IEnumerable<dynamic>> GetByColumnNum(string columnNum)
        {
            return await GetByConditions(new SqlServerSelectRequest
            {
                Queries = new List<IQuery>
                {
                    new DefaultQuery(columnNum, new DefaultQuerySql("columnNum")),
                    new DefaultQuery(false, new DefaultQuerySql("IsDel"))
                }
            });
        }

        public async Task<List<Dictionary<string, object>>> GetDictionaryDataByColumnNum(string columnNum)
        {
            var data = await GetByColumnNum(columnNum);
            return data?.Select(temp =>
            {
                Dictionary<string, object> item = DynamicToDictionary(temp);
                return item;
            }).ToList();
        }

        /// <summary>
        ///     将Dynamic 值转 不区分大小写的 Dictionary
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IDictionary<string, object> DynamicToDictionary(dynamic item)
        {
            return item == null ? null : new Dictionary<string, object>(item, StringComparer.OrdinalIgnoreCase);
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

        /// <summary>
        ///     更新是否置顶
        /// </summary>
        /// <param name="columnNum"></param>
        /// <param name="num"></param>
        /// <param name="isTop"></param>
        /// <returns></returns>
        public async Task<HandleResult> UpdateIsTop(string columnNum, string num, bool isTop)
        {
            var cm = await _columnService.GetModelByNum(columnNum);
            if (cm == null || cm?.ModelTable == null) return HandleResult.Error();

            var count = await _dapper.UpdateIsTop(cm?.ModelTable.SqlTableName, num, isTop);
            return count > 0 ? HandleResult.Success() : HandleResult.Error("");
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<HandleResult> Delete(string tableName, List<int> ids)
        {
            var count = await _dapper.Delete(tableName, ids);
            return count > 0 ? HandleResult.Success() : HandleResult.Error("");
        }

        /// <summary>
        ///     移动到回收站
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<HandleResult> MoveRecycle(string tableName, List<int> ids)
        {
            var count = await _dapper.Recycle(tableName, ids, true);
            return count > 0 ? HandleResult.Success() : HandleResult.Error("");
        }

        public async Task<HandleResult> Removed(string tableName, List<int> ids)
        {
            var count = await _dapper.Recycle(tableName, ids, false);
            return count > 0 ? HandleResult.Success() : HandleResult.Error("");
        }

        private List<CascaderDataType> GetCascaderData(List<Dictionary<string, object>> contents, string parentNum,
            string currentFieldName, string labelFieldName)
        {
            var child = contents.FindAll(temp => Equals(temp[currentFieldName] ?? "", parentNum ?? ""));
            if (child.Count <= 0) return null;

            var data = new List<CascaderDataType>();
            foreach (var item in child)
            {
                contents.Remove(item);

                var num = item["Num"].ToStr();
                data.Add(new CascaderDataType
                {
                    Label = item[labelFieldName].ToStr(),
                    Value = num,
                    Children = GetCascaderData(contents, num, currentFieldName, labelFieldName)
                });
            }

            return data;
        }

        #region 编辑内容

        public async Task<HandleResult> Edit(JObject form, string accountNum)
        {
            var itemNum = form["num"].ToStr();
            var columnNum = form["columnNum"].ToStr();

            if (columnNum.IsEmpty()) return HandleResult.Error("无效的提交数据");

            var column = await _columnService.GetByNum(columnNum);
            if (column == null) return HandleResult.Error("无效的提交数据");

            var model = await _modelTableService.GetByNum(column.ModelNum);
            if (model == null) return HandleResult.Error("栏目未绑定模型");

            ContentData oldData = null;
            if (column.IsSingle)
                oldData = await GetFirstByColumnNum(model.SqlTableName, column.Num);
            else if (itemNum.IsNotEmpty()) oldData = await GetByItem(model.SqlTableName, itemNum);

            var id = oldData?.Id ?? 0;

            var contentEdit = new DynamicTableSqlHelper(model.SqlTableName);
            contentEdit.SetData(await _columnFieldService.GetByColumnNum(columnNum), form);
            contentEdit.AddFieldAndValue("SeoTitle", form["seoTitle"].ToStr());
            contentEdit.AddFieldAndValue("SeoKeyword", form["seoKeyword"].ToStr());
            contentEdit.AddFieldAndValue("SeoDesc", form["seoDesc"].ToStr());
            contentEdit.AddFieldAndValue("CategoryNum", form["categoryNum"].ToStr());
            contentEdit.AddFieldAndValue("IsTop", form["isTop"].ToBoolean());

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

            return id > 0 ? await Update(contentEdit, id) : await Add(contentEdit);
        }

        public async Task<ContentData> GetByItem(string tableName, string num)
        {
            var data = await _dapper.GetByItem(tableName, num);
            return data != null ? new ContentData(data) : null;
        }

        #endregion


        #region 分页查询

        public async Task<PageResponse> Page(JObject form, bool isRecycle = false)
        {
            var req = new SqlServerPageRequest(form);

            var columnNum = req.GetQueryField("columnNum")?.Value.ToStr();
            if (columnNum == null) return PageResponse.Error("无效数据");

            var column = await _columnService.GetByNum(columnNum);
            if (column == null) return PageResponse.Error("无效数据");

            var model = await _modelTableService.GetByNum(column.ModelNum);
            if (model == null) return PageResponse.Error("栏目未绑定模型");

            req.Queries.Add(new DefaultQuery(isRecycle, new DefaultQuerySql("IsDel")));

            return await Page(model.SqlTableName, req);
        }

        public async Task<PageResponse> Page(string tableName, IPageRequest req)
        {
            if (!req.ContainsQueryField("columnNum")) return PageResponse.Error("无效数据");

            return await _dapper.Page(tableName, req);
        }

        #endregion
    }
}