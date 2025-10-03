using CommonServices.Models;

using InventoryMicroService.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace InventoryMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        //Add new inventory
        [HttpPost("add")]
        public async Task<IActionResult> AddInventory([FromBody] InventoryItem inventory)
        {
            var newInventory = await _inventoryService.AddInventory(inventory);

            return Ok(newInventory);
        }

        //Get all inventory
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllInventory()
        {
            var inventoryList = await _inventoryService.GetAllInventory();
            return Ok(inventoryList);
        }

        //Delete inventory
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteInventory([FromBody] InventoryItem inventory)
        {
            var result = await _inventoryService.DeleteInventory(inventory);
            return Ok(result);
        }
    }
}
