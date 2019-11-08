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

        [HttpGet("{sensortype}")]
        public ActionResult GetProduct(string sensortype, DateTime startDate) 
        {
            
            var result = _retrieveSensorDataClient._metaSensorData.Where(x => x.GetType().Name.ToLower() == sensortype && x.MeasurementDay.Date.ToString("yyyy-MM-dd") == "2019-01-10").First();
            if (result == null)
                return NotFound();
            else
            {
                var temp = (Temperature)result;

                temp.ConvertCelsiusToFahrenheit();
                return Ok(result.SensorData);
            }
               
        }
    }
}
