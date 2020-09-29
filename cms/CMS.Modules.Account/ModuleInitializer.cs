using CMS.Modules.Account.Abstractions.Interface.Dapper;
using CMS.Modules.Account.Abstractions.Interface.Service;
using CMS.Modules.Account.DataAccess.MySql;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;
using Foundation.Modal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Service.Account;

namespace CMS.Modules.Account
{
    public class ModuleInitializer: IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection service)
        {
            service.AddTransient<IPageSqlHelper, MySqlPageHelper>();
            
            service.AddTransient<IAdminDapper, AdminDapper>();
            service.AddTransient<IRoleDapper, RoleDapper>();
            service.AddTransient<IPermissionsDapper, PermissionsDapper>();
            
            service.AddTransient<IAdminService, AdminService>();
            service.AddTransient<IPermissionsService, PermissionsService>();
            service.AddTransient<IRoleService, RoleService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }
    }
}