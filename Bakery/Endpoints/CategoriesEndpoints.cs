using Bakery.Data;
using Bakery.Dtos;
using Bakery.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Endpoints;

public static class CategoriesEndpoints
{
    public static RouteGroupBuilder MapCategoriesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("categories");

        // GET /categories
        group.MapGet("/", async (BakeryDbContext dbContext) =>
        {
            var categories = await dbContext.Categories.ToListAsync();

            var categoriesDtos = categories
                .Select(c => c.ToCategorySummaryDto())
                .ToList();

            return Results.Ok(categoriesDtos);
        });

        // GET /categories/{id}
        group.MapGet("/{id}", async (int id, BakeryDbContext dbContext) =>
        {
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(category.ToCategorySummaryDto());
        });

        // POST /categories
        group.MapPost("/", async (CreateCategoryDto newCategory, BakeryDbContext dbContext) =>
        {
            var category = newCategory.ToEntity();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var createdCategory = await dbContext.Categories
                .FirstAsync(c => c.Id == category.Id);

            return Results.Created(
                $"/products/{createdCategory.Id}",
                createdCategory.ToCategorySummaryDto());
        });

        // PUT /categories/{id}
        group.MapPut("/{id}", async (int id, UpdateCategoryDto updatedCategory, BakeryDbContext dbContext) =>
        {
            var existingCategory = await dbContext.Categories.FindAsync(id);

            if (existingCategory is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingCategory)
                .CurrentValues
                .SetValues(updatedCategory.ToEntity(id));
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /categories/{id}
        group.MapDelete("/{id}", async (int id, BakeryDbContext dbContext) =>
        {
            var existingCategory = await dbContext.Categories.FindAsync(id);

            if (existingCategory is null)
            {
                return Results.NotFound();
            }

            await dbContext.Categories
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }
}