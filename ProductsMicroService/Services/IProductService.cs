using CommonServices.Models;

namespace ProductsMicroService.Services
{
    public interface IProductService
    {
        Task<Product> Add(Product product);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(Guid id);
        Task<Product> CreateProductAsync(CreateProduct product);
        Task<Product> UpdateProductAsync(Guid id, Product product);
        Task<bool> DeleteProductAsync(Guid id);
    }
}
