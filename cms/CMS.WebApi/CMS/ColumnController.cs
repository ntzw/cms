using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CMS.React;
using CMS.React.Model;
using Extension;
using Foundation.Modal;
using Foundation.Modal.Result;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Model.CMS;
using Newtonsoft.Json.Linq;
using Service.CMS;

namespace WebApi.CMS
{
    [Route("Api/CMS/[controller]/[action]")]
    public class ColumnController : ControllerBase
    {
        public async Task<PageResponse> Page([FromBody] JObject form)
        {
            string siteNum = form["siteNum"].ToStr();
            if (siteNum.IsEmpty()) return PageResponse.Error("请选择站点");

            return await ColumnService.Interface.GetTreeTableData(siteNum);
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            string parentNum = form["parentNum"].ToStr();
            int id = form["id"].ToInt();
            var data = id > 0
                ? await ColumnService.Interface.GetById(id)
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
            var info = model.Id > 0 ? await ColumnService.Interface.GetById(model.Id) : new Column();
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
                ? await ColumnService.Interface.Update(info)
                : await ColumnService.Interface.Add(info);
            return res;
        }

        public async Task<HandleResult> Delete([FromBody] JArray form)
        {
            if (form == null || form.Count <= 0) return HandleResult.Error("请选择要删除的数据");

            var deleteModels = form.Select(temp => new Column {Id = temp.ToInt()}).ToList();
            
            //todo 需要延期删除无用的数据，栏目内容、栏目字段
            return await ColumnService.Interface.Delete(deleteModels);
        }

        public async Task<HandleResult> CascaderData([FromBody] JObject form)
        {
            string siteNum = form["siteNum"].ToStr();
            if (siteNum.IsEmpty()) return HandleResult.Error("请选择站点");

            return new HandleResult
            {
                IsSuccess = true,
                Data = await ColumnService.Interface.GetCascaderData(siteNum)
            };
        }
    }
}