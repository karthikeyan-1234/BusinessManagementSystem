using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Models
{
    public enum PaymentStatus
    {
        Processed,
        Refunded,
        Failed
    }

    public class PaymentTransaction
    {
        public Guid TransactionId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Processed; // Processed, Refunded, Failed
    }

}
