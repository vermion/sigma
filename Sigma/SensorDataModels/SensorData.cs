using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sigma.SensorDataModels
{
    public class SensorData
    {
        public List<double> MeasurementData { get; set; }
        public List<DateTime> MeasurementTime { get; set; }
    }
}
