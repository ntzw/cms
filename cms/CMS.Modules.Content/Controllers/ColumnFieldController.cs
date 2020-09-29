using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using CMS.React;
using Extension;
using Foundation.Modal;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Content.Controllers
{
    [Route("Api/CMS/[controller]/[action]")]
    public class ColumnFieldController : ControllerBase
    {
        private readonly IColumnFieldService _service;
        private readonly IModelFieldService _modelFieldService;

        public ColumnFieldController(IColumnFieldService service, IModelFieldService modelFieldService)
        {
            _service = service;
            _modelFieldService = modelFieldService;
        }

        public async Task<PageResponse> Page([FromBody] JObject form)
        {
            var req = new SqlServerPageRequest(form, new Dictionary<string, string>
            {
                {"Sort", "DESC"}
            });

            if (!req.ContainsQueryField("columnNum")) return PageResponse.Error("请选择栏目");
            return await _service.Page(req);
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            int id = form["id"].ToInt();
            var data = id > 0 ? await _service.GetById(id) : new ColumnField();
            if (data == null) return HandleResult.Error("无效数据");

            return new HandleResult
            {
                IsSuccess = true,
                Data = new
                {
                    EditData = data,
                    Field = ReactForm.ToFormFields<ColumnField>(data.Id > 0)
                }
            };
        }

        public async Task<HandleResult> GetEditValue([FromBody] JObject form)
        {
            int id = form["id"].ToInt();
            if (id <= 0) return HandleResult.Error("无效参数");

            var info = await _service.GetById(id);
            if (info == null) return HandleResult.Error("无效参数");

            return new HandleResult
            {
                IsSuccess = true,
                Data = info,
            };
        }

        public async Task<HandleResult> Edit([FromBody] ColumnField model)
        {
            if (model.Id <= 0) return HandleResult.Error("无效数据");

            var info = await _service.GetById(model.Id);
            if (info == null) return HandleResult.Error("无效数据");

            info.Explain = model.Explain;
            info.Options = model.Options;

            return await _service.Update(info);
        }

        public async Task<HandleResult> MoveModelField([FromBody] JObject form)
        {
            string columnNum = form["columnNum"].ToStr(true);
            if (columnNum.IsEmpty()) return HandleResult.Error("请选择栏目");
            if (!(form["fieldNums"] is JArray jFieldNums) || jFieldNums.Count <= 0) return HandleResult.Error("请选择字段");

            var fields = await _modelFieldService.GetByNum(jFieldNums.Select(temp => temp.ToStr()).ToArray());
            var columnNumStrings = columnNum.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

            var modelFieldNum = new List<string>();
            var columnFields = new List<ColumnField>();
            foreach (var field in fields)
            {
                foreach (var columnNumString in columnNumStrings)
                {
                    var info = new ColumnField
                    {
                        ColumnNum = columnNumString,
                        Name = field.Name,
                        ModelFieldNum = field.Num,
                        Explain = field.Explain,
                        OptionType = field.OptionType,
                        DataType = field.DataType,
                        Options = field.Options,
                    };

                    info.Init();

                    columnFields.Add(info);
                    modelFieldNum.Add(field.Num);
                }
            }

            if (columnNumStrings.Length <= 1)
            {
                var verify = await _service.GetByModelFieldNum(columnNumStrings[0], modelFieldNum);
                if (verify.Any()) return HandleResult.Error("选择的字段有的已存在");
            }

            return await _service.Add(columnFields);
        }

        public async Task<HandleResult> Delete([FromBody] JArray form)
        {
            if (form == null || form.Count <= 0) return HandleResult.Error("请选择要删除的数据");

            var deleteModels = form.Select(temp => new ColumnField {Id = temp.ToInt()}).ToList();
            return await _service.Delete(deleteModels);
        }

        public async Task<HandleResult> Clear([FromBody] JArray nums)
        {
            if (nums == null || nums.Count <= 0) return HandleResult.Error("请选择要清空的数据");
            return await _service.Clear(nums.Select(temp => temp.ToStr()).ToArray());
        }

        public async Task<HandleResult> Sort([FromBody] JObject form)
        {
            string num = form["num"].ToStr();
            int sort = form["sort"].ToInt();

            if (num.IsEmpty()) return HandleResult.Error("无效参数");

            var info = await _service.GetByNum(num);
            if (info == null) return HandleResult.Error("无效参数");

            info.Sort = sort;
            return await _service.Update(info);
        }
    }
}