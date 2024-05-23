using Bakery.Dtos;
using Bakery.Models;

namespace Bakery.Mapping;

public static class CategoryMapping
{
    public static CategoryDetailsDto ToCategoryDetailsDto(this Category category)
    {
        return new CategoryDetailsDto(
            category.Id,
            category.CategoryName
        );
    }

    public static Category ToEntity(this CreateCategoryDto createdCategory)
    {
        return new Category()
        {
            CategoryName = createdCategory.CategoryName
        };
    }

    public static Category ToEntity(this UpdateCategoryDto updatedCategory, int id)
    {
        return new Category()
        {
            Id = id,
            CategoryName = updatedCategory.CategoryName
        };
    }

    public static Category ToEntity(this CategoryDetailsDto categoryDto)
    {
        return new Category()
        {
            CategoryName = categoryDto.CategoryName
        };
    }
}
