using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mozart.Composition.AspNetCore.Mvc.Demo.Models;
using Mozart.Composition.Core.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductByServiceController : ControllerBase
    {
        private readonly IMozartModelComposer<Product> _productModelComposer;

        public ProductByServiceController(IMozartModelComposer<Product> productModelComposer)
        {
            _productModelComposer = productModelComposer ?? throw new ArgumentNullException(nameof(productModelComposer));
        }

        // GET: api/<controller>/{id}/IActionResult
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(404)]
        [Route("{id}/IActionResult")]
        public async Task<IActionResult> GetProductIActionResult(int id)
        {
            var result = await _productModelComposer.BuildCompositeModelAsync(HttpContext.GetRouteData().Values);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // GET: api/<controller>/{id}/ActionResult
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(404)]
        [Route("{id}/ActionResult")]
        public async Task<ActionResult> GetProductActionResult(int id)
        {
            var result = await _productModelComposer.BuildCompositeModelAsync(HttpContext.GetRouteData().Values);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        // GET: api/<controller>/{id}/ActionResultOfT
        [HttpGet]
        [Route("{id}/ActionResultOfT")]
        public async Task<ActionResult<Product>> GetProductActionResultOfT(int id)
        {
            var result = await _productModelComposer.BuildCompositeModelAsync(HttpContext.GetRouteData().Values);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // GET: api/<controller>/ObjectResult
        [HttpGet]
        [Route("{id}/ObjectResult")]
        public async Task<Product> GetObjectResult(int id)
        {
            var result = await _productModelComposer.BuildCompositeModelAsync(HttpContext.GetRouteData().Values);

            return result;
        }
    }
}