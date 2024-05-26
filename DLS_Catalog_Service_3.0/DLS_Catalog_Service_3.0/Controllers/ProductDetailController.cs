using DLS_Catalog_Service.Model;
using DLS_Catalog_Service.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DLS_Catalog_Service.Controllers
{
    [Route("api/productdetails")]
    [ApiController]
    public class ProductDetailsController : ControllerBase
    {
        private readonly IProductDetailRepository _productDetailRepository;

        public ProductDetailsController(IProductDetailRepository productDetailRepository)
        {
            _productDetailRepository = productDetailRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductDetails()
        {
            var productDetails = await _productDetailRepository.GetAllAsync();
            return Ok(productDetails);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDetailById(string id)
        {
            if (!int.TryParse(id, out int productDetailId))
            {
                return BadRequest("Invalid product detail id format.");
            }

            var productDetail = await _productDetailRepository.GetByIdAsync(productDetailId);
            if (productDetail == null) return NotFound();
            return Ok(productDetail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductDetail([FromBody] ProductDetail productDetail)
        {
            var createdProductDetail = await _productDetailRepository.CreateAsync(productDetail);
            return CreatedAtAction(nameof(GetProductDetailById), new { id = createdProductDetail.Id },
                createdProductDetail);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductDetail(string id, [FromBody] ProductDetail productDetail)
        {
            if (!int.TryParse(id, out int productDetailId))
            {
                return BadRequest("Invalid product detail id format.");
            }

            if (productDetailId != productDetail.Id)
            {
                return BadRequest("Product detail id mismatch.");
            }

            var updated = await _productDetailRepository.UpdateAsync(productDetailId, productDetail);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductDetail(string id)
        {
            if (!int.TryParse(id, out int productDetailId))
            {
                return BadRequest("Invalid product detail id format.");
            }

            var deleted = await _productDetailRepository.DeleteAsync(productDetailId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
