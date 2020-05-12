using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sigma.ElasticSearch
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:index"];



            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex)
                .DefaultMappingFor<ElasticSearchIndexModel>(m => m
                    .PropertyName(p => p.SensorData, "SensorData")
                    .PropertyName(p => p.MeasurementDay, "MeasurementDay")
                    .PropertyName(p => p.DeviceID, "DeviceID")
                    .PropertyName(p => p.SensorType, "SensorType")
                );

            var client = new ElasticClient(settings);

            var result = client.Indices.Exists(defaultIndex);

            if (!result.Exists)
                client.Indices.Create(defaultIndex);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
