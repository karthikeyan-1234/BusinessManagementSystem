using CommonServices.Models;

namespace OrdersMicroService.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> SaveAsync(Order order);
        Task UpdateAsync(Order order);
        Task<Order?> GetAsync(Guid orderId);
        Task<IEnumerable<Order>> GetAllAsync();
    }
}
