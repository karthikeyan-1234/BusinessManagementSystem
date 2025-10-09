using CommonServices.Models;

namespace OrdersMicroService.Repositories
{
    public interface IOrderItemRepository
    {
        Task<OrderItem> SaveAsync(OrderItem order);
        Task UpdateAsync(OrderItem order);
        Task<OrderItem?> GetAsync(Guid orderId);
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<OrderItem?> GetByIdAsync(Guid orderId);
        Task DeleteAsync(OrderItem order);
    }
}
