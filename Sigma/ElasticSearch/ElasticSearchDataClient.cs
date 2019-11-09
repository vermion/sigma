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

namespace Sigma.ElasticSearch
{
    public class ElasticSearchDataClient
    {
        private readonly IOptions<GeneralSettings> _generalSettings;
        private readonly ILogger<ElasticSearchDataClient> _logger;

        public List<SensorMetaData> _metaSensorData { get; set; }

        public ElasticSearchDataClient(IOptions<GeneralSettings> generalSettings, 
                                        ILogger<ElasticSearchDataClient> logger)
        {
            _logger = logger;
            _generalSettings = generalSettings;
        }

        public async Task UpdateAsync()
        { 
            _logger.LogInformation($"Background service to fetch sensor data triggered at: {DateTime.Now}");

            await ParseConnectionStringAndConnectoToBlob();
        }

        private async Task ParseConnectionStringAndConnectoToBlob()
        {
            // Retrieve the connection string for use with the application. 
            string storageConnectionString = _generalSettings.Value.AzureBlobConnectionString;

            // Check whether the connection string can be parsed.
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                var cloudBlobContainer = cloudBlobClient.GetContainerReference("iotbackend");
                // await ListBlobsFlatListingAsync(cloudBlobContainer, null);
            }
            else
            {
                _logger.LogError("Failed to parse connection string");
            }
        }

    }
}
