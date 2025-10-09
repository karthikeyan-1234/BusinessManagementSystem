using CommonServices.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using OrdersMicroService.Services;

namespace OrdersMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderssAsync()
        {
            // Logic to retrieve all orders
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdAsync(Guid id)
        {
            // Logic to retrieve an order by ID
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { Message = "Order not found" });
            }
            return Ok(order);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest OrderRequest)
        {
            // Logic to create an order
            await _orderService.CreateOrderAsync(new Order
            {
                customerName = OrderRequest.customerName,
                orderDate = OrderRequest.orderDate,
                status = OrderRequest.status
            });

            return Ok(new { Message = "Order created successfully", OrderId = 123 });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteOrderAsync([FromBody] Guid orderId)
        {
            // Logic to delete an order
            var result = await _orderService.DeleteOrderAsync(orderId);
            if (result)
            {
                return Ok(new { Message = "Order deleted successfully" });
            }
            return NotFound(new { Message = "Order not found" });
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateOrderAsync([FromBody] Order order)
        {
            // Logic to update an order
            var existingOrder = await _orderService.GetOrderByIdAsync(order.Id);
            if (existingOrder == null)
            {
                return NotFound(new { Message = "Order not found" });
            }
            await _orderService.UpdateOrderAsync(order);
            return Ok(new { Message = "Order updated successfully" });
        }

    }
}
