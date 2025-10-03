using CommonServices.Models;
using Microsoft.EntityFrameworkCore;

using InventoryMicroService.Contexts;
using PaymentsMicroService.Repositories;

namespace InventoryMicroService.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _dbContext;

        public InventoryRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<InventoryItem> SaveAsync(InventoryItem item)
        {
            var newOrder = _dbContext.InventoryItems.Add(item);
            await _dbContext.SaveChangesAsync();
            return newOrder.Entity;
        }

        public async Task UpdateAsync(InventoryItem order)
        {
            _dbContext.InventoryItems.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<InventoryItem?> GetAsync(Guid id)
        {
            return await _dbContext.InventoryItems.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<InventoryItem?> GetByProductIdAsync(Guid Productid)
        {
            return await _dbContext.InventoryItems.FirstOrDefaultAsync(o => o.ProductId == Productid);
        }

        public async Task<InventoryItem> AddAsync(InventoryItem item)
        {
            return await SaveAsync(item);
        }

        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return await _dbContext.InventoryItems.ToListAsync();
        }

        public async Task<bool> DeleteAsync(Guid productId)
        {
            var item = _dbContext.InventoryItems.FirstOrDefault(o => o.ProductId == productId);
            if (item != null)
            {
                _dbContext.InventoryItems.Remove(item);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
