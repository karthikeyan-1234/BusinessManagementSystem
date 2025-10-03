using CommonServices.Models;

using Microsoft.EntityFrameworkCore;

namespace OrdersMicroService.Contexts
{
    public class OrderDbContext: DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
            modelBuilder.Entity<Order>().Property(o => o.Status).HasConversion<string>();
        }
    }
}
