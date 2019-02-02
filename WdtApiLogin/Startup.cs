using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WdtApiLogin.Areas.Identity.Data;
using WdtApiLogin.Repo;
using WdtUtils;
using WdtUtils.Model;

namespace WdtApiLogin
{
    public class Startup
    {
        private readonly Lazy<string> _connectionString;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this._connectionString = new Lazy<string>(configuration.BuildConnectionString());
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            UserManager<WdtApiLoginUser> userManager)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePages();
            
            app.UseStatusCodePagesWithReExecute("/ErrorStatus/{0}");

            app.UseHttpsRedirection();
            
            app.UseAuthentication();

            app.UseSession();

            app.UseMvc(
                routes =>
                {
                    // New Route
                    routes.MapRoute(
                        name: "faq-route",
                        template: "faq",
                        defaults: new {controller = "Home", action = "Faq"});
                    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GenericSettingsModel>(Configuration.GetSection("GenericSettings"));

            services.AddMvc(
                config =>
                {
                    // using Microsoft.AspNetCore.Mvc.Authorization;
                    // using Microsoft.AspNetCore.Authorization;
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddSessionStateTempDataProvider();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireStudentRole", policy => policy.RequireRole(UserConstants.Student));
                options.AddPolicy("RequireStaffRole", policy => policy.RequireRole(UserConstants.Staff));
            });

            services.AddDistributedMemoryCache();

            services.AddSession(
                options =>
                {
                    options.IdleTimeout = TimeSpan.FromSeconds(40);
                    options.Cookie.HttpOnly = true;
                });

            services.ConfigureApplicationCookie(
                options =>
                {
                    // Cookie settings
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    options.SlidingExpiration = true;
                });

            services.AddHttpClient<IApiService, ApiService>(
                    c => c.BaseAddress = new Uri(this.Configuration["WebApiUrl"]))
                .SetHandlerLifetime(TimeSpan.FromMinutes(10));
        }
    }
}