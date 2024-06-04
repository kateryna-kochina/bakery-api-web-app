using AutoMapper;
using Bakery.Dtos;
using Bakery.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bakery.Endpoints;

public static class ProductsEndpoints
{
    public static RouteGroupBuilder MapProductsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("products").WithTags("Products");

        group.MapGet("/", GetProductsAsync)
        .WithName(nameof(GetProductsAsync))
        .Produces<List<ProductDetailsDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetProductByIdAsync)
        .WithName(nameof(GetProductByIdAsync))
        .Produces<ProductDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateProductAsync)
        .WithName(nameof(CreateProductAsync))
        .Produces<ProductDetailsDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", UpdateProductAsync)
        .WithName(nameof(UpdateProductAsync))
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", DeleteProductAsync)
        .WithName(nameof(DeleteProductAsync))
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }

    // GET /products
    public static async Task<Ok<List<ProductDetailsDto>>> GetProductsAsync(int? categoryId, IProductRepository productRepository, IMapper mapper)
    {
        var products = await productRepository.GetProductsAsync(categoryId);

        var productsDtos = mapper.Map<List<ProductDetailsDto>>(products);

        return TypedResults.Ok(productsDtos);
    }

    // GET /products/{id}
    public static async Task<Results<Ok<ProductDetailsDto>, NotFound<string>>> GetProductByIdAsync(int id, IProductRepository productRepository, IMapper mapper)
    {
        var product = await productRepository.GetProductByIdAsync(id);

        if (product is null)
        {
            return TypedResults.NotFound(EndpointsConstants.Product.NOT_FOUND_MESSAGE);
        }

        var productDto = mapper.Map<ProductDetailsDto>(product);

        return TypedResults.Ok(productDto);
    }

    // POST /products
    public static async Task<Results<Created<ProductDetailsDto>, BadRequest<string>>> CreateProductAsync(CreateProductDto newProduct, IProductRepository productRepository, IMapper mapper, IValidator<CreateProductDto> validator)
    {
        if (newProduct is null)
        {
            return TypedResults.BadRequest(EndpointsConstants.Product.BAD_REQUEST_MESSAGE);
        }

        ValidationResult validationResult = await validator.ValidateAsync(newProduct);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return TypedResults.BadRequest(errors);
        }

        var createdProduct = await productRepository.CreateProductAsync(newProduct);

        var createdProductDetailsDto = mapper.Map<ProductDetailsDto>(createdProduct);

        return TypedResults.Created($"/products/{createdProduct!.Id}", createdProductDetailsDto);
    }

    // PUT /products/{id}
    public static async Task<Results<NoContent, NotFound<string>>> UpdateProductAsync(int id, UpdateProductDto updatedProduct, IProductRepository productRepository)
    {
        var result = await productRepository.UpdateProductAsync(id, updatedProduct);

        if (!result)
        {
            return TypedResults.NotFound(EndpointsConstants.Product.NOT_FOUND_MESSAGE);
        }

        return TypedResults.NoContent();
    }

    // DELETE /products/{id}
    public static async Task<Results<NoContent, NotFound<string>>> DeleteProductAsync(int id, IProductRepository productRepository)
    {
        var result = await productRepository.DeleteProductAsync(id);

        if (!result)
        {
            return TypedResults.NotFound(EndpointsConstants.Product.NOT_FOUND_MESSAGE);
        }

        return TypedResults.NoContent();
    }
}
