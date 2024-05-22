using Bakery.Data;
using Bakery.Dtos;
using Bakery.Models;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Endpoints;

public static class ProductsEndpoints
{
    const string GetProductEndpointName = "GetProduct";

    public static RouteGroupBuilder MapProductsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("products");

        // GET /products
        group.MapGet("/", async (BakeryDbContext dbContext) =>
            await dbContext.Products
                .ToListAsync()
        );

        group.MapPost("/", async (BakeryDbContext dbContext, CreateProductDto newProduct) =>
        {
            var product = new Product
            {
                Title = newProduct.Title,
                Price = newProduct.Price,
                Image = newProduct.Image,
                CategoryId = newProduct.CategoryId,
                Category = dbContext.Categories.FirstOrDefault(c => c.Id == newProduct.CategoryId),
                Description = newProduct.Description
            };

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/products/{product.Id}", product);
        }
        );

        return group;
    }
}
