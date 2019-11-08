using System;
using System.Collections.Generic;
using System.Text;

namespace Sigma.SensorDataModels
{
    public abstract class SensorMetaData
    {
        public List<SensorData> SensorData { get; set; }
        public DateTime MeasurementDay { get; set; }
    }
}
