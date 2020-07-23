using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Modal
{
    public class ResultBase : IActionResult
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var services = context.HttpContext.RequestServices;
            var executor = services.GetRequiredService<IActionResultExecutor<ResultBase>>();

            return executor.ExecuteAsync(context, this);
        }
    }
}