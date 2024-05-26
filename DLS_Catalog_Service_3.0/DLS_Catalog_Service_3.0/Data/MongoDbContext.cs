using DLS_Catalog_Service.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);

        // Ensure collections are created
        var collectionNames = _database.ListCollectionNames().ToList();
        if (!collectionNames.Contains("Catalog"))
        {
            _database.CreateCollection("Catalog");
        }
        if (!collectionNames.Contains("Category"))
        {
            _database.CreateCollection("Category");
        }
        if (!collectionNames.Contains("Product"))
        {
            _database.CreateCollection("Product");
        }
        if (!collectionNames.Contains("ProductDetail"))
        {
            _database.CreateCollection("ProductDetail");
        }
    }

    public IMongoCollection<Catalog> Catalog => _database.GetCollection<Catalog>("Catalog");
    public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Category");
    public IMongoCollection<Product> Products => _database.GetCollection<Product>("Product");
    public IMongoCollection<ProductDetail> ProductDetails => _database.GetCollection<ProductDetail>("ProductDetail");
}