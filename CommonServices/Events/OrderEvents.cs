using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Events
{
        // Fired when an order is created
    public record OrderCreatedEvent(Guid OrderId, decimal Price, Guid ProductId, float Quantity);

    // Request to reserve inventory
    public record ReserveInventoryRequest(Guid OrderId, Guid ProductId, float Quantity);

    // Request to refund a payment
    public record RefundPaymentRequest(Guid OrderId);

}
