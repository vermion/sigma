using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Sigma.SensorDataModels
{
    public class Temperature : SensorMetaData
    {
        public List<SensorData> ConvertCelsiusToFahrenheit()
        {

            foreach (var sensorData in SensorData)
            {
                sensorData.MeasurementData = sensorData.MeasurementData.Select(x => 9 * x / 5 + 32).ToList();
            }

            return SensorData;
        }
    }
}
