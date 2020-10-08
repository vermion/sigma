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
using Sigma.ElasticSearch;
using Serilog.Events;
using Serilog.Core;
using Serilog.Sinks.Elasticsearch;
using MediatR;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.CodeAnalysis.Options;
using Swashbuckle.Swagger;

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
                   .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
            }
            else if (hostingEnvironment.IsProduction())
            {
                builder = new ConfigurationBuilder()
                    .SetBasePath(hostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
            }
            else if (hostingEnvironment.IsStaging())
            {
                builder = new ConfigurationBuilder()
                    .SetBasePath(hostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.Staging.json", optional: true, reloadOnChange: true);
            }
            else if (hostingEnvironment.EnvironmentName == "Test")
            {
                builder = new ConfigurationBuilder()
                    .SetBasePath(hostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true);
            }
            else
            {
                throw new Exception("NO ENVIRONMENT SET!");
            }

            var settings = configuration.GetSection("Logging").GetSection("LogLevel").GetValue("Default", "Information");

            var levelSwitch = new LoggingLevelSwitch();
            levelSwitch.MinimumLevel = settings switch
            {
                "Debug" => LogEventLevel.Debug,
                "Information" => LogEventLevel.Information,
                "Warning" => LogEventLevel.Warning,
                "Error" => LogEventLevel.Error,
                _ => LogEventLevel.Information,
            };
            var elasticUri = configuration["elasticsearch:uri"];
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "SIGMA")
                .Enrich.WithProperty("Environment", hostingEnvironment.EnvironmentName)
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    AutoRegisterTemplate = true,

                })
            .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GeneralSettings>(_configuration.GetSection("GeneralSettings"));

            services.AddControllers();

            services.AddSingleton<RetrieveSensorDataClient>();
            services.AddSingleton<IHostedService, ExecuteService>();

            services.AddSingleton<ElasticSearchDataClient>();
            services.AddSingleton<IHostedService, ElasticSearchExecuteService>();

            services.AddElasticsearch(_configuration);

            services.AddMediatR(typeof(Startup));

            services.AddSwaggerGen(x => 
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Sensor data API", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var swaggerOptions = new Swagger.SwaggerOptions();
            _configuration.GetSection(nameof(Swagger.SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);    
            });

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
