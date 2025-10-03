using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ProductsMicroService.Services;

namespace ProductsMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        //all basic crud operations
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CommonServices.Models.Product product)
        {
            if (product == null) return BadRequest();
            var created = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = created.ProductId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] CommonServices.Models.Product product)
        {
            if (product == null || id != product.ProductId) return BadRequest();
            var updated = await _productService.UpdateProductAsync(id, product);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
    }
}
