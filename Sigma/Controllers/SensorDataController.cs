using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sigma.Queries;
using System;


namespace Sigma.Controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class SensorDataController : ControllerBase
    {
        private readonly ILogger<SensorDataController> _logger;
        private readonly IMediator _mediator;

        public SensorDataController(ILogger<SensorDataController> logger,
                                    IMediator mediator)
        {
            _logger = logger;
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
