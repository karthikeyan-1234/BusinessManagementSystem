using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Events
{
    // Success
    public record PaymentProcessedEvent(Guid OrderId, Guid TransactionId,float qty);

    // Failure
    public record PaymentFailedEvent(Guid OrderId, string Reason, float qty);

    // Refund acknowledgment (optional, could just be logged)
    public record PaymentRefundedEvent(Guid OrderId, Guid TransactionId);


}
