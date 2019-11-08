using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sigma.Backgroundservices;
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

        [HttpGet("{sensortype}")]
        public ActionResult GetProduct(string sensortype) 
        {

            var result = _retrieveSensorDataClient._metaSensorData.Where(x => x.GetType().Name.ToLower() == sensortype && x.MeasurementDay.Date.ToString("yyyy-MM-dd") == "2019-01-10").FirstOrDefault();
            if (result == null)
                return NotFound();
            else
                return Ok(result.SensorData);

            //foreach (var item in _retrieveSensorDataClient._sensorData)
            //{
            //    if (item.GetType().Name.ToLower() == sensortype.ToLower() && item.MeasurementDay.Date.ToString("yyyy-MM-dd") == "2019-01-10")
            //        _logger.LogInformation($"Is temperature on date: {item.MeasurementDay.ToString()}");
            //}
            //return Ok();
        }
    }
}
