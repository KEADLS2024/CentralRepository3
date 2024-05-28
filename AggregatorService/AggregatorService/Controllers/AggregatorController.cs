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

        [HttpGet("order")]
        public async Task<IActionResult> GetOrderData()
        {
            var client = _httpClientFactory.CreateClient();

            // Call OrderService
            var ordersResponse = await client.GetStringAsync("http://order-app/api/OrderTables");

            // Aggregate the data
            var aggregatedData = new
            {
                Orders = ordersResponse
            };

            return Ok(aggregatedData);
        }
    }
}