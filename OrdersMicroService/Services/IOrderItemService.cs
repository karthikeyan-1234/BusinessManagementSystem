using CommonServices.Models;

namespace OrdersMicroService.Services
{
    public interface IOrderItemService
    {
        Task<string> CreateOrderItemAsync(OrderItem newOrder);
        Task<IEnumerable<OrderItem>> CreateMultipleOrderItemsAsync(IEnumerable<OrderItem> newOrders);
        public Task ListenForEvents(CancellationToken cancellationToken);
        public Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync();
        Task<bool> DeleteOrderItemAsync(Guid OrderItemId);
        Task<OrderItem?> GetOrderItemByIdAsync(Guid OrderItemId);
    }
}