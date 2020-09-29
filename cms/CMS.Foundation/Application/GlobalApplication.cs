using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Foundation.Application
{
    public static class GlobalApplication
    {
        public static IConfiguration Configuration { get; private set; }

        public static IHttpContextAccessor ContextAccessor { get; private set; }

        public static IMemoryCache Cache { get; private set; }

        public static IWebHostEnvironment Env { get; private set; }

        public static ILoggerFactory Log { get; private set; }

        public static void Inject(IApplicationBuilder app)
        {
            Configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            ContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            Cache = app.ApplicationServices.GetRequiredService<IMemoryCache>();
            Env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            Log = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
        }
    }
}