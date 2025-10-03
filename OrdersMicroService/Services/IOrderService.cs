using CommonServices.Models;

namespace OrdersMicroService.Services
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(Order newOrder);
        public Task ListenForEvents(CancellationToken cancellationToken);
        public Task<IEnumerable<Order>> GetAllOrdersAsync();
    }
}