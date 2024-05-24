using Bakery.Dtos;
using Bakery.Models;

namespace Bakery.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync(int? categoryId);
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> CreateProductAsync(CreateProductDto newProduct);
        Task<bool> UpdateProductAsync(int id, UpdateProductDto updatedProduct);
        Task<bool> DeleteProductAsync(int id);
    }
}
