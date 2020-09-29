using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using CMS.React;
using CMS.React.Model;
using Extension;
using Foundation.Modal;
using Foundation.Modal.Result;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Content.Controllers
{
    [Route("Api/CMS/[controller]/[action]")]
    public class SiteController : ControllerBase
    {
        private readonly ISiteService _service;

        public SiteController(ISiteService service)
        {
            _service = service;
        }
        
        public async Task<PageResponse> Page([FromBody] JObject form)
        {
            var req = new SqlServerPageRequest(form);
            return await _service.Page(req);
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            int id = form["id"].ToInt();
            var data = id > 0 ? await _service.GetById(id) : new Site();
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

        public async Task<HandleResult> Edit([FromBody]Site model)
        {
            var info = model.Id > 0 ? await _service.GetById(model.Id) : new Site();
            if (info == null) return HandleResult.Error("无效数据");

            info.Init();
            ReactForm.SetEditModelValue(info, model, info.Id > 0);

            var res = info.Id > 0 ? await _service.Update(info) : await _service.Add(info);

            if (res.IsSuccess)
            {
                if (info.IsDefault)
                    await _service.RemoveOtherDefault(info.Id);

                string siteFolderPath = Path.GetFullPath($"./Views/{info.SiteFolder}");
                if (!Directory.Exists(siteFolderPath))
                    Directory.CreateDirectory(siteFolderPath);
            }


            return res;
        }

        public async Task<HandleResult> Delete([FromBody] JArray form)
        {
            if (form == null || form.Count <= 0) return HandleResult.Error("请选择要删除的数据");

            var deleteModels = form.Select(temp => new Site {Id = temp.ToInt()}).ToList();
            return await _service.Delete(deleteModels);
        }

        public Task<HandleResult> SelectData()
        {
            return _service.GetSelectData();
        }
        
        public async Task<HandleResult> TemplatePath([FromBody] JObject form)
        {
            string siteNum = form["siteNum"].ToStr();
            if (siteNum.IsEmpty()) return HandleResult.Error("无效参数");

            var site = await _service.GetByNum(siteNum);
            if (site == null) return HandleResult.Error("无效参数");

            var siteFolderPath = Path.GetFullPath($"./Views/Content/{site.SiteFolder}/");
            if (!Directory.Exists(siteFolderPath)) return HandleResult.Error("无效参数");

            return new HandleResult
            {
                IsSuccess = true,
                Data = FileHelper.GetFolderAllChilden(siteFolderPath)?.Select(path =>
                {
                    var temp = path.Replace(siteFolderPath, "");
                    return new SelectDataType
                    {
                        Label = temp,
                        Value = temp
                    };
                })
            };
        }
    }
}