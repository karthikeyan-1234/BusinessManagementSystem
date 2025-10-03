using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Models
{
    public class InventoryItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public float AvailableQuantity { get; set; }
    }
}
