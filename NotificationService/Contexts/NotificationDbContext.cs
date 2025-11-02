using CommonServices.Models;

using Microsoft.EntityFrameworkCore;

namespace NotificationService.Contexts
{
    public class NotificationDbContext: DbContext
    {
        public DbSet<Notification> Notifications { get; set; }

        public NotificationDbContext(DbContextOptions<NotificationDbContext> options): base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=NotificationsDb;User Id=sa;Password=Password_123#;TrustServerCertificate=True;");
            }
        }
    }
}
