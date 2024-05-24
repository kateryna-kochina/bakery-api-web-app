using Bakery.Dtos;
using Bakery.Models;

namespace Bakery.Repositories.Contracts;

public interface ICategoryRepository
{
    Task<List<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category?> CreateCategoryAsync(CreateCategoryDto newCategory);
    Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updatedCategory);
    Task<bool> DeleteCategoryAsync(int id);
}
