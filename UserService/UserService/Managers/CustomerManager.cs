using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Data;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Managers {
   public class CustomerManager : ICustomerManager {
      private readonly MyDbContext _dbContext;
      private readonly ElasticClient _elasticClient;

      public CustomerManager(MyDbContext myDbContext) {
         _dbContext = myDbContext;
         _elasticClient = ElasticsearchConfig.CreateClient();

         // Ensure the index exists when the manager is initialized
         CreateIndexIfNotExists();
      }

      public async Task<IEnumerable<Customer>> GetAll() {
         return await _dbContext.Customers.ToListAsync();
      }

      public async Task<Customer> Get(int id) {
         return await _dbContext.Customers.FindAsync(id);
      }

      public async Task<bool> UpdateCustomerAsync(Customer customer) {
         try {
            await IndexCustomerAsync(customer);
            _dbContext.Entry(customer).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
         }
         catch (Exception ex) {
            Console.WriteLine("Error updating customer: " + ex.Message);
            throw;
         }
      }

      public async Task<Customer> CreateCustomerAsync(Customer customer) {
         try {
            var existingCustomer = await _dbContext.Customers
                .AnyAsync(c => c.Email == customer.Email);

            if (existingCustomer) {
               throw new Exception("Customer with the given email already exists.");
            }

            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();

            await IndexCustomerAsync(customer);

            return customer;
         }
         catch (Exception ex) {
            Console.WriteLine("Error creating customer: " + ex.Message);
            throw;
         }
      }

      public async Task<bool> DeleteCustomerAsync(Customer customer) {
         _dbContext.Customers.Remove(customer);
         await _dbContext.SaveChangesAsync();
         await DeleteCustomerFromIndexAsync(customer.CustomerId);
         return true;
      }

      public async Task DeleteCustomerAsync(int customerId) {
         var customer = await _dbContext.Customers.FindAsync(customerId);
         if (customer != null) {
            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();
            await DeleteCustomerFromIndexAsync(customerId);
         }
      }

      public async Task<bool> Exists(int customerId) {
         return await _dbContext.Customers.AnyAsync(c => c.CustomerId == customerId);
      }

      public async Task IndexCustomerAsync(Customer customer) {
         var indexResponse = await _elasticClient.IndexDocumentAsync(customer);
         if (!indexResponse.IsValid) {
            Console.WriteLine("Failed to index customer: " + indexResponse.OriginalException.Message);
         }
      }

      public async Task DeleteCustomerFromIndexAsync(int customerId) {
         var deleteResponse = await _elasticClient.DeleteAsync<Customer>(customerId);
         if (!deleteResponse.IsValid) {
            Console.WriteLine("Failed to delete customer from index: " + deleteResponse.OriginalException.Message);
         }
      }

      public async Task<List<Customer>> SearchCustomersAsync(string query) {
         var searchResponse = await _elasticClient.SearchAsync<Customer>(s => s
             .Query(q => q
                 .MultiMatch(m => m
                     .Fields(f => f
                         .Field(ff => ff.FirstName)
                         .Field(ff => ff.LastName)
                         .Field(ff => ff.Email)
                         .Field(ff => ff.Phone)
                     )
                     .Query(query)
                 )
             )
         );

         if (!searchResponse.IsValid) {
            Console.WriteLine("Failed to search customers: " + searchResponse.OriginalException.Message);
            return new List<Customer>();
         }

         return searchResponse.Documents.ToList();
      }

      private void CreateIndexIfNotExists() {
         var existsResponse = _elasticClient.Indices.Exists("customers");
         if (!existsResponse.Exists) {
            var createIndexResponse = _elasticClient.Indices.Create("customers", c => c
                .Map<Customer>(m => m
                    .AutoMap()
                )
            );

            if (!createIndexResponse.IsValid) {
               Console.WriteLine("Failed to create index: " + createIndexResponse.OriginalException.Message);
            }
         }
      }
   }
}


