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

    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Pending, Completed, Failed
        public Guid? PaymentTransactionId { get; set; }

        public Order? Order { get; set; }
    }

    public record OrderItemRequest(Guid OrderId, Guid ProductId, int Quantity, decimal Price);

}
