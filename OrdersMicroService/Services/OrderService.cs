using CommonServices.Models;

using OrdersMicroService.Repositories;

namespace OrdersMicroService.Services
{
    public class OrderService : IOrderService
    {
        public readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<string> CreateOrderAsync(Order newOrder)
        {
            var orderId = Guid.NewGuid();
            newOrder.Id = orderId;
            await _orderRepository.SaveAsync(newOrder);
            return orderId.ToString();
        }

        public async Task<bool> DeleteOrderAsync(Guid OrderId)
        {
            return (await _orderRepository.DeleteAsync(OrderId) > 0);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid OrderId)
        {
            return await _orderRepository.GetByIdAsync(OrderId);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateAsync(order);
        }
    }
}
