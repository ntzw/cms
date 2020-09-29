using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Account.Abstractions.Interface.Service;
using CMS.Modules.Account.Abstractions.Model;
using Extension;
using Foundation.Attribute;
using Foundation.Attribute.AuthorizeModel;
using Foundation.Modal.Result;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Utils.Controllers
{
    [Route("api/Utils/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public LoginController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        
        [AdminAjax]
        public async Task<dynamic> GetCurrentAdmin()
        {
            var loginAdmin = await _adminService.GetByLoginAdmin();
            if (loginAdmin == null)
                return HandleResult.Error("");

            return GetLoginAdminData(loginAdmin);
        }

        public async Task<HandleResult> Account([FromBody] JObject form)
        {
            string password = form["password"].ToStr();
            string accountName = form["userName"].ToStr();
            if (accountName.IsEmpty() || password.IsEmpty()) return HandleResult.Error("提交参数有误");

            var admin = await _adminService.GetByAccountName(accountName);
            if (admin == null || admin.Password != _adminService.PasswordToMd5(password))
                return HandleResult.Error("账号密码有误");

            SetAdminLoginCookie(admin);
            return GetLoginAdminData(admin);
        }

        private HandleResult GetLoginAdminData(Administrator admin)
        {
            return HandleResult.Success(new
            {
                admin.AccountName,
                admin.TrueName,
                admin.Num,
                authority = new List<string>()
            });
        }

        private void SetAdminLoginCookie(Administrator administrator)
        {
            string singleLoginToken = RandomHelper.CreateNum();
            AdminCookieAttribute.Login(ControllerContext.HttpContext, new AdminLogin
            {
                Id = administrator.Id.ToString(),
                RoleNum = administrator.GroupNum,
                Name = administrator.TrueName,
                SingleLoginToken = singleLoginToken,
                AccountName = administrator.AccountName,
                Num = administrator.Num
            });
        }
    }
}