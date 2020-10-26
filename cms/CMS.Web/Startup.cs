using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using CMS.Modules.Account;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using Extension;
using Foundation.Application;
using Foundation.Attribute;
using Foundation.ControllerFormatter;
using Foundation.Modal;
using Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionResultExecutor<ResultBase>, ResultBaseExecutor>();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            services.AddAuthentication().AddCookie(AdminCookieAttribute.Scheme, o =>
            {
                o.ClaimsIssuer = AdminCookieAttribute.ClaimsIssuer;
                o.LoginPath = new PathString("/Admin/Login");

                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = async context =>
                    {
                        var admin = AdminCookieAttribute.GetLoginAdmin(context.Principal.Claims);
                        if (admin == null || admin.Id.ToInt() <= 0)
                        {
                            await AdminValidateFail(context);
                            return;
                        }

                        AdminCookieAttribute.SetLoginAdmin(admin);
                    }
                };
            });

            services.AddControllersWithViews(options =>
            {
                options.InputFormatters.Insert(0, new JsonInputFormatter());
            });

            services.AddRazorPages().AddRazorRuntimeCompilation();

            var baseType = typeof(IModuleInitializer);
            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var referencedAssemblies = System.IO.Directory.GetFiles(path, "*.dll").ToList().FindAll(temp =>
            {
                string fileName = Path.GetFileName(temp);
                return fileName.StartsWith("CMS.Modules");
            }).Select(Assembly.LoadFrom).ToArray();
            var types = referencedAssemblies
                .SelectMany(a => a.DefinedTypes)
                .Select(type => type.AsType())
                .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
            var implementTypes = types.Where(x => x.IsClass).ToArray();
            foreach (var implementType in implementTypes)
            {
                services.AddSingleton(baseType, implementType);
            }

            var sp = services.BuildServiceProvider();
            var moduleInitializers = sp.GetServices<IModuleInitializer>();
            foreach (var moduleInitializer in moduleInitializers)
            {
                moduleInitializer.ConfigureServices(services);
            }
        }

        private static async Task AdminValidateFail(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(AdminCookieAttribute.Scheme);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var moduleInitializers = app.ApplicationServices.GetServices<IModuleInitializer>();
            foreach (var moduleInitializer in moduleInitializers)
            {
                moduleInitializer.Configure(app, env);
            }

            GlobalApplication.Inject(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.Use(async (content, next) =>
            {
                var host = content.Request.Host.Value;

                var currentSite = SessionHelper.Get<Site>("CurrentSite");
                if (currentSite == null)
                {
                    var siteService = app.ApplicationServices.GetService<ISiteService>();
                    var site = await siteService.GetByHost(host) ?? await siteService.GetByDefault();

                    site.IsMobileSite = site.IsEnableMobileSite && site.MobileHost.IsNotEmpty() && site.MobileHost
                        .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList()
                        .Exists(temp => string.Equals(temp, host));

                    SessionHelper.Set("CurrentSite", site);
                }

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Content}/{action=Index}/{id?}");
            });
        }
    }
}