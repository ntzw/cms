using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.React.Model;
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

        /// <summary>
        /// 提交内容数据
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<IActionResult> Submit([FromBody] JObject form)
        {
            var loginAdmin = await AdminService.Interface.GetByLoginAdmin();
            if (loginAdmin == null) return Unauthorized();

            return await ContentService.Interface.Edit(form, loginAdmin.Num);
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public Task<PageResponse> Page([FromBody] JObject form)
        {
            return ContentService.Interface.Page(form);
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

            var data = await ContentService.Interface.GetByColumnNum(columnNum);
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

            return await ContentService.Interface.GetCascaderData(columnNum, labelFieldName, currentFieldName);
        }
    }
}