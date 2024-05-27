using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace AggregatorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggregatorController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AggregatorController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("user-order-catalog")]
        public async Task<IActionResult> GetUserOrderCatalogData()
        {
            var client = _httpClientFactory.CreateClient();

            // Call UserService
            var addressesResponse = await client.GetStringAsync("http://user-app:8060/api/Addresses");
            // Call OrderService
            var ordersResponse = await client.GetStringAsync("http://order-app:8080/api/OrderTables");
            // Call CatalogService
            var categoriesResponse = await client.GetStringAsync("http://catalog-app:8070/api/Categories");

            // Aggregate the data
            var aggregatedData = new
            {
                Addresses = addressesResponse,
                Orders = ordersResponse,
                Categories = categoriesResponse
            };

            return Ok(aggregatedData);
        }
    }
}