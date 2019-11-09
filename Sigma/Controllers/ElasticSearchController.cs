using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using Sigma.Backgroundservices;
using Sigma.ElasticSearch;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sigma.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            if (sensorType == null)
            {
                var sensorData = await _elasticClient.SearchAsync<ElasticSearchIndexModel>(s => s.Index(Indices.Index("devicedata"))
                    .Query(query => query
                    .Term(md => md.MeasurementDay, startDate))
                    .Explain()
                );

                if (sensorData != null)
                    return Ok(sensorData.Documents);
                else
                    return NotFound();
            }

            var result = await _elasticClient.SearchAsync<ElasticSearchIndexModel>(s => s.Index("devicedata")
            .Query(q => q
                .Bool(bq => bq
                    .Filter(
                        fq => fq.Terms(t => t.Field(f => f.MeasurementDay).Terms(startDate)),
                        fq => fq.Terms(t => t.Field(f => f.SensorType).Terms(sensorType))
                        )
                    )
                )
            );

            if (result != null)
            {
                return Ok(result.Documents);
            }
            else
            {
                return NotFound();
            }

        }
    }
}
