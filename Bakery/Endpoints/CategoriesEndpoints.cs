using Bakery.Dtos;
using Bakery.Mapping;
using Bakery.Repositories.Contracts;

namespace Bakery.Endpoints;

public static class CategoriesEndpoints
{
    public static RouteGroupBuilder MapCategoriesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("categories").WithTags("Categories");

        // GET /categories
        group.MapGet("/", async (ICategoryRepository categoryRepository) =>
        {
            var categories = await categoryRepository.GetCategoriesAsync();

            var categoriesDtos = categories
                .Select(c => c.ToCategoryDetailsDto())
                .ToList();

            return Results.Ok(categoriesDtos);
        })
        .WithName("GetCategories")
        .Produces<List<CategoryDetailsDto>>(StatusCodes.Status200OK);

        // GET /categories/{id}
        group.MapGet("/{id}", async (int id, ICategoryRepository categoryRepository) =>
        {
            var category = await categoryRepository.GetCategoryByIdAsync(id);

            if (category is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(category.ToCategoryDetailsDto());
        })
        .WithName("GetCategoryById")
        .Produces<CategoryDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /categories
        group.MapPost("/", async (CreateCategoryDto newCategory, ICategoryRepository categoryRepository) =>
        {
            var createdCategory = await categoryRepository.CreateCategoryAsync(newCategory);

            return Results.Created(
                $"/categories/{createdCategory!.Id}",
                createdCategory.ToCategoryDetailsDto());
        })
        .WithName("CreateCategory")
        .Produces<CategoryDetailsDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // PUT /categories/{id}
        group.MapPut("/{id}", async (int id, UpdateCategoryDto updatedCategory, ICategoryRepository categoryRepository) =>
        {
            var result = await categoryRepository.UpdateCategoryAsync(id, updatedCategory);

            if (!result)
            {
                return Results.NotFound("Invalid data provided.");
            }

            return Results.NoContent();
        })
        .WithName("UpdateCategory")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /categories/{id}
        group.MapDelete("/{id}", async (int id, ICategoryRepository categoryRepository) =>
        {
            var result = await categoryRepository.DeleteCategoryAsync(id);

            if (!result)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .WithName("DeleteCategory")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
