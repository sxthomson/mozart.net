using Microsoft.AspNetCore.Mvc;
using Mozart.Composition.AspNetCore.Mvc.Demo.Models;

namespace Mozart.Composition.AspNetCore.Mvc.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // GET: api/<controller>/{id}/IActionResult
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product))]
        [Route("{id}/IActionResult")]
        public IActionResult GetProductIActionResult(int id)
        {
            return Ok();
        }

        // GET: api/<controller>/{id}/ActionResult
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product))]
        [Route("{id}/ActionResult")]
        public ActionResult GetProductActionResult(int id)
        {
            return Ok();
        }

        // GET: api/<controller>/{id}/ActionResultOfT
        [HttpGet]
        [Route("{id}/ActionResultOfT")]
        public ActionResult<Product> GetProductActionResultOfT(int id)
        {
            return Ok();
        }

        // GET: api/<controller>/ObjectResult
        [HttpGet]
        [Route("{id}/ObjectResult")]
        public Product GetObjectResult(int id)
        {
            return null;
        }
    }
}