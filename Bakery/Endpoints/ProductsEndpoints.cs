using Bakery.Data;
using Bakery.Dtos;
using Bakery.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Endpoints;

public static class ProductsEndpoints
{
    public static RouteGroupBuilder MapProductsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("products").WithTags("Products");

        // GET /products
        group.MapGet("/", async (int? categoryId, BakeryDbContext dbContext) =>
        {
            var query = dbContext.Products
                    .Include(c => c.Category)
                    .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId);
            }

            var products = await query.ToListAsync();

            var productsDtos = products
                .Select(p => p.ToProductDetailsDto())
                .ToList();

            return Results.Ok(productsDtos);
        })
        .WithName("GetProducts")
        .Produces<List<ProductDetailsDto>>(StatusCodes.Status200OK);

        // GET /products/{id}
        group.MapGet("/{id}", async (int id, BakeryDbContext dbContext) =>
        {
            var product = await dbContext.Products
                .Include(c => c.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(product.ToProductDetailsDto());
        })
        .WithName("GetProductById")
        .Produces<ProductDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /products
        group.MapPost("/", async (CreateProductDto newProduct, BakeryDbContext dbContext) =>
        {
            // validation if provided category exists
            var category = await dbContext.Categories.FindAsync(newProduct.CategoryId);

            if (category is null)
            {
                return Results.NotFound("Unknown CategoryId provided.");
            }

            var product = newProduct.ToEntity();

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            var createdProduct = await dbContext.Products
                .Include(c => c.Category)
                .FirstAsync(p => p.Id == product.Id);

            return Results.Created(
                $"/products/{createdProduct.Id}",
                createdProduct.ToProductDetailsDto());
        })
        .WithName("CreateProduct")
        .Produces<ProductDetailsDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /products/{id}
        group.MapPut("/{id}", async (int id, UpdateProductDto updatedProduct, BakeryDbContext dbContext) =>
        {
            var existingProduct = await dbContext.Products.FindAsync(id);

            if (existingProduct is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingProduct)
                .CurrentValues
                .SetValues(updatedProduct.ToEntity(id));
            await dbContext.SaveChangesAsync();

            return Results.NoContent();

        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /products/{id}
        group.MapDelete("/{id}", async (int id, BakeryDbContext dbContext) =>
        {
            var existingProduct = await dbContext.Products.FindAsync(id);

            if (existingProduct is null)
            {
                return Results.NotFound();
            }

            await dbContext.Products
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();

            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
