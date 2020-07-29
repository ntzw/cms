using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CMS.React.Model;
using DataAccess.Interface.CMS;
using DataAccess.SqlServer.CMS;
using Extension;
using Foundation.Modal.Result;
using Helper;
using Model.CMS;
using Newtonsoft.Json.Linq;

namespace Service.CMS
{
    public class CategoryService
    {
        private readonly ICategoryDapper _dapper = new CategoryDapper();

        private CategoryService()
        {
        }

        private static CategoryService _interface;
        public static CategoryService Interface => _interface ??= new CategoryService();


        public async Task<PageResponse> GetTreeTableData(string columnNum)
        {
            var model = await ColumnService.Interface.GetModelByNum(columnNum);
            if (model == null) return PageResponse.Error("栏目未绑定模型");

            var data = await _dapper.GetByColumnNum(columnNum, model.SqlCategoryTableName);

            var columns = data as Category[] ?? data.ToArray();
            return new PageResponse(await GetTreeTableData(columns.ToList(), ""), columns.Length);
        }

        private async Task<IEnumerable<dynamic>> GetTreeTableData(List<Category> categories, string parentNum)
        {
            var child = categories.FindAll(temp => string.Equals(temp.ParentNum ?? "", parentNum ?? ""));
            if (child.Count <= 0) return null;

            List<dynamic> data = new List<dynamic>();
            foreach (var category in child)
            {
                categories.Remove(category);

                var item = new ExpandoObject();
                item.TryAdd("Id", category.Id);
                item.TryAdd("Num", category.Num);
                item.TryAdd("Name", category.Name);
                item.TryAdd("ParentNum", category.ParentNum);
                item.TryAdd("CreateDate", category.CreateDate);

                var children = await GetTreeTableData(categories, category.Num);
                if (children != null && children.Any())
                    item.TryAdd("children", children);

                data.Add(item);
            }

            return data;
        }

        public async Task<Category> GetById(string columnNum, int id)
        {
            var model = await ColumnService.Interface.GetModelByNum(columnNum);
            if (model == null) return null;

            return await _dapper.GetById(model.SqlCategoryTableName, id);
        }

        public async Task<List<CascaderDataType>> GetCascaderData(string columnNum)
        {
            var model = await ColumnService.Interface.GetModelByNum(columnNum);
            if (model == null) return null;

            var data = await _dapper.GetByColumnNum(columnNum, model.SqlCategoryTableName);
            var categories = data as Category[] ?? data.ToArray();
            return GetCascaderData(categories.ToList(), "");
        }

        private List<CascaderDataType> GetCascaderData(List<Category> categories, string parentNum)
        {
            var child = categories.FindAll(temp => string.Equals(temp.ParentNum ?? "", parentNum ?? ""));
            if (child.Count <= 0) return null;

            List<CascaderDataType> data = new List<CascaderDataType>();
            foreach (var category in child)
            {
                categories.Remove(category);
                data.Add(new CascaderDataType
                {
                    Label = category.Name,
                    Value = category.Num,
                    Children = GetCascaderData(categories, category.Num)
                });
            }

            return data;
        }

        public async Task<HandleResult> Edit(JObject form, string accountNum)
        {
            string num = form["num"].ToStr();
            string columnNum = form["columnNum"].ToStr();

            if (columnNum.IsEmpty()) return HandleResult.Error("无效的提交数据");

            var column = await ColumnService.Interface.GetByNum(columnNum);
            if (column == null) return HandleResult.Error("无效的提交数据");

            var model = await ColumnService.Interface.GetModelByNum(columnNum);
            if (model == null) return HandleResult.Error("无效的提交数据");

            var oldData = (num.IsEmpty() ? null : await _dapper.GetByNum(model.SqlCategoryTableName, num));

            var id = oldData?.Id ?? 0;
            var parentNum = form["parentNum"].ToStr();

            if (oldData != null && oldData.Num == parentNum) return HandleResult.Error("父类别不能是自身");

            var contentEdit = new DynamicTableSqlHelper(model.SqlCategoryTableName);
            contentEdit.AddFieldAndValue("Name", form["name"].ToStr());
            contentEdit.AddFieldAndValue("ParentNum", parentNum);

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

        public async Task<HandleResult> Delete(string columnNum, string ids)
        {
            var model = await ColumnService.Interface.GetModelByNum(columnNum);
            if (model == null) return HandleResult.Error("无效参数");

            int count = await _dapper.Delete(model.SqlCategoryTableName,
                ids.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries));

            return count > 0 ? HandleResult.Success() : HandleResult.Error("");
        }
    }
}