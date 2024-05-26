using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Interfaces;
using OrderService.Models;
using RabbitMQ.Client;

namespace OrderService.Managers;

public class OrderTablesManager : IOrderTables
{
    private readonly MyDbContext _context;
    private readonly IModel _channel;

    public OrderTablesManager(MyDbContext context, IConnection connection)
    {
        _context = context;
        _channel = connection.CreateModel(); // Create a channel per instance, or manage lifecycle elsewhere

        // Ensure the queue exists when the manager is instantiated
        _channel.QueueDeclare(queue: "orders",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    private void PublishOrderMessage(OrderTable order, string action)
    {
        var messageObject = new
        {
            OrderId = order.OrderId,
            TotalAmount = order.TotalAmount,
            CustomerId = order.CustomerId,
            Action = action  // "Added" or "Updated"
        };

        var messageString = JsonSerializer.Serialize(messageObject);
        var body = Encoding.UTF8.GetBytes(messageString);

        _channel.BasicPublish(exchange: "",
            routingKey: "orders",
            basicProperties: null,
            body: body);
    }

    public async Task<IEnumerable<OrderTable>> GetAllOrdersAsync()
    {
        return await _context.OrderTables.Where(o => o.DeletedAt == null).ToListAsync();
    }

    public async Task<IEnumerable<OrderTable>> GetAllDeletedOrdersAsync()
    {
        return await _context.OrderTables.ToListAsync();
    }

    public async Task<OrderTable> GetOrderByIdAsync(int orderId)
    {
        return await _context.OrderTables.FirstOrDefaultAsync(o => o.OrderId == orderId && o.DeletedAt == null);
    }

    public async Task AddOrderAsync(OrderTable order)
    {
        _context.OrderTables.Add(order);
        await _context.SaveChangesAsync();
        PublishOrderMessage(order, "Added");
    }

    public async Task UpdateOrderAsync(OrderTable order)
    {
        if (order.DeletedAt == null)
        {
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            PublishOrderMessage(order, "Updated");
        }
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        var order = await GetOrderByIdAsync(orderId);
        if (order != null)
        {
            order.DeletedAt = DateTime.Now;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            PublishOrderMessage(order, "Deleted");
        }
    }

    public async Task<IEnumerable<OrderTable>> GetOrdersByCustomerAndDateAsync(int customerId, DateTime start, DateTime end)
    {
        return await _context.OrderTables
            .Where(o => o.CustomerId == customerId && o.OrderDate >= start && o.OrderDate <= end && o.DeletedAt == null)
            .ToListAsync();
    }

    public async Task DeleteOrderAsync(OrderTable order)
    {
        if (order != null)
        {
            order.DeletedAt = DateTime.Now;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            PublishOrderMessage(order, "Deleted");
        }
    }
}