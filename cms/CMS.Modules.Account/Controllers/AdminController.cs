using System.Linq;
using System.Threading.Tasks;
using CMS.Modules.Account.Abstractions.Interface.Service;
using CMS.Modules.Account.Abstractions.Model;
using CMS.React;
using Extension;
using Foundation.Modal;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Service;
using Service.Account;

namespace CMS.Modules.Account.Controllers
{
    [Route("api/Account/[controller]/[action]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<PageResponse> Page([FromBody] JObject form)
        {
            var req = new SqlServerPageRequest(form);
            return await _adminService.Page(req);
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            int id = form["id"].ToInt();
            var data = id > 0 ? await _adminService.GetById(id) : new Administrator();
            if (data == null) return HandleResult.Error("无效数据");

            return new HandleResult
            {
                IsSuccess = true,
                Data = new
                {
                    EditData = data,
                    Field = ReactForm.ToFormFields<Administrator>(data.Id > 0)
                }
            };
        }

        public async Task<HandleResult> Edit([FromBody] Administrator model)
        {
            var info = model.Id > 0 ? await _adminService.GetById(model.Id) : new Administrator();
            if (info == null) return HandleResult.Error("无效数据");

            info.Init();
            ReactForm.SetEditModelValue(info, model, info.Id > 0);

            return info.Id > 0 ? await _adminService.Update(info) : await _adminService.Add(info);
        }

        public async Task<HandleResult> Delete([FromBody] JArray form)
        {
            if (form == null || form.Count <= 0) return HandleResult.Error("请选择要删除的数据");

            var deleteModels = form.Select(temp => new Administrator {Id = temp.ToInt()}).ToList();
            return await _adminService.Delete(deleteModels);
        }
    }
}