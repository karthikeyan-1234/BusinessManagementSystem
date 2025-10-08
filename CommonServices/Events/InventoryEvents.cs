using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Events
{
    public record ReserveInventory(Guid OrderId, Guid ProductId, float qty);

    public record ReleaseInventory(Guid OrderId, Guid ProductId, float qty);

    // Success
    public record InventoryReservedEvent(Guid OrderId,Guid ProductId);

    // Failure
    public record InventoryFailedEvent(Guid OrderId, Guid ProductId, string Reason);
}
