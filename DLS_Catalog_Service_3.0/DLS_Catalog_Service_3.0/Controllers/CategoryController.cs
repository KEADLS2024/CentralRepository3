using DLS_Catalog_Service.Model;
using DLS_Catalog_Service.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DLS_Catalog_Service.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            if (!int.TryParse(id, out int categoryId))
            {
                return BadRequest("Invalid category id format.");
            }

            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            var createdCategory = await _categoryRepository.CreateAsync(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] Category category)
        {
            if (!int.TryParse(id, out int categoryId))
            {
                return BadRequest("Invalid category id format.");
            }

            if (categoryId != category.Id)
            {
                return BadRequest("Category id mismatch.");
            }

            var updated = await _categoryRepository.UpdateAsync(categoryId, category);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            if (!int.TryParse(id, out int categoryId))
            {
                return BadRequest("Invalid category id format.");
            }

            var deleted = await _categoryRepository.DeleteAsync(categoryId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
