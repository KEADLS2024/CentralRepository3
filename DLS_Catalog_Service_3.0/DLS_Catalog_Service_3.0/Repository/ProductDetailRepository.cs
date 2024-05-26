using DLS_Catalog_Service.Model;
using MongoDB.Driver;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DLS_Catalog_Service.Repositories
{
    public class ProductDetailRepository : IProductDetailRepository
    {
        private readonly MongoDbContext _context;
        private readonly IModel _channel;

        public ProductDetailRepository(MongoDbContext context, IConnection connection)
        {
            _context = context;
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "product_details", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        private void PublishProductDetailMessage(ProductDetail detail, string action)
        {
            var messageObject = new { ProductDetailId = detail.Id, Description = detail.Description, Action = action };
            var messageString = JsonSerializer.Serialize(messageObject);
            var body = Encoding.UTF8.GetBytes(messageString);
            _channel.BasicPublish(exchange: "", routingKey: "product_details", basicProperties: null, body: body);
        }

        public async Task<IEnumerable<ProductDetail>> GetAllAsync()
        {
            return await _context.ProductDetails.Find(_ => true).ToListAsync();
        }

        public async Task<ProductDetail> GetByIdAsync(int id)
        {
            return await _context.ProductDetails.Find(detail => detail.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ProductDetail> CreateAsync(ProductDetail detail)
        {
            await _context.ProductDetails.InsertOneAsync(detail);
            PublishProductDetailMessage(detail, "Added");
            return detail;
        }

        public async Task<bool> UpdateAsync(int id, ProductDetail detail)
        {
            var updateResult = await _context.ProductDetails.ReplaceOneAsync(
                g => g.Id == id,
                detail);
            if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
            {
                PublishProductDetailMessage(detail, "Updated");
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var filter = Builders<ProductDetail>.Filter.Eq(m => m.Id, id);
            var deleteResult = await _context.ProductDetails.DeleteOneAsync(filter);
            if (deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0)
            {
                PublishProductDetailMessage(new ProductDetail { Id = id }, "Deleted");
                return true;
            }
            return false;
        }
    }
}
