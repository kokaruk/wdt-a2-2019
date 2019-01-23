using System;
using System.Data.SqlClient;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WdtApiLogin.Areas.Identity;
using WdtApiLogin.Areas.Identity.Data;
using WdtApiLogin.Models;
using WdtApiLogin.Utils;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace WdtApiLogin.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(
                (context, services) =>
                    {
                        var connString = context.Configuration.BuldConnectionString();

                        services.AddDbContext<WdtApiLoginContext>(
                            options => options.UseSqlServer(connString));

                        services.AddIdentity<WdtApiLoginUser, IdentityRole>()
                            .AddDefaultUI(UIFramework.Bootstrap4)
                            .AddEntityFrameworkStores<WdtApiLoginContext>()
                            .AddDefaultTokenProviders(); ;

                        services.Configure<IdentityOptions>(options =>
                            {
                                // Password settings.
                                options.Password.RequireDigit = true;
                                options.Password.RequireLowercase = 
                                options.Password.RequireNonAlphanumeric =
                                options.Password.RequireUppercase = false;
                                options.Password.RequiredLength = 6;
                                options.Password.RequiredUniqueChars = 1;

                                // Lockout settings.
                                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                                options.Lockout.MaxFailedAccessAttempts = 5;
                                options.Lockout.AllowedForNewUsers = true;

                                // User settings.
                                options.User.AllowedUserNameCharacters =
                                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                                options.User.RequireUniqueEmail = true;
                            });
                    });
        }
    }
}
