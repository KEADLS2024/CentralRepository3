using Microsoft.EntityFrameworkCore;
using System.Net;
using UserService.Data;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Managers
{
    public class CustomerManager : ICustomerManager 
    {
        private readonly MyDbContext _dbContext;

        public CustomerManager(MyDbContext myDbContext)
        {
            _dbContext = myDbContext;
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<Customer> Get(int id)
        {
            return await _dbContext.Customers.FindAsync(id);
        }

        public async Task<bool> UpdateCustomerAsync( Customer customer)
        {
            _dbContext.Entry(customer).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        //public async Task<Customer> CreateCustomerAsync(Customer customer)               
        //{
        //    _dbContext.Customers.Add(customer);
        //    await _dbContext.SaveChangesAsync();
        //    return customer;
        //}
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            // Check if the customer already exists
            var existingCustomer = await _dbContext.Customers
                .AnyAsync(c => c.Email == customer.Email);

            if (existingCustomer)
            {
                // Handle the case, e.g., throw an exception or return null
                throw new Exception("Customer with the given email already exists.");
            }

            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();
            return customer;
        }


        public async Task<bool> DeleteCustomerAsync(Customer customer)
        {
            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await _dbContext.Customers.FindAsync(customerId);
            if (customer != null)
            {
                _dbContext.Customers.Remove(customer);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int customerId)
        {
            return await _dbContext.Customers.AnyAsync(c => c.CustomerId == customerId);
        }
    }
}
