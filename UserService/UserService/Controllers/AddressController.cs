using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Managers;
using UserService.Models;

namespace UserService.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : Controller
    {
        private readonly AddressManager _addressesManager;

        public AddressController(AddressManager addressesManager)
        {
            _addressesManager = addressesManager;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            var addresses = await _addressesManager.GetAllAddressesAsync();

            if (addresses == null)
            {
                return NotFound("Address list is empty.");
            }

            return Ok(addresses);
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _addressesManager.GetAddressByIdAsync(id);

            if (address == null)
            {
                return NotFound($"Address with ID {id} not found.");
            }

            return Ok(address);
        }

        // PUT: api/Addresses/5
        [HttpPut("{id}")]
    
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.AddressId)
            {
                return BadRequest("Address ID mismatch.");
            }

            try
            {
                await _addressesManager.UpdateAddressAsync(address);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Address with ID {id} not found.");
            }

            return NoContent();
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            var createdAddress = await _addressesManager.CreateAddressAsync(address);

            return CreatedAtAction(nameof(GetAddress), new { id = createdAddress.AddressId }, createdAddress);
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var deleted = await _addressesManager.DeleteAddressAsync(id);

            if (!deleted)
            {
                return base.NotFound($"Address with ID {id} not found.");
            }

            return NoContent();
        }
    }
}
