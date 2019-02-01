using System;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WdtApiLogin.Areas.Identity;
using WdtApiLogin.Areas.Identity.Data;
using WdtApiLogin.Models;
using WdtUtils;

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
                    var connString = context.Configuration.BuildConnectionString();

                    services.AddDbContext<WdtApiLoginContext>(
                        options => options.UseSqlServer(connString));

                    services.AddIdentity<WdtApiLoginUser, IdentityRole>()
                        .AddDefaultUI(UIFramework.Bootstrap4)
                        .AddEntityFrameworkStores<WdtApiLoginContext>()
                        .AddDefaultTokenProviders();

                    // https://github.com/aspnet/AspNetCore/issues/6069
                    services.AddAuthentication().AddGoogle(
                        o =>
                        {
                            o.ClientId = context.Configuration["Authentication:Google:ClientId"];
                            o.ClientSecret = context.Configuration["Authentication:Google:ClientSecret"];
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

                    services.Configure<IdentityOptions>(options =>
                    {
                        // Password settings.
                        options.Password.RequireDigit =
                            options.Password.RequireLowercase =
                                options.Password.RequireNonAlphanumeric =
                                    options.Password.RequireUppercase = false;
                        options.Password.RequiredLength = 2;
                        options.Password.RequiredUniqueChars = 1;

                        // Lockout settings.
                        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                        options.Lockout.MaxFailedAccessAttempts = 5;
                        options.Lockout.AllowedForNewUsers = false;

                        // User settings.
                        options.User.AllowedUserNameCharacters =
                            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        options.User.RequireUniqueEmail = true;
                    });
                });
        }
    }
}