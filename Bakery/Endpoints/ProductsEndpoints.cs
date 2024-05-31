using AutoMapper;
using Bakery.Dtos;
using Bakery.Mapping;
using Bakery.Models;
using Bakery.Repositories;

namespace Bakery.Endpoints;

public static class ProductsEndpoints
{
    public static RouteGroupBuilder MapProductsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("products").WithTags("Products");

        // GET /products
        group.MapGet("/", async (int? categoryId, IProductRepository productRepository, IMapper mapper) =>
        {
            var products = await productRepository.GetProductsAsync(categoryId);

            var productsDtos = mapper.Map<List<ProductDetailsDto>>(products);

            return Results.Ok(productsDtos);
        })
        .WithName("GetProducts")
        .Produces<List<ProductDetailsDto>>(StatusCodes.Status200OK);

        // GET /products/{id}
        group.MapGet("/{id}", async (int id, IProductRepository productRepository, IMapper mapper) =>
        {
            var product = await productRepository.GetProductByIdAsync(id);

            if (product is null)
            {
                return Results.NotFound();
            }

            var productDto = mapper.Map<ProductDetailsDto>(product);

            return Results.Ok(productDto);
        })
        .WithName("GetProductById")
        .Produces<ProductDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /products
        group.MapPost("/", async (CreateProductDto newProduct, IProductRepository productRepository, IMapper mapper) =>
        {
            var createdProduct = await productRepository.CreateProductAsync(newProduct);

            var createdProductDetailsDto = mapper.Map<Product>(createdProduct);

            return Results.Created(
                $"/products/{createdProduct!.Id}",
                createdProductDetailsDto);
        })
        .WithName("CreateProduct")
        .Produces<ProductDetailsDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /products/{id}
        group.MapPut("/{id}", async (int id, UpdateProductDto updatedProduct, IProductRepository productRepository) =>
        {
            var result = await productRepository.UpdateProductAsync(id, updatedProduct);

            if (!result)
            {
                return Results.NotFound("Invalid data provided.");
            }

            return Results.NoContent();

        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /products/{id}
        group.MapDelete("/{id}", async (int id, IProductRepository productRepository) =>
        {
            var result = await productRepository.DeleteProductAsync(id);

            if (!result)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
