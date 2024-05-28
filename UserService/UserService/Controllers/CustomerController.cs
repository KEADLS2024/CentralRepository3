using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Models;
using UserService.Managers;

namespace UserService.Controllers {
   [Route("api/[controller]")]
   [ApiController]
   public class CustomerController : ControllerBase {
      private readonly CustomerManager _customersManager;

      public CustomerController(CustomerManager customersManager) {
         _customersManager = customersManager;
      }

      // GET: api/Customers
      [HttpGet]
      public async Task<ActionResult<IEnumerable<Customer>>> Get() {
         var customers = await _customersManager.GetAll();
         return Ok(customers);
      }

      // GET: api/Customers/5
      [HttpGet("{id}")]
      public async Task<ActionResult<Customer>> GetbyID(int id) {
         var customer = await _customersManager.Get(id);
         if (customer == null) {
            return NotFound();
         }

         return customer;
      }

      // PUT: api/Customers/5
      [HttpPut("{id}")]
      public async Task<IActionResult> Put(int id, [FromBody] Customer customer) {
         if (id != customer.CustomerId) {
            return BadRequest();
         }

         try {
            bool result = await _customersManager.UpdateCustomerAsync(customer);
            if (!result) {
               return StatusCode(500, "An error occurred while updating the customer.");
            }
         }
         catch (KeyNotFoundException) {
            return NotFound();
         }
         catch (Exception ex) {
            Console.WriteLine("Error in PUT request: " + ex.Message); // Log the exception
            return StatusCode(500, "An error occurred: " + ex.Message);
         }

         return NoContent();
      }



      // POST: api/Customers
      [HttpPost]
      public async Task<ActionResult<Customer>> Post([FromBody] Customer customer) {
         try {
            var createdCustomer = await _customersManager.CreateCustomerAsync(customer);
            return CreatedAtAction(nameof(GetbyID), new { id = createdCustomer.CustomerId }, createdCustomer);
         }
         catch (Exception ex) {
            Console.WriteLine("Error in POST request: " + ex.Message); // Log the exception
            return StatusCode(500, "An error occurred: " + ex.Message);
         }
      }


      // DELETE: api/Customers/5
      [HttpDelete("{id}")]
      public async Task<IActionResult> Delete(int id) {
         try {
            await _customersManager.DeleteCustomerAsync(id);
         }
         catch (KeyNotFoundException) {
            return NotFound();
         }

         return NoContent();
      }

      // GET: api/Customers/search?query=xyz
      [HttpGet("search")]
      public async Task<ActionResult<IEnumerable<Customer>>> Search([FromQuery] string query) {
         var customers = await _customersManager.SearchCustomersAsync(query);
         return Ok(customers);
      }
   }
}



