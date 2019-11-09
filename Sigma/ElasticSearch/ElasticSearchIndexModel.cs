using Sigma.SensorDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sigma.ElasticSearch
{
    public class ElasticSearchIndexModel
    {
        public string SensorType { get; set; }
        public string DeviceID { get; set; }
        public List<SensorData> SensorData { get; set; }
        public DateTime MeasurementDay { get; set; }
    }
}
