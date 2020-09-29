using CMS.Modules.Utils.Abstractions.Interface.Dapper;
using CMS.Modules.Utils.Abstractions.Interface.Service;
using CMS.Modules.Utils.DataAccess.MySql;
using CMS.Modules.Utils.Service;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;
using Foundation.Modal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.Modules.Utils
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection service)
        {
            service.AddTransient<ICodeGenerateService, CodeGenerateService>();

            service.AddTransient<ICodeGenerateDapper, CodeGenerateDapperDapper>();

            service.AddTransient<IPageSqlHelper, MySqlPageHelper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }
    }
}