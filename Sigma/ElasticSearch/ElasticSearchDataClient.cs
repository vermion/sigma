using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Sigma.Backgroundservices;
using Sigma.Models;
using System;
using System.Threading.Tasks;

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

            try
            {
                await CheckForNewData();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
        }

        /// <summary>
        /// Checks if any new data is available to index. If any date is found in the blob that doesn't exist in ElasticSearch it gets added.
        /// </summary>
        /// <returns></returns>
        private async Task CheckForNewData()
        {

            foreach (var item in _retrieveSensorDataClient._metaSensorData)
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
        }

    }
}
