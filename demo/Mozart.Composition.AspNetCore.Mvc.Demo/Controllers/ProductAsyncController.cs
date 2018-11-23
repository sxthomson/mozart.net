using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozart.Composition.AspNetCore.Mvc.Actions.Filters;
using Mozart.Composition.AspNetCore.Mvc.Demo.Models;

namespace Mozart.Composition.AspNetCore.Mvc.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MozartComposeModel]
    public class ProductAsyncController : ControllerBase
    {
        // GET: api/<controller>/{id}/IActionResult
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product))]
        [Route("{id}/IActionResult")]
        public async Task<IActionResult> GetProductIActionResult(int id)
        {
            return Ok();
        }

        // GET: api/<controller>/{id}/ActionResult
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product))]
        [Route("{id}/ActionResult")]
        public async Task<ActionResult> GetProductActionResult(int id)
        {
            return Ok();
        }


        // GET: api/<controller>/{id}/ActionResultOfT
        [HttpGet]
        [Route("{id}/ActionResultOfT")]
        public async Task<ActionResult<Product>> GetProductActionResultOfT()
        {
            return Ok();
        }

        // GET: api/<controller>/ObjectResult
        [HttpGet]
        [Route("{id}/ObjectResult")]
        public async Task<Product> GetObjectResult()
        {
            return null;
        }
    }
}