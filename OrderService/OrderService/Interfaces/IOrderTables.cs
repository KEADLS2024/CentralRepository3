using OrderService.Models;

namespace OrderService.Interfaces
{
    public interface IOrderTables
    {
        Task<IEnumerable<OrderTable>> GetAllOrdersAsync();
        Task<OrderTable> GetOrderByIdAsync(int orderId);
        Task AddOrderAsync(OrderTable order);
        Task UpdateOrderAsync(OrderTable order);
        Task DeleteOrderAsync(int orderId);
        Task<IEnumerable<OrderTable>> GetOrdersByCustomerAndDateAsync(int customerId, DateTime start, DateTime end);
        Task DeleteOrderAsync(OrderTable order);
    }
}
