using UserService.Models;

namespace UserService.Interfaces
{
    public interface ICustomerManager
    {
        Task<IEnumerable<Customer>> GetAll();
        Task<Customer> Get(int customerId);
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int customerId);
        Task<bool> Exists(int customerId);
    }
}
