
using CommonServices.Models;
using CommonServices.Repositories;

using Confluent.Kafka;

namespace PurchaseMicroService.Services
{
    public class PurchaseService : IPurchaseService
    {
        IGenericRepository<Purchase> purchaseRepo;
        IGenericRepository<PurchaseItem> purchaseItemRepo;
        IProducer<string, string> _producer;

        public PurchaseService(IGenericRepository<Purchase> purchaseRepo, IGenericRepository<PurchaseItem> purchaseItemRepo, IProducer<string, string> producer)
        {
            this.purchaseRepo = purchaseRepo;
            this.purchaseItemRepo = purchaseItemRepo;
            _producer = producer;
        }

        //Add new purchase
        public async Task<Purchase> CreatePurchaseAsync(CreatePurchase newPurchase)
        {
            var purchase = new Purchase()
            {
                id = Guid.NewGuid(),
                purchaseDate = DateTime.Now,
                PaymentTransactionId = null,
                state = OrderStatus.Pending,
            };

            await purchaseRepo.AddAsync(purchase);
            await purchaseRepo.SaveAsync();

            return purchase;
        }

        //Add new purchase item
        public async Task<PurchaseItem> CreatePurchaseItemAsync(PurchaseItem newPurchaseItem)
        {
            newPurchaseItem.Id = Guid.NewGuid();

            await purchaseItemRepo.AddAsync(newPurchaseItem);
            await purchaseItemRepo.SaveAsync();

            return newPurchaseItem;
        }

        /// <summary>Retrieves all Purchases.</summary>
        public async Task<IEnumerable<Purchase>> GetAllPurchasesAsync()
        {
            return await purchaseRepo.GetAllAsync();
        }

        /// <summary>Retrieves a single Purchase by id.</summary>
        public async Task<Purchase?> GetPurchaseByIdAsync(Guid id)
        {
            return await purchaseRepo.GetAsync(id);
        }

        public async Task<IEnumerable<PurchaseItem>> GetPurchaseItemsByPurchase(Guid purchaseId)
        {
            var purchase = await purchaseRepo.GetAsync(purchaseId);
            return purchase!.PurchaseItems!;
        }

        /// <summary>Updates an existing Purchase.</summary>
        public async Task<Purchase?> UpdatePurchaseAsync(Guid id, Purchase Purchase)
        {
            var existing = await purchaseRepo.GetAsync(id);
            if (existing == null) return null;

            // copy fields (you can use AutoMapper here if available)
            existing.purchaseDate = Purchase.purchaseDate;
            existing.state = Purchase.state;

            purchaseRepo.Update(existing);
            await purchaseRepo.SaveAsync();

            return existing;
        }


        public async Task<PurchaseItem?> UpdatePurchaseItemAsync(Guid id, PurchaseItem PurchaseItem)
        {
            var existing = await purchaseItemRepo.GetAsync(id);
            if (existing == null) return null;

            existing.Status = PurchaseItem.Status;
            existing.Quantity = PurchaseItem.Quantity;
            existing.Price = PurchaseItem.Price;

            purchaseItemRepo.Update(existing);
            await purchaseItemRepo.SaveAsync();
            return existing;
        }

        public async Task<bool> DeletePurchaseAsync(Guid id)
        {
            var deleted = await purchaseRepo.DeleteAsync(id);
            if(deleted)
                await purchaseRepo.SaveAsync();
            return deleted;
        }

        /// <summary>Deletes a Purchase by id.</summary>
        public async Task<bool> DeletePurchaseItemAsync(Guid id)
        {
            var deleted = await purchaseItemRepo.DeleteAsync(id);
            if (deleted)
            {
                await purchaseItemRepo.SaveAsync();
            }
            return deleted;
        }

    }
}
