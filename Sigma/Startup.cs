using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sigma.BackgroundServices;
using Sigma.Models;
using Serilog;
using Sigma.Backgroundservices;

namespace Sigma
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration, Microsoft.Extensions.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;


            IConfigurationBuilder builder;
            if (hostingEnvironment.IsDevelopment())
            {
                builder = new ConfigurationBuilder()
                   .SetBasePath(hostingEnvironment.ContentRootPath)
                   .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true);
            }
            else if (hostingEnvironment.IsProduction())
            {
                builder = new ConfigurationBuilder()
                    .SetBasePath(hostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.production.json", optional: true, reloadOnChange: true);
            }
            else if (hostingEnvironment.IsStaging())
            {
                builder = new ConfigurationBuilder()
                    .SetBasePath(hostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.staging.json", optional: true, reloadOnChange: true);
            }
            else if (hostingEnvironment.EnvironmentName == "Test")
            {
                builder = new ConfigurationBuilder()
                    .SetBasePath(hostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true);
            }
            else
                throw new Exception("NO ENVIRONMENT SET!");

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GeneralSettings>(_configuration.GetSection("GeneralSettings"));

            services.AddControllers();

            services.AddSingleton<RetrieveSensorDataClient>();
            services.AddSingleton<IHostedService, ExecuteService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            loggerFactory.AddSerilog();
        }
    }
}
