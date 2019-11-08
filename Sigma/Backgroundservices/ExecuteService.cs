using System;
using System.Threading;
using System.Threading.Tasks;
using Sigma.Models;
using Microsoft.Extensions.Options;
using Sigma.Backgroundservices;

namespace Sigma.BackgroundServices
{
    public class ExecuteService : HostedService
    {
        private readonly IOptions<GeneralSettings> _generalSettings;
        private readonly RetrieveSensorDataClient _retrieveSensorDataClient;

        public ExecuteService(RetrieveSensorDataClient retrieveSensorDataClient, 
                              IOptions<GeneralSettings> generalSettings)
        {
            _retrieveSensorDataClient = retrieveSensorDataClient;
            _generalSettings = generalSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _retrieveSensorDataClient.UpdateAsync();
                await Task.Delay(TimeSpan.FromMinutes(_generalSettings.Value.BackgrousServiceUpdateIntervalInMinutes), cancellationToken);
            }
        }
    }
}
