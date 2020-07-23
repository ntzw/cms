using System.Threading.Tasks;
using Foundation.Modal;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Foundation.Attribute
{
    public class AdminAjaxAttribute : System.Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!(await AdminCookieAttribute.IsLogin(context.HttpContext)))
            {
                context.Result = new HandleResult
                {
                    IsSuccess = false,
                    Message = "登录状态已失效,请重新登录!",
                };
            }
        }
    }
}