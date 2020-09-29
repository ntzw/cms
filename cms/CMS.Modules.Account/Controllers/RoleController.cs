using System;
using System.Linq;
using System.Threading.Tasks;
using CMS.Modules.Account.Abstractions.Interface.Service;
using CMS.Modules.Account.Abstractions.Model;
using CMS.React;
using Extension;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Service;
using Service.Account;

namespace CMS.Modules.Account.Controllers
{
    [Route("Api/Account/[controller]/[action]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }
        
        public Task<PageResponse> Page()
        {
            return _service.TreeTableData();
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            var id = form["id"].ToInt();
            var role = id > 0 ? await _service.GetById(id) : new Role();
            if (role == null) return HandleResult.Error("无效数据");

            return HandleResult.Success(new
            {
                EditData = role,
                Field = ReactForm.ToFormFields<Role>(role.Id > 0)
            });
        }

        public async Task<HandleResult> Edit([FromBody] Role model)
        {
            var info = model.Id > 0 ? await _service.GetById(model.Id) : new Role();
            if (info == null) return HandleResult.Error("无效数据");
            if (string.Equals(info.Num, model.ParentNum, StringComparison.OrdinalIgnoreCase))
                return HandleResult.Error("无效数据");
            
            info.Init();
            info.Name = model.Name;
            info.Desc = model.Desc;
            info.ParentNum = model.ParentNum;

            return info.Id > 0 ? await _service.Update(info) : await _service.Add(info);
        }

        public async Task<HandleResult> Delete([FromBody] JArray form)
        {
            if (form == null || form.Count <= 0) return HandleResult.Error("请选择要删除的数据");

            var roles = form.Select(temp => new Role {Id = temp.ToInt()}).ToList();
            return await _service.Delete(roles);
        }

        public async Task<HandleResult> CascaderData([FromBody] JObject form)
        {
            return HandleResult.Success(await _service.GetCascaderData());
        }
    }
}