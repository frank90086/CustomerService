using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omi.Education.Web.Management.Services;

namespace Omi.Education.Web.Management
{
    public class Startup
    {
        readonly string baseUri;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            baseUri = "http://notificationbyhub.azurewebsites.net/notificationhub?connectionType=Service";
            //baseUri = "http://localhost:10596/notificationhub?connectionType=Service";
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAntiforgery(opts => opts.Cookie.Name = "form-token");

            services.AddMvc();
            services.AddSingleton<ISupportService, SupportService>(x => { return new SupportService(baseUri); });
            services.AddSingleton<IScheduleService, ScheduleService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.Use(async(context, next) =>
            {
                var csp = "default-src 'self'; script-src * 'unsafe-inline'; child-src *; frame-src *; frame-ancestors *; style-src * 'unsafe-inline'; font-src *; img-src data: https: *; connect-src *;";

                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("Content-Security-Policy", csp);
                context.Response.Headers.Add("X-Content-Security-Policy", csp);
                context.Response.Headers.Add("X-WebKit-CSP", csp);

                await next();
            });

            app.UseStaticFiles();

            app.UseAuthentication();
            using(var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var supportService = serviceScope.ServiceProvider.GetService<ISupportService>();
                var scheduleService = serviceScope.ServiceProvider.GetService<IScheduleService>();
            }
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "Sign-Out",
                    "SignOut",
                    new { controller = "Home", action = "SignOut" });

                routes.MapRoute(
                    "AccessDenied",
                    "AccessDenied",
                    new { controller = "Home", action = "AccessDenied" });

                routes.MapRoute(
                    "default",
                    "{controller}/{action}/{id?}",
                    new { controller = "Customer", action = "Index" });
            });
        }
    }
}