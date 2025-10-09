using CommonServices.Models;
using Microsoft.EntityFrameworkCore;

using OrdersMicroService.Contexts;


namespace OrdersMicroService.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly OrderDbContext _dbContext;

        public OrderItemRepository(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OrderItem> SaveAsync(OrderItem order)
        {
            var newOrder = _dbContext.OrderItems.Add(order);
            await _dbContext.SaveChangesAsync();
            return newOrder.Entity;
        }

        public async Task UpdateAsync(OrderItem order)
        {
            _dbContext.OrderItems.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<OrderItem?> GetByIdAsync(Guid orderId)
        {
            return await _dbContext.OrderItems.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            return await _dbContext.OrderItems.ToListAsync();
        }

        public async Task<OrderItem?> GetAsync(Guid orderId)
        {
            return await _dbContext.OrderItems.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task DeleteAsync(OrderItem order)
        {
            _dbContext.OrderItems.Remove(order);
            await _dbContext.SaveChangesAsync();
        }
    }
}
