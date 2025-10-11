using CommonServices.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using OrdersMicroService.Services;

namespace OrdersMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        IOrderItemService _orderService;
        public OrderItemController(IOrderItemService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderItemssAsync()
        {
            // Logic to retrieve all orders
            var orders = await _orderService.GetAllOrderItemsAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderItemByIdAsync(Guid id)
        {
            // Logic to retrieve an order by ID
            var order = await _orderService.GetOrderItemByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { Message = "Order not found" });
            }
            return Ok(order);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrderItem([FromBody] OrderItemRequest orderItemRequest)
        {
            // Logic to create an order
            await _orderService.CreateOrderItemAsync(new OrderItem
            {
                OrderId = orderItemRequest.OrderId,
                ProductId = orderItemRequest.ProductId,
                Quantity = orderItemRequest.Quantity,
                Price = orderItemRequest.Price,
                Status = OrderStatus.Pending
            });

            return Ok(new { Message = "Order created successfully", OrderId = 123 });
        }

        [HttpPost("createMultiple")]
        public async Task<IActionResult> CreateMultipleOrderItems([FromBody] List<OrderItemRequest> orderItemRequests)
        {
            // Logic create and add multiple order items to order
            var orderItems = orderItemRequests.Select(o => new OrderItem
            {
                OrderId = o.OrderId,
                ProductId = o.ProductId,
                Quantity = o.Quantity,
                Price = o.Price,
                Status = OrderStatus.Pending
            });

            var createdItems = await _orderService.CreateMultipleOrderItemsAsync(orderItems);
            return Ok(new { Message = "Order items created successfully", CreatedItems = createdItems });
        }


        [HttpPost("delete")]
        public async Task<IActionResult> DeleteOrderItemAsync([FromBody] Guid orderId)
        {
            // Logic to delete an order
            var result = await _orderService.DeleteOrderItemAsync(orderId);
            if (result)
            {
                return Ok(new { Message = "Order deleted successfully" });
            }
            return NotFound(new { Message = "Order not found" });
        }

        
    }
}
