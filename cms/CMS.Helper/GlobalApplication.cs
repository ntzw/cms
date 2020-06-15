using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Helper
{
    public static class GlobalApplication
    {
        private static IConfiguration _config;
        private static IHttpContextAccessor _accessor;
        private static IMemoryCache _cache;
        private static IHostingEnvironment _env;
        private static ILoggerFactory _log;

        public static void Inject(IApplicationBuilder app)
        {
            _config = app.ApplicationServices.GetRequiredService<IConfiguration>();
            _accessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            _cache = app.ApplicationServices.GetRequiredService<IMemoryCache>();
            _env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            _log = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
        }

        public static IConfiguration Configuration => _config;

        public static IHttpContextAccessor ContextAccessor => _accessor;

        public static IMemoryCache Cache => _cache;

        public static IHostingEnvironment Env => _env;

        public static ILoggerFactory Log => _log;
    }
}