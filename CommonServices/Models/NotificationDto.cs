using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Models
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? Link { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
