using DLS_Catalog_Service.Model;
using MongoDB.Driver;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DLS_Catalog_Service.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly MongoDbContext _context;
        private readonly IModel _channel;

        public CatalogRepository(MongoDbContext context, IConnection connection)
        {
            _context = context;
            _channel = connection.CreateModel(); // Create a channel per instance, or manage lifecycle elsewhere

            // Ensure the queue exists when the repository is instantiated
            _channel.QueueDeclare(queue: "catalogs",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        private void PublishOrderMessage(Catalog catalog, string action)
        {
            var messageObject = new
            {
                CatalogId = catalog.Id,
                Name = catalog.Name,
                Year = catalog.Year,
                Action = action  // "Added" or "Updated"
            };

            var messageString = JsonSerializer.Serialize(messageObject);
            var body = Encoding.UTF8.GetBytes(messageString);

            _channel.BasicPublish(exchange: "",
                routingKey: "catalogs",
                basicProperties: null,
                body: body);
        }

        public async Task<IEnumerable<Catalog>> GetAllCatalogsAsync()
        {
            return await _context.Catalog.Find(_ => true).ToListAsync();
        }

        public async Task<Catalog> GetCatalogByIdAsync(int id)
        {
            return await _context.Catalog.Find(catalog => catalog.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Catalog> CreateCatalogAsync(Catalog catalog)
        {
            await _context.Catalog.InsertOneAsync(catalog);
            PublishOrderMessage(catalog, "Added");  // Publish message when a catalog is created
            return catalog;
        }

        public async Task<bool> UpdateCatalogAsync(int id, Catalog catalog)
        {
            ReplaceOneResult updateResult = await _context.Catalog.ReplaceOneAsync(
                filter: g => g.Id == id,
                replacement: catalog);
            if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
            {
                PublishOrderMessage(catalog, "Updated");  // Publish message when a catalog is updated
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteCatalogAsync(int id)
        {
            DeleteResult deleteResult = await _context.Catalog.DeleteOneAsync(catalog => catalog.Id == id);
            if (deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0)
            {
                PublishOrderMessage(new Catalog { Id = id }, "Deleted");  // Publish message when a catalog is deleted
                return true;
            }
            return false;
        }
    }
}
