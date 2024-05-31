using AutoMapper;
using Bakery.Data;
using Bakery.Dtos;
using Bakery.Models;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly BakeryDbContext _dbContext;
    private readonly IMapper _mapper;

    public ProductRepository(BakeryDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<Product>> GetProductsAsync(int? categoryId)
    {
        var query = _dbContext.Products
            .Include(c => c.Category)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId);
        }

        return await query.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var product = await _dbContext.Products
            .Include(c => c.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return null;
        }

        return product;
    }

    public async Task<Product?> CreateProductAsync(CreateProductDto newProduct)
    {
        // validation if provided category exists
        var category = await _dbContext.Categories.FindAsync(newProduct.CategoryId);

        if (category is null)
        {
            return null;
        }

        var product = _mapper.Map<Product>(newProduct);

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return product;
    }

    public async Task<bool> UpdateProductAsync(int id, UpdateProductDto updatedProduct)
    {
        var existingProduct = await _dbContext.Products.FindAsync(id);

        if (existingProduct is null)
        {
            return false;
        }

        // Ensure all foreign keys are valid before saving
        if (!_dbContext.Categories.Any(c => c.Id == updatedProduct.CategoryId))
        {
            return false;
        }

        var updateProductDto = _mapper.Map(updatedProduct, existingProduct);

        // Map updatedProduct to the existingProduct entity
        _dbContext.Entry(existingProduct)
            .CurrentValues
            .SetValues(updateProductDto);

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var existingProduct = await _dbContext.Products.FindAsync(id);

        if (existingProduct is null)
        {
            return false;
        }

        await _dbContext.Products
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();

        return true;
    }
}
