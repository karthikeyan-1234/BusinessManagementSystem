using CommonServices.Models;

using OrdersMicroService.Repositories;

namespace OrdersMicroService.Services
{
    public class OrderService : IOrderService
    {
        public readonly IOrderRepository _orderRepository;
        public readonly IOrderItemRepository _orderItemRepository;

        public OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<string> CreateOrderAsync(Order newOrder)
        {
            var orderId = Guid.NewGuid();
            newOrder.Id = orderId;
            await _orderRepository.SaveAsync(newOrder);
            return orderId.ToString();
        }

        //Create full order with order items
        public async Task<FullOrderRequest> CreateOrUpdateFullOrderAsync(FullOrderRequest fullOrderRequest)
        {
            //Create order if not exists

            if (fullOrderRequest!.order!.Id == Guid.Empty)
            {
                var newOrder = new Order
                {
                    customerName = fullOrderRequest.order.customerName,
                    orderDate = fullOrderRequest.order.orderDate,
                    status = fullOrderRequest.order.status

                };

                fullOrderRequest.order = await _orderRepository.SaveAsync(newOrder);
            }

            //Check if Guid of order items are empty, if yes then call SaveAsync else call UpdateAsync

            bool isCreated = false;

            foreach (var item in fullOrderRequest.orderItems!)
            {
                item.OrderId = fullOrderRequest.order.Id;
                if (item.Id == Guid.Empty)
                {
                    await _orderItemRepository.SaveAsync(item);
                    isCreated = true;
                }
            }

            if (!isCreated)
                await _orderItemRepository.UpdateAsync(fullOrderRequest.orderItems);

            return fullOrderRequest;
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
            var order = await _orderRepository.GetByIdAsync(OrderId);
            order!.OrderItems = (ICollection<OrderItem>?)await _orderItemRepository.GetByOrderIdAsync(OrderId);
            return order;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateAsync(order);
        }
    }
}
