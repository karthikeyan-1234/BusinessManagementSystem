using CommonServices.Models;

namespace PurchaseMicroService.Services
{
    public interface IPurchaseService
    {
        Task<Purchase> CreatePurchaseAsync(CreatePurchase newPurchase);
        Task<PurchaseItem> CreatePurchaseItemAsync(PurchaseItem newPurchaseItem);
        Task<bool> DeletePurchaseAsync(Guid id);
        Task<bool> DeletePurchaseItemAsync(Guid id);
        Task<IEnumerable<Purchase>> GetAllPurchasesAsync();
        Task<Purchase?> GetPurchaseByIdAsync(Guid id);
        Task<IEnumerable<PurchaseItem>> GetPurchaseItemsByPurchase(Guid purchaseId);
        Task<Purchase?> UpdatePurchaseAsync(Guid id, Purchase Purchase);
        Task<PurchaseItem?> UpdatePurchaseItemAsync(Guid id, PurchaseItem PurchaseItem);
    }
}