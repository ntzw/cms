using System;
using System.Linq;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using CMS.React;
using Extension;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Content.Controllers
{
    [Route("Api/CMS/[controller]/[action]")]
    public class ColumnController : ControllerBase
    {
        private readonly IColumnService _service;

        public ColumnController(IColumnService service)
        {
            _service = service;
        }
        
        public async Task<PageResponse> Page([FromBody] JObject form)
        {
            string siteNum = form["siteNum"].ToStr();
            if (siteNum.IsEmpty()) return PageResponse.Error("请选择站点");

            return await _service.GetTreeTableData(siteNum);
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            string parentNum = form["parentNum"].ToStr();
            int id = form["id"].ToInt();
            var data = id > 0
                ? await _service.GetById(id)
                : new Column
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
                    Field = ReactForm.ToFormFields<Column>(data.Id > 0)
                }
            };
        }

        public async Task<HandleResult> Edit([FromBody] Column model)
        {
            var info = model.Id > 0 ? await _service.GetById(model.Id) : new Column();
            if (info == null) return HandleResult.Error("无效数据");
            if (model.ParentNum.IsNotEmpty() &&
                string.Equals(info.Num, model.ParentNum, StringComparison.OrdinalIgnoreCase))
                return HandleResult.Error("无效数据");

            info.Init();
            ReactForm.SetEditModelValue(info, model, info.Id > 0);

            info.SiteNum = model.SiteNum;
            if (info.ParentNum.IsEmpty())
                info.ParentNum = "";

            var res = info.Id > 0
                ? await _service.Update(info)
                : await _service.Add(info);
            return res;
        }

        public async Task<HandleResult> Delete([FromBody] JArray form)
        {
            if (form == null || form.Count <= 0) return HandleResult.Error("请选择要删除的数据");

            var deleteModels = form.Select(temp => new Column {Id = temp.ToInt()}).ToList();
            
            //todo 需要延期删除无用的数据，栏目内容、栏目字段
            return await _service.Delete(deleteModels);
        }

        public async Task<HandleResult> CascaderData([FromBody] JObject form)
        {
            string siteNum = form["siteNum"].ToStr();
            if (siteNum.IsEmpty()) return HandleResult.Error("请选择站点");

            return new HandleResult
            {
                IsSuccess = true,
                Data = await _service.GetCascaderData(siteNum)
            };
        }
    }
}