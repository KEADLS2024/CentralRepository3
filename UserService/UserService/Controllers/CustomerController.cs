using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Managers;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerManager _customersManager;

        public CustomerController(CustomerManager customersManager)
        {
            _customersManager = customersManager;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> Get()
        {
            var customers = await _customersManager.GetAll();
            return Ok(customers);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetbyID(int id)
        {
            var customer = await _customersManager.Get(id);
            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Customer customer)
        {

            try
            {
                await _customersManager.UpdateCustomerAsync( customer);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> Post([FromBody] Customer customer)
        {
            var createdCustomer = await _customersManager.CreateCustomerAsync(customer);

            return CreatedAtAction(nameof(Get), new { id = createdCustomer.CustomerId }, createdCustomer);
        }



        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _customersManager.DeleteCustomerAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }


    }
}
