using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WdtApiLogin.Areas.Identity.Data;
using WdtApiLogin.Models;

[assembly: HostingStartup(typeof(WdtApiLogin.Areas.Identity.IdentityHostingStartup))]
namespace WdtApiLogin.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<WdtApiLoginContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("WdtApiLoginContextConnection")));

                services.AddDefaultIdentity<WdtApiLoginUser>()
                    .AddDefaultUI(UIFramework.Bootstrap4)
                    .AddEntityFrameworkStores<WdtApiLoginContext>();
            });
        }
    }
}