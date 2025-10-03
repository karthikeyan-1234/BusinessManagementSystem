using CommonServices.Models;

using Microsoft.EntityFrameworkCore;

namespace InventoryMicroService.Contexts
{
    public class InventoryDbContext: DbContext
    {
        public DbSet<InventoryItem> InventoryItems { get; set; }

        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<InventoryItem>().HasKey(o => o.Id);

        }
    }
}
