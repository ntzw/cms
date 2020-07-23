/**
 * author：created by zw on 2020-06-20 14:28:19
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CMS.React.Model;
using DataAccess.Interface.Account;
using DataAccess.SqlServer.Account;
using Foundation.Modal;
using Foundation.Modal.Result;
using Model.Account;

namespace Service.Account
{
    public class RoleService : DefaultService<Role, IRoleDapper>
    {
        private IRoleDapper _dapper;

        protected override IRoleDapper GetDapper() => _dapper ??= new RoleDapper();

        private RoleService()
        {
        }

        private static RoleService _interface;
        public static RoleService Interface => _interface ??= new RoleService();

        /// <summary>
        /// 获取树形表格数据
        /// </summary>
        /// <returns></returns>
        public async Task<PageResponse> TreeTableData()
        {
            var data = await GetDapper().GetAll();
            var roles = data as Role[] ?? data.ToArray();
            return new PageResponse(GetTreeTableData(roles.ToList(), ""), roles.Length);
        }

        private IEnumerable<dynamic> GetTreeTableData(List<Role> roles, string parentNum)
        {
            var child = roles.FindAll(temp => string.Equals(temp.ParentNum ?? "", parentNum ?? ""));
            if (child.Count <= 0) return null;

            List<dynamic> data = new List<dynamic>();
            foreach (var role in child)
            {
                roles.Remove(role);

                var item = new ExpandoObject();

                item.TryAdd("Id", role.Id);
                item.TryAdd("Num", role.Num);
                item.TryAdd("Name", role.Name);
                item.TryAdd("ParentNum", role.ParentNum);
                item.TryAdd("Desc", role.Desc);
                item.TryAdd("CreateDate", role.CreateDate);

                var children = GetTreeTableData(roles, role.Num);
                if (children != null && children.Any())
                    item.TryAdd("children", children);

                data.Add(item);
            }

            return data;
        }

        public async Task<List<CascaderDataType>> GetCascaderData()
        {
            var data = await GetDapper().GetAll();
            var roles = data as Role[] ?? data.ToArray();
            return GetCascaderData(roles.ToList(), "");
        }

        private List<CascaderDataType> GetCascaderData(List<Role> roles, string parentNum)
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
    }
}