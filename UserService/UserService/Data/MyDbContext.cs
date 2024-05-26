using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace UserService.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Customer> Customers { get; set; }


    
    }
}
