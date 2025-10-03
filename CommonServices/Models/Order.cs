using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Models
{
    public enum OrderStatus
    {
        Pending,
        Completed,
        Failed
    }

    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Pending, Completed, Failed
        public Guid? PaymentTransactionId { get; set; }
    }

    public record OrderRequest(Guid ProductId, int Quantity, decimal Price);

}
