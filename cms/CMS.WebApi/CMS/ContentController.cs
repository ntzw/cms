using System.Threading.Tasks;
using Extension;
using Foundation.Attribute;
using Foundation.Modal;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Service.Account;
using Service.CMS;

namespace WebApi.CMS
{
    [Route("Api/CMS/[controller]/[action]")]
    [AdminAjax]
    public class ContentController : ControllerBase
    {
        public async Task<HandleResult> GetEdit([FromBody] JObject form)
        {
            var itemNum = form["itemNum"].ToStr();
            var columnNum = form["columnNum"].ToStr();

            if (columnNum.IsEmpty()) return HandleResult.Error("无效数据");

            var column = await ColumnService.Interface.GetByNum(columnNum);
            if (column == null) return HandleResult.Error("无效数据");

            var model = await ModelTableService.Interface.GetByNum(column.ModelNum);
            if (model == null) return HandleResult.Error("栏目未绑定模型");

            dynamic editValue = null;
            if (column.IsSingle)
            {
                editValue = await ContentService.Interface.GetFirstByColumnNum(model.SqlTableName, column.Num);
            }
            else
            {
                if (itemNum.IsEmpty()) return HandleResult.Error("无效数据");

                editValue = await ContentService.Interface.GetByNum(model.SqlTableName, itemNum);
            }

            return new HandleResult
            {
                IsSuccess = true,
                Data = editValue
            };
        }

        public async Task<HandleResult> GetFields([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            if (columnNum.IsEmpty()) return HandleResult.Error("请选择栏目");

            return new HandleResult
            {
                IsSuccess = true,
                Data = new
                {
                    fields = await ColumnFieldService.Interface.GetByColumnNum(columnNum)
                }
            };
        }

        public async Task<IActionResult> Submit([FromBody] JObject form)
        {
            var loginAdmin = await AdminService.Interface.GetByLoginAdmin();
            if (loginAdmin == null) return Unauthorized();

            return await ContentService.Interface.Edit(form, loginAdmin.Num);
        }

        public Task<PageResponse> Page([FromBody] JObject form)
        {
            return ContentService.Interface.Page(form);
        }
    }
}