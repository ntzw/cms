using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Model.Account;
using Service.Account;

namespace WebApi.Account
{
    [Route("api/account/[controller]/[action]")]
    public class AdminController : ControllerBase
    {
        public async Task<Administrator> Get()
        {
            var admin = await AdminService.Interface.GetById(1);

            return admin;
        }
    }
}