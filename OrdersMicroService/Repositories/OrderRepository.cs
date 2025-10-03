using CommonServices.Models;
using Microsoft.EntityFrameworkCore;

using OrdersMicroService.Contexts;


namespace OrdersMicroService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _dbContext;

        public OrderRepository(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Order> SaveAsync(Order order)
        {
            var newOrder = _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            return newOrder.Entity;
        }

        public async Task UpdateAsync(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Order?> GetAsync(Guid orderId)
        {
            return await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _dbContext.Orders.ToListAsync();
        }
    }
}
