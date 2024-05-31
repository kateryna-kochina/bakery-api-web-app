using AutoMapper;
using Bakery.Data;
using Bakery.Dtos;
using Bakery.Models;
using Bakery.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly BakeryDbContext _dbContext;
    private readonly IMapper _mapper;

    public CategoryRepository(BakeryDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var categories = await _dbContext.Categories.ToListAsync();

        return categories;
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return null;
        }

        return category;
    }

    public async Task<Category?> CreateCategoryAsync(CreateCategoryDto newCategory)
    {
        var category = _mapper.Map<Category>(newCategory);

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        return category;
    }

    public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updatedCategory)
    {
        var existingCategory = await _dbContext.Categories.FindAsync(id);

        if (existingCategory is null)
        {
            return false;
        }

        var updatedCategoryDto = _mapper.Map(updatedCategory, existingCategory);

        _dbContext.Entry(existingCategory)
            .CurrentValues
            .SetValues(updatedCategoryDto);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var existingCategory = await _dbContext.Categories.FindAsync(id);

        if (existingCategory is null)
        {
            return false;
        }

        await _dbContext.Categories
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

        return true;
    }
}
