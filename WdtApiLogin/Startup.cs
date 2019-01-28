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

namespace WdtApiLogin
{
    public class Startup
    {
        private readonly Lazy<string> _connectionString;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this._connectionString = new Lazy<string>(configuration.BuldConnectionString());
        }

        public IConfiguration Configuration { get; }

        private string ConnectionString => this._connectionString.Value;

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            UserManager<WdtApiLoginUser> userManager)
        {
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

            app.UseStatusCodePagesWithReExecute("/ErrorStatus/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            SeedData.SeedUsers(userManager);

            app.UseSession();

            app.UseMvc(
                routes =>
                    {
                        // New Route
                        routes.MapRoute(
                            name: "faq-route",
                            template: "faq",
                            defaults: new { controller = "Home", action = "Faq" });
                        routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                    });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddMvc(
                config =>
                    {
                        // using Microsoft.AspNetCore.Mvc.Authorization;
                        // using Microsoft.AspNetCore.Authorization;
                        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                        config.Filters.Add(new AuthorizeFilter(policy));
                    }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddSessionStateTempDataProvider();

            services.AddSession();

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
