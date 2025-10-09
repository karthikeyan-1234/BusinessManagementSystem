using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string? customerName { get; set; }
        public DateTime orderDate { get; set; }
        public OrderStatus status { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }

    public record OrderRequest(string? customerName, DateTime orderDate, OrderStatus status);
}
