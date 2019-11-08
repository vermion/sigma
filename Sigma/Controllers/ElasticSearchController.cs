using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using Sigma.Backgroundservices;

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

        // GET: /<controller>/
        public async Task<ActionResult> Index()
        {
            //await _elasticClient.UpdateAsync<SensorDataModels.SensorMetaData>(post, u => u.Doc(post));

            await _elasticClient.IndexDocumentAsync(_retrieveSensorDataClient._metaSensorData[0]);

            return Ok();
        }
    }
}
