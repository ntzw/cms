using System.Threading.Tasks;
using Foundation.Modal;
using Microsoft.AspNetCore.Mvc;
using Model.Account;
using Newtonsoft.Json.Linq;
using Service.Account;

namespace WebApi.Account
{
    [Route("api/Account/[controller]/[action]")]
    public class RoleController : ControllerBase
    {
        public Task<PageResponse> Page([FromBody] JObject form)
        {
            var req = new SqlServerPageRequest(form);
            return RoleService.Interface.Page(req);
        }
    }
}