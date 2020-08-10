using System;
using System.Threading.Tasks;
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
using Service.Account;
using Service.CMS;

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
        }

        private static async Task AdminValidateFail(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(AdminCookieAttribute.Scheme);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            GlobalApplication.Inject(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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

                var site = await SiteService.Interface.GetByHost(host) ?? await SiteService.Interface.GetByDefault();
                SessionHelper.Set("CurrentSite", site);

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