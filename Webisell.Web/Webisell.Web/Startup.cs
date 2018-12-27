using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webisell.Web.Commands;
using Webisell.Web.Configuration;

namespace Webisell.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddTransient(typeof(GetFilteredProductsCommand));
            services.AddTransient(typeof(SearchProductsCommand));
            services.AddTransient(typeof(GetFiltersViewModelCommand));
            services.AddTransient(typeof(GetDetailsViewModelCommand));
            services.AddTransient(typeof(ISettingsProvider), typeof(DefaultSettingsProvider));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.Map("/angular", action =>
            {
                action.Use(async (context, next) =>
                {
                    await context.Response.WriteAsync("<b>ANGULAR</b>");
                    await next();
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    //template: "{controller=Products}/{action=FilterPage}/{id?}");
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
