﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using AsrApp.Utils;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WdtApiLogin.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WdtApiLogin
{
    public class Startup
    {

        private readonly Lazy<string> _connectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            this._connectionString = new Lazy<string>(
                () =>
                    {
                        try
                        {
                            var secrets = this.Configuration.GetSection(nameof(DbSecrets)).Get<DbSecrets>();
                            var sqlString =
                                new SqlConnectionStringBuilder(this.Configuration.GetConnectionString("wdtA2"))
                                    {
                                        UserID = secrets.Uid,
                                        Password = secrets.Password
                                    };
                            return sqlString.ConnectionString;
                        }
                        catch (Exception)
                        {
                            var sqlString = new SqlConnectionStringBuilder(
                                this.Configuration.GetConnectionString("wdtA2Production"));
                            return sqlString.ConnectionString;
                        }
                    });
        }

        public IConfiguration Configuration { get; }
        private string ConnectionString => this._connectionString.Value;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseSqlServer(this.ConnectionString));
            // services.AddDefaultIdentity<IdentityUser>()
            //     .AddDefaultUI(UIFramework.Bootstrap4)
            //     .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
