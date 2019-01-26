using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using NSwag.AspNetCore;

using WdtA2Api.Data;
using WdtUtils;

namespace WdtA2Api
{
    public class Startup
    {
        private readonly Lazy<string> _connectionString;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this._connectionString = new Lazy<string>(() => Configuration.BuldConnectionString());
        }

        public IConfiguration Configuration { get; }

        private string ConnectionString => this._connectionString.Value;

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUi3();
            }

            if (env.IsProduction() || env.IsStaging() || env.IsEnvironment("Staging_2"))
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHealthChecks("/ready");
            

            // Shows UseCors with CorsPolicyBuilder.
            app.UseCors(builder =>
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin());

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // using net core 2_2 features as per https://www.youtube.com/watch?v=_vw3hcnSA1Y&t=420s
        // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-2.2&tabs=visual-studio%2Cvisual-studio-xml
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(
                options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register the Swagger services
            services.AddSwaggerDocument();

            //add CORS support
            services.AddCors();

            services.AddDbContext<WdtA2ApiContext>(
                options => options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(
                        this.ConnectionString,
                        opt => opt.EnableRetryOnFailure()));
            services.AddHealthChecks().AddDbContextCheck<WdtA2ApiContext>();
        }
    }
}
