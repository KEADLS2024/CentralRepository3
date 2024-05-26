using DLS_Catalog_Service.Model;
using DLS_Catalog_Service.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DLS_Catalog_Service.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            if (!int.TryParse(id, out int productId))
            {
                return BadRequest("Invalid product id format.");
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var createdProduct = await _productRepository.CreateAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product product)
        {
            if (!int.TryParse(id, out int productId))
            {
                return BadRequest("Invalid product id format.");
            }

            if (productId != product.Id)
            {
                return BadRequest("Product id mismatch.");
            }

            var updated = await _productRepository.UpdateAsync(productId, product);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (!int.TryParse(id, out int productId))
            {
                return BadRequest("Invalid product id format.");
            }

            var deleted = await _productRepository.DeleteAsync(productId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
