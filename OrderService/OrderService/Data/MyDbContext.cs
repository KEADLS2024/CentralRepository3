using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        public DbSet<OrderTable> OrderTables { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
