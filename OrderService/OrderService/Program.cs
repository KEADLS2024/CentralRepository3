using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Interfaces;
using OrderService.Managers;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// RabbitMQ Configuration
var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");
var factory = new ConnectionFactory()
{
    HostName = rabbitMQConfig["HostName"],
    UserName = rabbitMQConfig["UserName"],
    Password = rabbitMQConfig["Password"]
};

var connection = factory.CreateConnection();
builder.Services.AddSingleton<IConnection>(connection);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderTables, OrderTablesManager>(provider =>
    new OrderTablesManager(
        provider.GetRequiredService<MyDbContext>(),
        provider.GetRequiredService<IConnection>()
    )
);

builder.Services.AddScoped<IOrderItems, OrderItemsManager>(provider =>
    new OrderItemsManager(
        provider.GetRequiredService<MyDbContext>(),
        provider.GetRequiredService<IConnection>()
    )
);

builder.Services.AddScoped<OrderTablesManager>();
builder.Services.AddScoped<OrderItemsManager>();

builder.Services.AddScoped<IOrderTables, OrderTablesManager>();
builder.Services.AddScoped<IOrderItems, OrderItemsManager>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
