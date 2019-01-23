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

                        services.AddDefaultIdentity<WdtApiLoginUser>().AddDefaultUI(UIFramework.Bootstrap4)
                            .AddEntityFrameworkStores<WdtApiLoginContext>();
                    });
        }
    }
}
