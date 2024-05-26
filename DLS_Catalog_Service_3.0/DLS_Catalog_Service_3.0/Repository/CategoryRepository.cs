using DLS_Catalog_Service.Model;
using MongoDB.Driver;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DLS_Catalog_Service.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MongoDbContext _context;
        private readonly IModel _channel;

        public CategoryRepository(MongoDbContext context, IConnection connection)
        {
            _context = context;
            _channel = connection.CreateModel(); // Create a channel per instance, or manage lifecycle elsewhere

            // Ensure the queue exists when the repository is instantiated
            _channel.QueueDeclare(queue: "categories",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        private void PublishCategoryMessage(Category category, string action)
        {
            var messageObject = new
            {
                CategoryId = category.Id,
                Name = category.Name,
                Action = action  // "Added", "Updated", "Deleted"
            };

            var messageString = JsonSerializer.Serialize(messageObject);
            var body = Encoding.UTF8.GetBytes(messageString);

            _channel.BasicPublish(exchange: "",
                routingKey: "categories",
                basicProperties: null,
                body: body);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var filter = Builders<Category>.Filter.Eq(c => c.IsDeleted, false);
            return await _context.Categories.Find(filter).ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            var filter = Builders<Category>.Filter.And(
                Builders<Category>.Filter.Eq(c => c.Id, id),
                Builders<Category>.Filter.Eq(c => c.IsDeleted, false)
            );
            return await _context.Categories.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await _context.Categories.InsertOneAsync(category);
            PublishCategoryMessage(category, "Added");
            return category;
        }

        public async Task<bool> UpdateAsync(int id, Category category)
        {
            var updateDefinition = Builders<Category>.Update
                .Set(c => c.Name, category.Name)
                .Set(c => c.CatalogId, category.CatalogId);

            var updateResult = await _context.Categories.UpdateOneAsync(
                g => g.Id == id && !g.IsDeleted,
                updateDefinition);

            if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
            {
                PublishCategoryMessage(category, "Updated");
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Id, id);
            var update = Builders<Category>.Update.Set(c => c.IsDeleted, true);

            var updateResult = await _context.Categories.UpdateOneAsync(filter, update);
            if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
            {
                PublishCategoryMessage(new Category { Id = id, IsDeleted = true }, "Deleted");
                return true;
            }
            return false;
        }
    }
}
