using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Interfaces
{
    public interface IAddressManager
    {
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task<Address> GetAddressByIdAsync(int addressId);
        Task<Address> CreateAddressAsync(Address address);
        Task<bool> UpdateAddressAsync(Address address);
        Task<bool> DeleteAddressAsync(int id);
        Task<bool> AddressExistsAsync(int addressId);
    }
}
