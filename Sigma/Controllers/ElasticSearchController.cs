using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sigma.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ElasticSearchController : ControllerBase
    {
        private readonly ILogger<ElasticSearchController> _logger;

        public ElasticSearchController(ILogger<ElasticSearchController> logger)
        {
            _logger = logger;
        }

        // GET: /<controller>/
        public async Task<ActionResult> Index()
        {
            return Ok();
        }
    }
}
