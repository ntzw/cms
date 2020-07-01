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
            return new HandleResult
            {
                IsSuccess = true,
                Data = new
                {
                    EditData = data,
                    Field = ReactForm.ToFormFields<Administrator>()
                }
            };
        }

        public async Task<HandleResult> Edit([FromBody] Administrator model)
        {
            var info = model.Id > 0 ? await AdminService.Interface.GetById(model.Id) : new Administrator();

            info.Init();
            info.AccountName = model.AccountName;
            info.TrueName = model.TrueName;

            if (info.Password.IsEmpty() || info.Password != model.Password)
                info.Password = AdminService.Interface.PasswordToMd5(model.Password);

            return info.Id > 0 ? await AdminService.Interface.Update(info) : await AdminService.Interface.Add(info);
        }
    }
}