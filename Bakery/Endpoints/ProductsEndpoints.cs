using Bakery.Data;
using Bakery.Dtos;
using Bakery.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Endpoints;

public static class ProductsEndpoints
{
    public static RouteGroupBuilder MapProductsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("products");

        // GET /products
        group.MapGet("/", async (BakeryDbContext dbContext) =>
        {
            var products = await dbContext.Products
                .Include(c => c.Category)
                .ToListAsync();

            return Results.Ok(products);
        });

        // GET /products/{id}
        group.MapGet("/{id}", async (BakeryDbContext dbContext, int id) =>
        {
            var product = await dbContext.Products
                .Include(c => c.Category)
                .FirstAsync(p => p.Id == id);

            if (product is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(product.ToProductSummaryDto());
        });

        // POST /products
        group.MapPost("/", async (BakeryDbContext dbContext, CreateProductDto newProduct) =>
        {
            // validation if category exists
            var category = await dbContext.Categories.FindAsync(newProduct.CategoryId);

            if (category is null)
            {
                return Results.BadRequest("Unknown CategoryId provided.");
            }

            var product = newProduct.ToEntity();

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            var createdProduct = await dbContext.Products
                .Include(c => c.Category)
                .FirstAsync(p => p.Id == product.Id);

            return Results.Created(
                $"/products/{createdProduct.Id}",
                createdProduct.ToProductSummaryDto());
        });

        return group;
    }
}
