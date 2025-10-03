using CommonServices.Models;

namespace PaymentsMicroService.Repositories
{
    public interface IInventoryRepository
    {
        Task<InventoryItem> AddAsync(InventoryItem item);
        Task<InventoryItem> SaveAsync(InventoryItem item);
        Task UpdateAsync(InventoryItem item);
        Task<InventoryItem?> GetAsync(Guid id);
        Task<InventoryItem?> GetByProductIdAsync(Guid Productid);
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<bool> DeleteAsync(Guid productId);

    }
}
