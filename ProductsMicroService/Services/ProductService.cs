using CommonServices.Models;
using CommonServices.Repositories;

namespace ProductsMicroService.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;

        public ProductService(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>Adds a product and saves it.</summary>
        public async Task<Product> Add(Product product)
        {
            await _productRepository.AddAsync(product);
            await _productRepository.SaveAsync();
            return product;
        }

        /// <summary>Retrieves all products.</summary>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        /// <summary>Retrieves a single product by id.</summary>
        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _productRepository.GetAsync(id);
        }

        /// <summary>Creates a new product (same as Add, kept for clarity).</summary>
        public async Task<Product> CreateProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            await _productRepository.SaveAsync();
            return product;
        }

        /// <summary>Updates an existing product.</summary>
        public async Task<Product?> UpdateProductAsync(Guid id, Product product)
        {
            var existing = await _productRepository.GetAsync(id);
            if (existing == null) return null;

            // copy fields (you can use AutoMapper here if available)
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Stock = product.Stock;

            _productRepository.Update(existing);
            await _productRepository.SaveAsync();

            return existing;
        }

        /// <summary>Deletes a product by id.</summary>
        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var deleted = await _productRepository.DeleteAsync(id);
            if (deleted)
            {
                await _productRepository.SaveAsync();
            }
            return deleted;
        }
    }
}
