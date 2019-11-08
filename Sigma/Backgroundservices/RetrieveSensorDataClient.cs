using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Sigma.Models;
using Sigma.SensorDataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Backgroundservices
{
    public class RetrieveSensorDataClient
    {
        private readonly IOptions<GeneralSettings> _generalSettings;
        private readonly ILogger<RetrieveSensorDataClient> _logger;

        public List<SensorMetaData> _metaSensorData { get; set; }

        public RetrieveSensorDataClient(IOptions<GeneralSettings> generalSettings, 
                                        ILogger<RetrieveSensorDataClient> logger)
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
                await ListBlobsFlatListingAsync(cloudBlobContainer, null);
            }
            else
            {
                _logger.LogError("Failed to parse connection string");
            }
        }

        private async Task ListBlobsFlatListingAsync(CloudBlobContainer container, int? segmentSize)
        {
            BlobContinuationToken continuationToken = null;
            CloudBlob blob;

            try
            {
                do
                {
                    BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, segmentSize, continuationToken, null, null);

                    _metaSensorData = new List<SensorMetaData>();
                    foreach (var blobItem in resultSegment.Results)
                    {
                        // A flat listing operation returns only blobs, not virtual directories.
                        blob = (CloudBlob)blobItem;

                        using (var mStream = new MemoryStream())
                        {
                            if (!blob.Name.Contains("historical") && !blob.Name.Contains("metadata"))
                            {
                                SensorMetaData sensorMetaData = null;
                                if (blob.Name.Contains("humidity"))
                                {
                                    sensorMetaData = new Humidity();
                                }
                                else if (blob.Name.Contains("rainfall"))
                                {
                                    sensorMetaData = new Rainfall();
                                }
                                else if (blob.Name.Contains("temperature"))
                                {
                                    sensorMetaData = new Temperature();
                                }

                                await ParseSensorDataAndAddToDataStream(mStream, sensorMetaData, blob);
                            }
                        }

                        _logger.LogInformation($"Blob name {blob.Name}");
                    }

                    // Get the continuation token and loop until it is null.
                    continuationToken = resultSegment.ContinuationToken;

                } while (continuationToken != null);

            }
            catch (StorageException ex)
            {
                _logger.LogCritical(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Parses incoming data according to {DateTime}, {double}.
        /// Adds data to global list: SensorData.
        /// </summary>
        /// <param name="mStream"></param>
        /// <param name="sensorMetaData"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        private async Task ParseSensorDataAndAddToDataStream(MemoryStream mStream, SensorMetaData sensorMetaData, CloudBlob blob)
        {
            await blob.DownloadToStreamAsync(mStream);
            var byteStream = mStream.ToArray();
            string converted = Encoding.UTF8.GetString(byteStream, 0, byteStream.Length);

            // More robust using regx?

            var start = blob.Name.IndexOf(".csv") - 10;
            var measurementDate = blob.Name.Substring(start, 10);

            sensorMetaData.MeasurementDay = DateTime.Parse(measurementDate);
            sensorMetaData.SensorData = new List<SensorData>();

            var measData = new SensorData();
            measData.MeasurementData = new List<double>();
            measData.MeasurementTime = new List<DateTime>();

            string[] splitData = converted.Split("\r\n");
            foreach (var splitItem in splitData)
            {
                var finalSplit = splitItem.Split(";");

                try
                {
                    measData.MeasurementTime.Add(DateTime.Parse(finalSplit[0]));
                    measData.MeasurementData.Add(double.Parse(finalSplit[1]));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                }
            }

            sensorMetaData.SensorData.Add(measData);

            _metaSensorData.Add(sensorMetaData);
        }
    }
}
