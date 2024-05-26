using DLS_Catalog_Service.Model;
using MongoDB.Driver;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DLS_Catalog_Service.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MongoDbContext _context;
        private readonly IModel _channel;

        public ProductRepository(MongoDbContext context, IConnection connection)
        {
            _context = context;
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "products", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        private void PublishProductMessage(Product product, string action)
        {
            var messageObject = new { ProductId = product.Id, Name = product.Name, Action = action };
            var messageString = JsonSerializer.Serialize(messageObject);
            var body = Encoding.UTF8.GetBytes(messageString);
            _channel.BasicPublish(exchange: "", routingKey: "products", basicProperties: null, body: body);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Find(_ => true).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.Find(product => product.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _context.Products.InsertOneAsync(product);
            PublishProductMessage(product, "Added");
            return product;
        }

        public async Task<bool> UpdateAsync(int id, Product product)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var updateResult = await _context.Products.ReplaceOneAsync(filter, product);
            if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
            {
                PublishProductMessage(product, "Updated");
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var deleteResult = await _context.Products.DeleteOneAsync(filter);
            if (deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0)
            {
                PublishProductMessage(new Product { Id = id }, "Deleted");
                return true;
            }
            return false;
        }
    }
}
