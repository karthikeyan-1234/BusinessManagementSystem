using CommonServices.Models;

namespace OrdersMicroService.Services
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(Order newOrder);
        Task<bool> DeleteOrderAsync(Guid OrderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(Guid OrderId);
        Task UpdateOrderAsync(Order order);
    }
}