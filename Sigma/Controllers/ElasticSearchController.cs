using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using Sigma.Backgroundservices;
using Sigma.ElasticSearch;
using System;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sigma.Controllers
{
    /// <summary>
    /// This API is still a work in progress.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ElasticSearchController : ControllerBase
    {
        private readonly ILogger<ElasticSearchController> _logger;
        private readonly IElasticClient _elasticClient;
        private readonly RetrieveSensorDataClient _retrieveSensorDataClient;

        public ElasticSearchController(ILogger<ElasticSearchController> logger,
                                       IElasticClient elasticClient,
                                       RetrieveSensorDataClient retrieveSensorDataClient)
        {
            _logger = logger;
            _retrieveSensorDataClient = retrieveSensorDataClient;
            _elasticClient = elasticClient;
        }

        [HttpGet("fordevice")]
        public async Task<ActionResult> GetProduct(string deviceId, string sensorType, DateTime startDate)
        {
            var sensorData = await _elasticClient.SearchAsync<ElasticSearchIndexModel>(s => s.Index(Indices.Index("devicedata"))
                .From(0)
                .Size(2000)
                .MatchAll()
                .Explain()
            );

            if (sensorData.Total != 0)
                return Ok(sensorData.Documents);
            else
                return NotFound();
            
        }
    }
}
