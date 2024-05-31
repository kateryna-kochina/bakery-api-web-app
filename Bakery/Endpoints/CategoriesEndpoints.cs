using AutoMapper;
using Bakery.Dtos;
using Bakery.Repositories.Contracts;

namespace Bakery.Endpoints;

public static class CategoriesEndpoints
{
    public static RouteGroupBuilder MapCategoriesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("categories").WithTags("Categories");

        // GET /categories
        group.MapGet("/", async (ICategoryRepository categoryRepository, IMapper mapper) =>
        {
            var categories = await categoryRepository.GetCategoriesAsync();

            var categoriesDtos = mapper.Map<List<CategoryDetailsDto>>(categories);

            return Results.Ok(categoriesDtos);
        })
        .WithName("GetCategories")
        .Produces<List<CategoryDetailsDto>>(StatusCodes.Status200OK);

        // GET /categories/{id}
        group.MapGet("/{id}", async (int id, ICategoryRepository categoryRepository, IMapper mapper) =>
        {
            var category = await categoryRepository.GetCategoryByIdAsync(id);

            if (category is null)
            {
                return Results.NotFound();
            }

            var categoryDetailsDto = mapper.Map<CategoryDetailsDto>(category);

            return Results.Ok(categoryDetailsDto);
        })
        .WithName("GetCategoryById")
        .Produces<CategoryDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /categories
        group.MapPost("/", async (CreateCategoryDto newCategory, ICategoryRepository categoryRepository, IMapper mapper) =>
        {
            var createdCategory = await categoryRepository.CreateCategoryAsync(newCategory);

            var createdCategoryDetailsDto = mapper.Map<CategoryDetailsDto>(createdCategory);

            return Results.Created(
                $"/categories/{createdCategory!.Id}",
                createdCategoryDetailsDto);
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
