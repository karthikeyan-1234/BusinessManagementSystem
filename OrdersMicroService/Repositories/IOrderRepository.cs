using CommonServices.Models;

namespace OrdersMicroService.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> SaveAsync(Order order);
        Task UpdateAsync(Order order);
        Task<Order?> GetByIdAsync(Guid orderId);
        Task<IEnumerable<Order>> GetAllAsync();
        IQueryable<Order> GetAll();
        Task<int> DeleteAsync(Guid orderId);
    }
}
