using DLS_Catalog_Service.Model;
using DLS_Catalog_Service.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DLS_Catalog_Service.Controllers
{
    [Route("api/catalogs")]
    [ApiController]
    public class CatalogsController : ControllerBase
    {
        private readonly ICatalogRepository _catalogRepository;

        public CatalogsController(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCatalogs()
        {
            var catalogs = await _catalogRepository.GetAllCatalogsAsync();
            return Ok(catalogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatalogById(string id)
        {
            if (!int.TryParse(id, out int catalogId))
            {
                return BadRequest("Invalid catalog id format.");
            }

            var catalog = await _catalogRepository.GetCatalogByIdAsync(catalogId);
            if (catalog == null) return NotFound();
            return Ok(catalog);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCatalog([FromBody] Catalog catalog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the catalog with the given Id already exists
            var existingCatalog = await _catalogRepository.GetCatalogByIdAsync(catalog.Id);
            if (existingCatalog != null)
            {
                return BadRequest("A catalog with the same ID already exists.");
            }

            var createdCatalog = await _catalogRepository.CreateCatalogAsync(catalog);
            if (createdCatalog == null)
            {
                return BadRequest("Unable to create the catalog.");
            }
            return CreatedAtAction(nameof(GetCatalogById), new { id = createdCatalog.Id }, createdCatalog);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCatalog(string id, [FromBody] Catalog catalog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!int.TryParse(id, out int catalogId))
            {
                return BadRequest("Invalid catalog id format.");
            }

            if (catalogId != catalog.Id)
            {
                return BadRequest("ID mismatch between the URL and the body.");
            }

            var updated = await _catalogRepository.UpdateCatalogAsync(catalogId, catalog);
            if (!updated)
            {
                return NotFound($"Catalog with ID {id} not found.");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalog(string id)
        {
            if (!int.TryParse(id, out int catalogId))
            {
                return BadRequest("Invalid catalog id format.");
            }

            var deleted = await _catalogRepository.DeleteCatalogAsync(catalogId);
            if (!deleted)
            {
                return NotFound($"Catalog with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
