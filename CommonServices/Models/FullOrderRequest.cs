using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Models
{
    public class FullOrderRequest
    {
        public  Order? order { get; set; }
        public  List<OrderItem>? orderItems { get; set; }
    }
}
