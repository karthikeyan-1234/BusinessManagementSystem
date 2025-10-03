using CommonServices.Models;

using Microsoft.EntityFrameworkCore;

namespace ProductsMicroService.Contexts
{
    public class ProductsDbContext: DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasKey(o => o.ProductId);
        }
    }
}
