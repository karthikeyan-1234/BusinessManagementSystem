using CommonServices.Models;

namespace InventoryMicroService.Services
{
    public interface IInventoryService
    {
        Task<InventoryItem> AddInventory(InventoryItem inventory);
        Task<IEnumerable<InventoryItem>> GetAllInventory();
        Task<bool> DeleteInventory(InventoryItem inventory);
    }
}