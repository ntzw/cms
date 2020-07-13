using System.Linq;
using System.Threading.Tasks;
using CMS.React;
using Extension;
using Foundation.Modal;
using Microsoft.AspNetCore.Mvc;
using Model.Account;
using Model.CMS;
using Newtonsoft.Json.Linq;
using Service.CMS;

namespace WebApi.CMS
{
    [Route("Api/CMS/[controller]/[action]")]
    public class SiteController : ControllerBase
    {
        public async Task<PageResponse> Page([FromBody] JObject form)
        {
            var req = new SqlServerPageRequest(form);
            return await SiteService.Interface.Page(req);
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            int id = form["id"].ToInt();
            var data = id > 0 ? await SiteService.Interface.GetById(id) : new Site();
            if (data == null) return HandleResult.Error("无效数据");

            return new HandleResult
            {
                IsSuccess = true,
                Data = new
                {
                    EditData = data,
                    Field = ReactForm.ToFormFields<Site>(data.Id > 0)
                }
            };
        }

        public async Task<HandleResult> Edit([FromBody] Site model)
        {
            var info = model.Id > 0 ? await SiteService.Interface.GetById(model.Id) : new Site();
            if (info == null) return HandleResult.Error("无效数据");

            info.Init();
            ReactForm.SetEditModelValue(info, model, info.Id > 0);

            var res = info.Id > 0 ? await SiteService.Interface.Update(info) : await SiteService.Interface.Add(info);
            if (info.IsDefault && res.IsSuccess)
                await SiteService.Interface.RemoveOtherDefault(info.Id);

            return res;
        }

        public async Task<HandleResult> Delete([FromBody] JArray form)
        {
            if (form == null || form.Count <= 0) return HandleResult.Error("请选择要删除的数据");

            var deleteModels = form.Select(temp => new Site {Id = temp.ToInt()}).ToList();
            return await SiteService.Interface.Delete(deleteModels);
        }

        public Task<HandleResult> SelectData()
        {
            return SiteService.Interface.GetSelectData();
        }
    }
}