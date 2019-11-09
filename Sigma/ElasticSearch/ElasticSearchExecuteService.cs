using System;
using System.Threading;
using System.Threading.Tasks;
using Sigma.Models;
using Microsoft.Extensions.Options;
using Sigma.Backgroundservices;

namespace Sigma.ElasticSearch
{
    public class ElasticSearchExecuteService : ElasticSearchHostedService
    {
        private readonly IOptions<GeneralSettings> _generalSettings;
        private readonly ElasticSearchDataClient _elasticSearchDataClient;

        public ElasticSearchExecuteService(IOptions<GeneralSettings> generalSettings,
                                           ElasticSearchDataClient elasticSearchDataClient)
        {
            _elasticSearchDataClient = elasticSearchDataClient;
            _generalSettings = generalSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
                await _elasticSearchDataClient.UpdateAsync();
                await Task.Delay(TimeSpan.FromHours(_generalSettings.Value.BackgrousServiceUpdateIntervalInMinutes), cancellationToken);
            }
        }
    }
}
