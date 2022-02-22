using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrgChart.API.BackgroundServices;
using OrgChart.API.DTOs;
using OrgChart.API.Middlewares;
using OrgChart.API.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lib = SharepointCSOMLib;

namespace OrgChart.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            services.AddScoped<Lib.IListManager>((s) => GetSPListManager());

            services.AddScoped<IMicrosoftGraphService, MicrosoftGraphService>();
            services.AddScoped<ISharePointService, SharePointService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IReportService, ReportService>();

            services.AddHostedService<ReportBackgroundService>();
            services.AddHostedService<AzureADPollingService>();

            services.Configure<AzureADSettings>(Configuration.GetSection("AzureAD"));
            services.Configure<SharePointSettings>(Configuration.GetSection("SharePoint"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log.txt");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }else
            {
                app.UseHsts();
            }

            
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod());

            app.UseMiddleware<AuthMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private Lib.ListManager GetSPListManager()
        {
            var siteUrl = Configuration["SharePoint:SiteUrl"];
            var clientId = Configuration["SharePoint:ClientId"];
            var clientSecret = Configuration["SharePoint:ClientSecret"];
            var tenant = Configuration["SharePoint:Tenant"];
            var resource = Configuration["SharePoint:Resource"];
            var grantType = Configuration["SharePoint:GrantType"];

            var auth = new Lib.AuthenticationManager(siteUrl, grantType, resource, clientId, clientSecret, tenant);
            return new Lib.ListManager(auth);
        }
    }
}
