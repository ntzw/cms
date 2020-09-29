using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.DataAccess.MySql;
using CMS.Modules.Content.Service;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;
using Foundation.Modal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.Modules.Content
{
    public class ModuleInitializer: IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection service)
        {
            service.AddTransient<ICategoryService, CategoryService>();
            service.AddTransient<IColumnFieldService, ColumnFieldService>();
            service.AddTransient<IColumnService, ColumnService>();
            service.AddTransient<IContentService, ContentService>();
            service.AddTransient<IModelFieldService, ModelFieldService>();
            service.AddTransient<IModelTableService, ModelTableService>();
            service.AddTransient<ISiteService, SiteService>();

            service.AddTransient<ICategoryDapper, CategoryDapper>();
            service.AddTransient<IColumnFieldDapper, ColumnFieldDapper>();
            service.AddTransient<IColumnDapper, ColumnDapper>();
            service.AddTransient<IContentDapper, ContentDapper>();
            service.AddTransient<IModelFieldDapper, ModelFieldDapper>();
            service.AddTransient<IModelTableDapper, ModelTableDapper>();
            service.AddTransient<ISiteDapper, SiteDapper>();

            service.AddTransient<IPageSqlHelper, MySqlPageHelper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }
    }
}