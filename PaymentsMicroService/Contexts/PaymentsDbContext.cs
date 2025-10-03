using CommonServices.Models;

using Microsoft.EntityFrameworkCore;

namespace PaymentsMicroService.Contexts
{
    public class PaymentsDbContext: DbContext
    {
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PaymentTransaction>().HasKey(o => o.TransactionId);
            modelBuilder.Entity<PaymentTransaction>().Property(o => o.Status).HasConversion<string>();
        }
    }
}
