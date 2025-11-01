using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Models
{


    public class PurchaseItem
    {
        public Guid Id { get; set; }
        public Guid PurchaseId { get; set; }

        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Pending, Completed, Failed

        public Purchase? Purchase { get; set; }

    }
}
