using CommonServices.Models;

namespace OrdersMicroService.Repositories
{
    public interface IOrderItemRepository
    {
        Task<OrderItem> SaveAsync(OrderItem order);
        Task<bool> SaveAsync(IEnumerable<OrderItem> order);
        Task UpdateAsync(OrderItem order);
        Task UpdateAsync(IEnumerable<OrderItem> order);
        Task<OrderItem?> GetAsync(Guid orderId);
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<OrderItem?> GetByIdAsync(Guid orderId);
        Task DeleteAsync(OrderItem order);
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId);
    }
}
