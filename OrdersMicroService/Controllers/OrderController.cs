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

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            // Logic to create an order
            await _orderService.CreateOrderAsync(new Order
            {
                OrderId = Guid.NewGuid(),
                ProductId = orderRequest.ProductId,
                Quantity = orderRequest.Quantity,
                Price = orderRequest.Price,
                Status = OrderStatus.Pending
            });

            return Ok(new { Message = "Order created successfully", OrderId = 123 });
        }

        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            // Logic to retrieve all orders
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
    }
}
