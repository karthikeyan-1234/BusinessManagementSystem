using CommonServices.Models;

namespace OrdersMicroService.Services
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(Order newOrder);
        public Task ListenForEvents(CancellationToken cancellationToken);
        public Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> DeleteOrderAsync(Guid OrderId);
        Task<Order?> GetOrderByIdAsync(Guid id);
    }
}