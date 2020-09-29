using System.Threading.Tasks;
using CMS.Modules.Account.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using CMS.React;
using Extension;
using Foundation.Attribute;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Content.Controllers
{
    [Route("Api/CMS/[controller]/[action]")]
    [AdminAjax]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly IAdminService _adminService;

        public CategoryController(ICategoryService service, IAdminService adminService)
        {
            _service = service;
            _adminService = adminService;
        }


        public async Task<PageResponse> Page([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            if (columnNum.IsEmpty()) return PageResponse.Error("请选择栏目");

            return await _service.GetTreeTableData(columnNum);
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            if (columnNum.IsEmpty()) return HandleResult.Error("无效数据");

            string parentNum = form["parentNum"].ToStr();
            int id = form["id"].ToInt();
            var data = id > 0
                ? await _service.GetById(columnNum, id)
                : new Category
                {
                    ParentNum = parentNum,
                };
            if (data == null) return HandleResult.Error("无效数据");

            return new HandleResult
            {
                IsSuccess = true,
                Data = new
                {
                    EditData = data,
                    Field = ReactForm.ToFormFields<Category>(data.Id > 0)
                }
            };
        }

        public async Task<HandleResult> CascaderData([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            if (columnNum.IsEmpty()) return HandleResult.Error("无效数据");

            return new HandleResult
            {
                IsSuccess = true,
                Data = await _service.GetCascaderData(columnNum)
            };
        }

        public async Task<IActionResult> Submit([FromBody] JObject form)
        {
            var loginAdmin = await _adminService.GetByLoginAdmin();
            if (loginAdmin == null) return Unauthorized();

            return await _service.Edit(form, loginAdmin.Num);
        }

        public async Task<HandleResult> Delete([FromBody] JObject form)
        {
            string ids = form["ids"].ToStr();
            string columnNum = form["columnNum"].ToStr();

            if (ids.IsEmpty() || columnNum.IsEmpty()) return HandleResult.Error("无效参数");

            return await _service.Delete(columnNum, ids);
        }
    }
}