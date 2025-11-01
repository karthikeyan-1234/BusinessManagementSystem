using CommonServices.Models;

using Microsoft.EntityFrameworkCore;

namespace PurchaseMicroService.Contexts
{
    public class PurchaseDbContext: DbContext
    {
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchasesItem { get; set; }


        public PurchaseDbContext(DbContextOptions<PurchaseDbContext> options): base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var purchase = modelBuilder.Entity<Purchase>();
            var purchaseItem = modelBuilder.Entity<PurchaseItem>();

            purchase.HasKey(p => p.id);
            purchaseItem.HasKey(p => p.Id);

            purchase.HasMany(p => p.PurchaseItems).WithOne(pi => pi.Purchase).HasForeignKey(p => p.PurchaseId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
