using DLS_Catalog_Service.Model;
using DLS_Catalog_Service.Repositories;
using DLS_Catalog_Service_3_0.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using RabbitMQ.Client;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add configuration to read from environment variables
builder.Configuration.AddEnvironmentVariables();

// Register MongoDB settings
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// Register RabbitMQ settings
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));

// Register MongoDbContext
builder.Services.AddScoped<MongoDbContext>();

// RabbitMQ Configuration with retry mechanism
var rabbitMQSettings = builder.Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

if (rabbitMQSettings == null)
{
    throw new ArgumentNullException(paramName: nameof(RabbitMQSettings), message: "RabbitMQ settings are not configured properly.");
}

Console.WriteLine($"RabbitMQ HostName: {rabbitMQSettings.HostName}");
Console.WriteLine($"RabbitMQ UserName: {rabbitMQSettings.UserName}");
Console.WriteLine($"RabbitMQ Password: {rabbitMQSettings.Password}");

var factory = new ConnectionFactory()
{
    HostName = rabbitMQSettings.HostName,
    UserName = rabbitMQSettings.UserName,
    Password = rabbitMQSettings.Password
};

IConnection connection = null;
var retryAttempts = 5;
for (int i = 0; i < retryAttempts; i++)
{
    try
    {
        connection = factory.CreateConnection();
        break;  // Exit the loop if the connection is successful
    }
    catch (Exception ex)
    {
        Console.WriteLine($"RabbitMQ connection failed. Attempt {i + 1} of {retryAttempts}: {ex.Message}");
        System.Threading.Thread.Sleep(2000);  // Wait for 2 seconds before retrying
    }
}

if (connection == null)
{
    throw new Exception("RabbitMQ connection could not be established after multiple attempts.");
}

builder.Services.AddSingleton<IConnection>(connection);

// Register repositories with RabbitMQ dependency
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductDetailRepository, ProductDetailRepository>();

// Add controllers and other required services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline if in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

SeedDatabase(app.Services);

app.Run();

void SeedDatabase(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

    if (!dbContext.Catalog.AsQueryable().Any())
    {
        dbContext.Catalog.InsertMany(new[]
        {
            new Catalog { Id = 1, Name = "Catalog1/programseed" },
            new Catalog { Id = 2, Name = "Catalog2programseed" },
            new Catalog { Id = 3, Name = "Catalog3programseed" }
        });
    }

    if (!dbContext.Categories.AsQueryable().Any())
    {
        dbContext.Categories.InsertMany(new[]
        {
            new Category { Id = 1, Name = "Category1programseed" },
            new Category { Id = 2, Name = "Category2programseed" },
            new Category { Id = 3, Name = "Categoryprogramseed3" }
        });
    }

    if (!dbContext.Products.AsQueryable().Any())
    {
        dbContext.Products.InsertMany(new[]
        {
            new Product { Id = 1, Name = "Product1programseed", CategoryId = 1 },
            new Product { Id = 2, Name = "Product2", CategoryId = 1 },
            new Product { Id = 3, Name = "Product3", CategoryId = 2 },
            new Product { Id = 4, Name = "Product4", CategoryId = 3 }
        });
    }

    if (!dbContext.ProductDetails.AsQueryable().Any())
    {
        dbContext.ProductDetails.InsertMany(new[]
        {
            new ProductDetail { Id = 1, ProductId = 1, Description = "Detail1programseed" },
            new ProductDetail { Id = 2, ProductId = 2, Description = "Detail2" },
            new ProductDetail { Id = 3, ProductId = 3, Description = "Detail3" },
            new ProductDetail { Id = 4, ProductId = 4, Description = "Detail4" }
        });
    }
}
//using DLS_Catalog_Service.Model;
//using DLS_Catalog_Service.Repositories;
//using DLS_Catalog_Service_3_0.Config;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using MongoDB.Driver;
//using RabbitMQ.Client;
//using System;
//using System.Linq;

//var builder = WebApplication.CreateBuilder(args);

//// Add configuration to read from environment variables
//builder.Configuration.AddEnvironmentVariables();

//// Register MongoDB settings
//builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

//// Register RabbitMQ settings
//builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));

//// Register MongoDbContext
//builder.Services.AddScoped<MongoDbContext>();

//// RabbitMQ Configuration with retry mechanism
//var rabbitMQSettings = builder.Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

//if (rabbitMQSettings == null)
//{
//    throw new ArgumentNullException(paramName: nameof(RabbitMQSettings), message: "RabbitMQ settings are not configured properly.");
//}

//Console.WriteLine($"RabbitMQ HostName: {rabbitMQSettings.HostName}");
//Console.WriteLine($"RabbitMQ UserName: {rabbitMQSettings.UserName}");
//Console.WriteLine($"RabbitMQ Password: {rabbitMQSettings.Password}");

//var factory = new ConnectionFactory()
//{
//    HostName = rabbitMQSettings.HostName,
//    UserName = rabbitMQSettings.UserName,
//    Password = rabbitMQSettings.Password
//};

//IConnection connection = null;
//var retryAttempts = 5;
//for (int i = 0; i < retryAttempts; i++)
//{
//    try
//    {
//        connection = factory.CreateConnection();
//        break;  // Exit the loop if the connection is successful
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"RabbitMQ connection failed. Attempt {i + 1} of {retryAttempts}: {ex.Message}");
//        System.Threading.Thread.Sleep(2000);  // Wait for 2 seconds before retrying
//    }
//}

//if (connection == null)
//{
//    throw new Exception("RabbitMQ connection could not be established after multiple attempts.");
//}

//builder.Services.AddSingleton<IConnection>(connection);

//// Register repositories with RabbitMQ dependency
//builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<IProductDetailRepository, ProductDetailRepository>();

//// Add controllers and other required services
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline if in development
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

//    // Seed the database in development environment
//    SeedDatabase(app.Services);
//}

//app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//app.UseHttpsRedirection();
//app.UseRouting();
//app.UseAuthorization();
//app.UseEndpoints(endpoints => endpoints.MapControllers());

//app.Run();

//void SeedDatabase(IServiceProvider services)
//{
//    using var scope = services.CreateScope();
//    var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

//    // Drop existing collections
//    dbContext.Catalog.Database.DropCollection("Catalog");
//    dbContext.Categories.Database.DropCollection("Category");
//    dbContext.Products.Database.DropCollection("Product");
//    dbContext.ProductDetails.Database.DropCollection("ProductDetail");

//    // Re-create collections with seed data
//    dbContext.Catalog.InsertMany(new[]
//    {
//        new Catalog { Id = 1, Name = "Catalog1-__PROGRAM" },
//        new Catalog { Id = 2, Name = "Catalog2" },
//        new Catalog { Id = 3, Name = "Catalog3" }
//    });

//    dbContext.Categories.InsertMany(new[]
//    {
//        new Category { Id = 1, Name = "Category1" },
//        new Category { Id = 2, Name = "Category2" },
//        new Category { Id = 3, Name = "Category3" }
//    });

//    dbContext.Products.InsertMany(new[]
//    {
//        new Product { Id = 1, Name = "Product1", CategoryId = 1 },
//        new Product { Id = 2, Name = "Product2", CategoryId = 1 },
//        new Product { Id = 3, Name = "Product3", CategoryId = 2 },
//        new Product { Id = 4, Name = "Product4", CategoryId = 3 }
//    });

//    dbContext.ProductDetails.InsertMany(new[]
//    {
//        new ProductDetail { Id = 1, ProductId = 1, Description = "Detail1" },
//        new ProductDetail { Id = 2, ProductId = 2, Description = "Detail2" },
//        new ProductDetail { Id = 3, ProductId = 3, Description = "Detail3" },
//        new ProductDetail { Id = 4, ProductId = 4, Description = "Detail4" }
//    });
//}

