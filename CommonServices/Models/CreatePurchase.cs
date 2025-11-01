using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Models
{
    public class CreatePurchase
    {
        public DateTime purchaseDate { get; set; }
        public Guid? PaymentTransactionId { get; set; }
        public OrderStatus state { get; set; }
    }
}
