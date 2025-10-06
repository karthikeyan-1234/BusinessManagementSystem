using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Models
{
    public class PaymentIntentCreateRequest
    {
        public long Amount { get; set; }
        public string? Currency { get; set; }
        public Guid OrderId { get; set; }
    }
}
