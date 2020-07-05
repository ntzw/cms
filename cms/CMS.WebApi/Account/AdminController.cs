using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.React;
using Extension;
using Foundation.Modal;
using Foundation.Result;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Model.Account;
using Newtonsoft.Json.Linq;
using Service.Account;

namespace WebApi.Account
{
    [Route("api/Account/[controller]/[action]")]
    public class AdminController : ControllerBase
    {
        public async Task<PageResponse> Page([FromBody] JObject form)
        {
            var req = new SqlServerPageRequest(form);
            return await AdminService.Interface.Page(req);
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            int id = form["id"].ToInt();
            var data = id > 0 ? await AdminService.Interface.GetById(id) : new Administrator();
            if (data == null) return HandleResult.Error("无效数据");

            return new HandleResult
            {
                IsSuccess = true,
                Data = new
                {
                    EditData = data,
                    Field = ReactForm.ToFormFields<Administrator>(data.Id > 0)
                }
            };
        }

        public async Task<HandleResult> Edit([FromBody] Administrator model)
        {
            var info = model.Id > 0 ? await AdminService.Interface.GetById(model.Id) : new Administrator();
            if (info == null) return HandleResult.Error("无效数据");

            info.Init();
            info.AccountName = model.AccountName;
            info.TrueName = model.TrueName;

            if (info.Id == 0)
                info.Password = AdminService.Interface.PasswordToMd5(model.Password);

            return info.Id > 0 ? await AdminService.Interface.Update(info) : await AdminService.Interface.Add(info);
        }
    }
}