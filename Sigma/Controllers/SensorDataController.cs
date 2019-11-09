using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sigma.Backgroundservices;
using Sigma.SensorDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sigma.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SensorDataController : ControllerBase
    {
        private readonly ILogger<SensorDataController> _logger;
        private readonly RetrieveSensorDataClient _retrieveSensorDataClient;

        public SensorDataController(ILogger<SensorDataController> logger,
                                    RetrieveSensorDataClient retrieveSensorDataClient)
        {
            _logger = logger;
            _retrieveSensorDataClient = retrieveSensorDataClient;
        }

        [HttpGet("fordevice")]
        public ActionResult GetProduct(string deviceId, string sensorType, DateTime startDate) 
        {
            if (sensorType == null)
            {
                var sensorData = _retrieveSensorDataClient._metaSensorData.Where(x => x.DeviceID == deviceId && x.MeasurementDay.Date.ToString("yyyy-MM-dd") == startDate.Date.ToString("yyyy-MM-dd"));
                if (sensorData != null)
                    return Ok(sensorData);
                else
                    return NotFound();
            }

            var result = _retrieveSensorDataClient._metaSensorData.Where(x => x.GetType().Name.ToLower() == sensorType && x.DeviceID== deviceId && x.MeasurementDay.Date.ToString("yyyy-MM-dd") == startDate.Date.ToString("yyyy-MM-dd")).FirstOrDefault();
            if (result == null)
                return NotFound();
            else if (sensorType == "temperature")
            {
                var temp = (Temperature)result;

                temp.ConvertCelsiusToFahrenheit();
                return Ok(result.SensorData);
            }
            else
            {
                return Ok(result.SensorData);
            }
        }
    }
}
