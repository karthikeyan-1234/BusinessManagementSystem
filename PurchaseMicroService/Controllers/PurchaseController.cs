using CommonServices.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PurchaseMicroService.Services;

namespace PurchaseMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        IPurchaseService purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            this.purchaseService = purchaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderssAsync()
        {
            return Ok(await purchaseService.GetAllPurchasesAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdAsync(Guid id)
        {
            var purchase = await purchaseService.GetPurchaseByIdAsync(id);

            if(purchase != null)
                return Ok(purchase);

            return NotFound();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrderasync(CreatePurchase newPurchase)
        {
            var purchase = await purchaseService.CreatePurchaseAsync(newPurchase);
            return Ok(purchase);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var result = await purchaseService.DeletePurchaseAsync(id);

            if(result)
                return Ok(result);

            return BadRequest();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync(Purchase existingPurchase)
        {
            var purchase = await purchaseService.GetPurchaseByIdAsync(existingPurchase.id);

            if (purchase != null)
            {
                var updatedPurchase = await purchaseService.UpdatePurchaseAsync(existingPurchase.id,existingPurchase);
                return Ok(updatedPurchase);
            }

            return NotFound();
        }
    }
}
