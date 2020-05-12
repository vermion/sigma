using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using Sigma.Backgroundservices;
using Sigma.Queries;
using Sigma.SensorDataModels;
using System;
using System.Linq;


namespace Sigma.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SensorDataController : ControllerBase
    {
        private readonly ILogger<SensorDataController> _logger;
        private readonly RetrieveSensorDataClient _retrieveSensorDataClient;
        private readonly IMediator _mediator;

        public SensorDataController(ILogger<SensorDataController> logger,
                                    RetrieveSensorDataClient retrieveSensorDataClient,
                                    IMediator mediator)
        {
            _logger = logger;
            _retrieveSensorDataClient = retrieveSensorDataClient;
            _mediator = mediator;
        }

        [HttpGet("fordevice")]
        public ActionResult GetProduct(string deviceId, string sensorType, DateTime startDate) 
        {
            var query = new GetInMemorySensorDataQuery {
                DeviceId = deviceId,
                SensorType = sensorType,
                StartDate = startDate
            };

            var response = _mediator.Send(query);

            if (response != null)
                return Ok(response);
            else
                return NotFound();
        }
    }
}
