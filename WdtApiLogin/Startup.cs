using System;
using System.Data.SqlClient;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WdtApiLogin.Utils;
using WdtApiLogin.Utils.Model;

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"); });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<MyAppSettings>(this.Configuration.GetSection(nameof(MyAppSettings)));

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddSessionStateTempDataProvider();

            services.AddSession();

            // https://github.com/aspnet/AspNetCore/issues/6069
            services.AddAuthentication().AddGoogle(
                o =>
                    {
                        o.ClientId = Configuration["Authentication:Google:ClientId"];
                        o.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                        o.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                        o.ClaimActions.Clear();
                        o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                        o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                        o.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                        o.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                        o.ClaimActions.MapJsonKey("urn:google:profile", "link");
                        o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                        o.ClaimActions.MapJsonKey("urn:google:image", "picture");
                    });

            services.ConfigureApplicationCookie(
                options =>
                    {
                        // Cookie settings
                        options.Cookie.HttpOnly = true;
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                        options.SlidingExpiration = true;
                    });
        }
    }
}
