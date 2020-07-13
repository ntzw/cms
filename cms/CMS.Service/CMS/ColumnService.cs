/**
 * author：created by zw on 2020-06-20 14:28:19
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CMS.React.Model;
using DataAccess.Interface.CMS;
using DataAccess.SqlServer.CMS;
using Foundation.Modal;
using Model.CMS;

namespace Service.CMS
{
    public class ColumnService : DefaultService<Column, IColumnDapper>
    {
        private IColumnDapper _dapper;

        protected override IColumnDapper GetDapper() => _dapper ??= new ColumnDapper();

        private ColumnService()
        {
        }

        private static ColumnService _interface;
        public static ColumnService Interface => _interface ??= new ColumnService();

        public async Task<List<CascaderDataType>> GetCascaderData(string siteNum)
        {
            var data = await GetDapper().GetBySiteNum(siteNum);
            var columns = data as Column[] ?? data.ToArray();
            return GetCascaderData(columns.ToList(), "");
        }

        private List<CascaderDataType> GetCascaderData(List<Column> roles, string parentNum)
        {
            var child = roles.FindAll(temp => string.Equals(temp.ParentNum ?? "", parentNum ?? ""));
            if (child.Count <= 0) return null;

            List<CascaderDataType> data = new List<CascaderDataType>();
            foreach (var role in child)
            {
                roles.Remove(role);
                data.Add(new CascaderDataType
                {
                    Label = role.Name,
                    Value = role.Num,
                    Children = GetCascaderData(roles, role.Num)
                });
            }

            return data;
        }

        public async Task<PageResponse> GetTreeTableData(string siteNum)
        {
            var data = await GetDapper().GetBySiteNum(siteNum);
            var columns = data as Column[] ?? data.ToArray();
            var models = (await ModelTableService.Interface.GetAll())
                .ToDictionary(temp => temp.Num, temp => temp.Explain);
            return new PageResponse(await GetTreeTableData(columns.ToList(), models, ""), columns.Length);
        }

        private async Task<IEnumerable<dynamic>> GetTreeTableData(List<Column> columns,
            Dictionary<string, string> models,
            string parentNum)
        {
            var child = columns.FindAll(temp => string.Equals(temp.ParentNum ?? "", parentNum ?? ""));
            if (child.Count <= 0) return null;

            List<dynamic> data = new List<dynamic>();
            foreach (var column in child)
            {
                columns.Remove(column);

                var item = new ExpandoObject();

                item.TryAdd("Id", column.Id);
                item.TryAdd("Num", column.Num);
                item.TryAdd("Name", column.Name);
                item.TryAdd("ParentNum", column.ParentNum);
                item.TryAdd("CreateDate", column.CreateDate);
                item.TryAdd("ModelNum", column.ModelNum);
                item.TryAdd("ModelName", models.ContainsKey(column.ModelNum) ? models[column.ModelNum] : "");

                var children = await GetTreeTableData(columns, models, column.Num);
                if (children != null && children.Any())
                    item.TryAdd("children", children);

                data.Add(item);
            }

            return data;
        }
    }
}