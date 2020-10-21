using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Modules.Account.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model.Content;
using CMS.React.Model;
using Extension;
using Foundation.Attribute;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Content.Controllers
{
    [Route("Api/CMS/[controller]/[action]")]
    [AdminAjax]
    public class ContentController : ControllerBase
    {
        private readonly IColumnService _columnService;
        private readonly IColumnFieldService _columnFieldService;
        private readonly IModelTableService _modelTableService;
        private readonly IAdminService _adminService;
        private readonly IContentService _service;

        public ContentController(
            IContentService service,
            IColumnService columnService,
            IColumnFieldService columnFieldService,
            IModelTableService modelTableService,
            IAdminService adminService)
        {
            _columnService = columnService;
            _columnFieldService = columnFieldService;
            _modelTableService = modelTableService;
            _adminService = adminService;
            _service = service;
        }

        public async Task<HandleResult> GetEdit([FromBody] JObject form)
        {
            var itemNum = form["itemNum"].ToStr();
            var columnNum = form["columnNum"].ToStr();

            if (columnNum.IsEmpty()) return HandleResult.Error("无效数据");

            var column = await _columnService.GetByNum(columnNum);
            if (column == null) return HandleResult.Error("无效数据");

            var model = await _modelTableService.GetByNum(column.ModelNum);
            if (model == null) return HandleResult.Error("栏目未绑定模型");

            ContentData editValue = null;
            if (column.IsSingle)
            {
                editValue = await _service.GetFirstByColumnNum(model.SqlTableName, column.Num);
            }
            else
            {
                if (itemNum.IsEmpty()) return HandleResult.Error("无效数据");
                editValue = await _service.GetByItem(model.SqlTableName, itemNum);
            }

            return new HandleResult
            {
                IsSuccess = true,
                Data = editValue.ToDictionary()
            };
        }

        public async Task<HandleResult> GetFields([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            if (columnNum.IsEmpty()) return HandleResult.Error("请选择栏目");

            var column = await _columnService.GetByNum(columnNum);
            if (column == null) return HandleResult.Error("栏目不存在");
            if (column.ModelNum.IsEmpty()) return HandleResult.Error("栏目未绑定模型");
            
            return new HandleResult
            {
                IsSuccess = true,
                Data = new
                {
                    fields = await _columnFieldService.GetByColumnNum(columnNum)
                }
            };
        }

        /// <summary>
        /// 提交内容数据
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<IActionResult> Submit([FromBody] JObject form)
        {
            var loginAdmin = await _adminService.GetByLoginAdmin();
            if (loginAdmin == null) return Unauthorized();

            return await _service.Edit(form, loginAdmin.Num);
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public Task<PageResponse> Page([FromBody] JObject form)
        {
            return _service.Page(form);
        }

        /// <summary>
        /// 回收站分页数据
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public Task<PageResponse> RecyclePage([FromBody] JObject form)
        {
            return _service.Page(form, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<HandleResult> SelectData([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            string labelFieldName = form["labelFieldName"].ToStr();
            string valueFieldName = form["valueFieldName"].ToStr();

            if (columnNum.IsEmpty() || labelFieldName.IsEmpty()) return HandleResult.Error("无效数据");

            List<SelectDataType> resData = new List<SelectDataType>();

            var data = await _service.GetByColumnNum(columnNum);
            var enumerable = data as dynamic[] ?? data.ToArray();
            if (enumerable.Any())
            {
                if (enumerable[0] is IDictionary<string, object> firstItem)
                {
                    if (!firstItem.ContainsKey(labelFieldName)) return HandleResult.Error("无效的显示列名");

                    valueFieldName = valueFieldName.IsNotEmpty() ? valueFieldName : "Num";
                    if (!firstItem.ContainsKey(valueFieldName)) return HandleResult.Error("无效的值列名");

                    foreach (var o in enumerable)
                    {
                        if (o is IDictionary<string, object> item)
                        {
                            resData.Add(new SelectDataType
                            {
                                Label = item[labelFieldName].ToStr(),
                                Value = item[valueFieldName]
                            });
                        }
                    }
                }
            }


            return new HandleResult
            {
                IsSuccess = true,
                Data = resData
            };
        }

        public async Task<HandleResult> CascaderData([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            string labelFieldName = form["labelFieldName"].ToStr();
            string currentFieldName = form["currentFieldName"].ToStr();

            if (columnNum.IsEmpty() || labelFieldName.IsEmpty() || currentFieldName.IsEmpty())
                return HandleResult.Error("无效数据");

            return await _service.GetCascaderData(columnNum, labelFieldName, currentFieldName);
        }

        public async Task<HandleResult> UpdateTopStatus([FromBody] JObject form)
        {
            string num = form["num"].ToStr();
            string columnNum = form["columnNum"].ToStr();
            bool isTop = form["isTop"].ToBoolean();
            if (num.IsEmpty() || columnNum.IsEmpty()) return HandleResult.Error("参数错误");

            return await _service.UpdateIsTop(columnNum, num, isTop);
        }

        /// <summary>
        /// 删除内容数据
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<HandleResult> Delete([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            if (!(form["ids"] is JArray ids) || ids.Count <= 0 || columnNum.IsEmpty())
                return HandleResult.Error("无效数据");

            var cm = await _columnService.GetModelByNum(columnNum);
            if (cm == null || cm?.ModelTable == null) return HandleResult.Error("无效参数");

            return await _service.Delete(cm?.ModelTable.SqlTableName,
                ids.Select(temp => temp.ToInt()).ToList());
        }

        /// <summary>
        /// 移入回收站
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<HandleResult> MoveRecycle([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            if (!(form["ids"] is JArray ids) || ids.Count <= 0 || columnNum.IsEmpty())
                return HandleResult.Error("无效数据");

            var cm = await _columnService.GetModelByNum(columnNum);
            if (cm == null || cm?.ModelTable == null) return HandleResult.Error("无效参数");

            return await _service.MoveRecycle(cm?.ModelTable.SqlTableName,
                ids.Select(temp => temp.ToInt()).ToList());
        }

        /// <summary>
        /// 移出回收站
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<HandleResult> RemovedRecycle([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr();
            if (!(form["ids"] is JArray ids) || ids.Count <= 0 || columnNum.IsEmpty())
                return HandleResult.Error("无效数据");

            var cm = await _columnService.GetModelByNum(columnNum);
            if (cm == null || cm?.ModelTable == null) return HandleResult.Error("无效参数");

            return await _service.Removed(cm?.ModelTable.SqlTableName,
                ids.Select(temp => temp.ToInt()).ToList());
        }
    }
}