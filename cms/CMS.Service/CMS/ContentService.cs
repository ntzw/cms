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

        public async Task<dynamic> GetByNumAndColumn(string columnNum, string num)
        {
            var model = await ColumnService.Interface.GetModelByNum(columnNum);
            if (model?.ModelTable == null) return null;

            return GetByNum(model?.ModelTable.SqlTableName, num);
        }

        #region 编辑内容

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

        #endregion

        public Task<HandleResult> Update(Dictionary<string, object> form, string tableName)
        {
            if (!form.ContainsKey("id")) return Task.FromResult(HandleResult.Error("无效参数"));

            int id = form["id"].ToInt();
            if (id <= 0) return Task.FromResult(HandleResult.Error("无效参数"));

            var sqlHelper = new DynamicTableSqlHelper(tableName);
            sqlHelper.SetDictionaryData(form, new[] {"id"});

            return Update(sqlHelper, id);
        }

        public async Task<HandleResult> Update(DynamicTableSqlHelper form, int id)
        {
            string sql = form.GetUpdateSql(id);
            if (sql.IsEmpty()) return HandleResult.Error("无效参数");

            var res = await _dapper.Execute(sql, form.GetValue());
            return res > 0 ? HandleResult.Success() : HandleResult.Error("操作失败");
        }

        public async Task<HandleResult> Add(DynamicTableSqlHelper form)
        {
            string sql = form.GetAddSql();
            if (sql.IsEmpty()) return HandleResult.Error("无效参数");

            var res = await _dapper.Execute(sql, form.GetValue());
            return res > 0 ? HandleResult.Success() : HandleResult.Error("操作失败");
        }


        #region 分页查询

        public async Task<PageResponse> Page(JObject form, bool isRecycle = false)
        {
            var req = new SqlServerPageRequest(form);

            var columnNum = req.GetQueryField("columnNum")?.Value.ToStr();
            if (columnNum == null) return PageResponse.Error("无效数据");

            var column = await ColumnService.Interface.GetByNum(columnNum);
            if (column == null) return PageResponse.Error("无效数据");

            var model = await ModelTableService.Interface.GetByNum(column.ModelNum);
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
            if (columnField == null || columnField.Value.ToStr().IsEmpty()) return null;

            var cm = await ColumnService.Interface.GetModelByNum(columnField.Value.ToStr());
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
        /// 将Dynamic 值转 不区分大小写的 Dictionary
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

        /// <summary>
        /// 更新是否置顶
        /// </summary>
        /// <param name="columnNum"></param>
        /// <param name="num"></param>
        /// <param name="isTop"></param>
        /// <returns></returns>
        public async Task<HandleResult> UpdateIsTop(string columnNum, string num, bool isTop)
        {
            var cm = await ColumnService.Interface.GetModelByNum(columnNum);
            if (cm == null || cm?.ModelTable == null) return HandleResult.Error("无效参数");

            var count = await _dapper.UpdateIsTop(cm?.ModelTable.SqlTableName, num, isTop);
            return count > 0 ? HandleResult.Success() : HandleResult.Error("");
        }

        /// <summary>
        /// 删除数据
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
        /// 移动到回收站
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
    }
}