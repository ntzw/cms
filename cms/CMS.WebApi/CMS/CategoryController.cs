using System.Threading.Tasks;
using CMS.React;
using Extension;
using Foundation.Attribute;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Model.CMS;
using Newtonsoft.Json.Linq;
using Service.Account;
using Service.CMS;

namespace WebApi.CMS
{
    [Route("Api/CMS/[controller]/[action]")]
    [AdminAjax]
    public class CategoryController : ControllerBase
    {
        public async Task<PageResponse> Page([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            if (columnNum.IsEmpty()) return PageResponse.Error("请选择栏目");

            return await CategoryService.Interface.GetTreeTableData(columnNum);
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            if (columnNum.IsEmpty()) return HandleResult.Error("无效数据");

            string parentNum = form["parentNum"].ToStr();
            int id = form["id"].ToInt();
            var data = id > 0
                ? await CategoryService.Interface.GetById(columnNum, id)
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
                Data = await CategoryService.Interface.GetCascaderData(columnNum)
            };
        }

        public async Task<IActionResult> Submit([FromBody] JObject form)
        {
            var loginAdmin = await AdminService.Interface.GetByLoginAdmin();
            if (loginAdmin == null) return Unauthorized();

            return await CategoryService.Interface.Edit(form, loginAdmin.Num);
        }

        public async Task<HandleResult> Delete([FromBody] JObject form)
        {
            string ids = form["ids"].ToStr();
            string columnNum = form["columnNum"].ToStr();

            if (ids.IsEmpty() || columnNum.IsEmpty()) return HandleResult.Error("无效参数");

            return await CategoryService.Interface.Delete(columnNum, ids);
        }
    }
}