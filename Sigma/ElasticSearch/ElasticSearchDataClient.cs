using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Sigma.Models;
using Sigma.SensorDataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Sigma.Backgroundservices;

namespace Sigma.ElasticSearch
{
    public class ElasticSearchDataClient
    {
        private readonly IOptions<GeneralSettings> _generalSettings;
        private readonly ILogger<ElasticSearchDataClient> _logger;
        private readonly RetrieveSensorDataClient _retrieveSensorDataClient;
        private readonly IElasticClient _elasticClient;

        public ElasticSearchDataClient(IOptions<GeneralSettings> generalSettings, 
                                        ILogger<ElasticSearchDataClient> logger,
                                        RetrieveSensorDataClient retrieveSensorDataClient,
                                        IElasticClient elasticClient)
        {
            _logger = logger;
            _generalSettings = generalSettings;
            _retrieveSensorDataClient = retrieveSensorDataClient;
            _elasticClient = elasticClient;
        }

        public async Task UpdateAsync()
        { 
            _logger.LogInformation($"Background service to index ElasticSearch triggered at: {DateTime.Now}");

            await CheckForNewData();
        }

        /// <summary>
        /// Checks if any new data is available to index. If any date is found in the blob that doesn't exist in ElasticSearch it gets added.
        /// </summary>
        /// <returns></returns>
        private async Task CheckForNewData()
        {

            foreach (var item in _retrieveSensorDataClient._metaSensorData)
            {
                try
                {
                    var response = await _elasticClient.SearchAsync<ElasticSearchIndexModel>(s => s.Index(Indices.Index("devicedata"))
                        .Query(query => query
                        .Term(f => f.MeasurementDay, item.MeasurementDay))
                        .Size(1)
                        .Explain()
                    );

                    if (response == null)
                    {
                        var esim = new ElasticSearchIndexModel();
                        esim.DeviceID = item.DeviceID;
                        esim.SensorData = item.SensorData;
                        esim.SensorType = item.GetType().Name;
                        esim.MeasurementDay = item.MeasurementDay;
                        await _elasticClient.IndexDocumentAsync(esim);
                        _logger.LogInformation($"Adding new measurement: {item.MeasurementDay} for {item.GetType().Name}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                }
            }
        }

    }
}
