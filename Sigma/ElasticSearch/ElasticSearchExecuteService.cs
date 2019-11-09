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
        private readonly RetrieveSensorDataClient _retrieveSensorDataClient;

        public ElasticSearchExecuteService(RetrieveSensorDataClient retrieveSensorDataClient, 
                              IOptions<GeneralSettings> generalSettings)
        {
            _retrieveSensorDataClient = retrieveSensorDataClient;
            _generalSettings = generalSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
                await _retrieveSensorDataClient.UpdateAsync();
                await Task.Delay(TimeSpan.FromMinutes(_generalSettings.Value.BackgrousServiceUpdateIntervalInMinutes), cancellationToken);
            }
        }
    }
}
