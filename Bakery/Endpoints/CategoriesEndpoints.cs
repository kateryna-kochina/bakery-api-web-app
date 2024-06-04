using AutoMapper;
using Bakery.Dtos;
using Bakery.Repositories.Contracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bakery.Endpoints;

public static class CategoriesEndpoints
{
    public static RouteGroupBuilder MapCategoriesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("categories").WithTags("Categories");

        group.MapGet("/", GetCategoriesAsync)
            .WithName(nameof(GetCategoriesAsync))
            .Produces<List<CategoryDetailsDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetCategoryByIdAsync)
            .WithName(nameof(GetCategoryByIdAsync))
            .Produces<CategoryDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateCategoryAsync)
            .WithName(nameof(CreateCategoryAsync))
            .Produces<CategoryDetailsDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id}", UpdateCategoryAsync)
            .WithName(nameof(UpdateCategoryAsync))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // DELETE /categories/{id}
        group.MapDelete("/{id}", DeleteCategoryAsync)
            .WithName(nameof(DeleteCategoryAsync))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }

    // GET /categories
    public static async Task<Ok<List<CategoryDetailsDto>>> GetCategoriesAsync(
        ICategoryRepository categoryRepository, IMapper mapper)
    {
        var categories = await categoryRepository.GetCategoriesAsync();

        var categoriesDtos = mapper.Map<List<CategoryDetailsDto>>(categories);

        return TypedResults.Ok(categoriesDtos);
    }

    // GET /categories/{id}
    public static async Task<Results<Ok<CategoryDetailsDto>, NotFound<string>>> GetCategoryByIdAsync(
        int id, ICategoryRepository categoryRepository, IMapper mapper)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(id);

        if (category is null)
        {
            return TypedResults.NotFound(EndpointsConstants.Category.NOT_FOUND_MESSAGE);
        }

        var categoryDetailsDto = mapper.Map<CategoryDetailsDto>(category);

        return TypedResults.Ok(categoryDetailsDto);
    }

    // POST /categories
    public static async Task<Results<Created<CategoryDetailsDto>, BadRequest<string>>> CreateCategoryAsync(
        CreateCategoryDto newCategory, ICategoryRepository categoryRepository, IMapper mapper, IValidator<CreateCategoryDto> validator)
    {
        if (newCategory is null)
        {
            return TypedResults.BadRequest(EndpointsConstants.Category.BAD_REQUEST_MESSAGE);
        }

        ValidationResult validationResult = await validator.ValidateAsync(newCategory);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return TypedResults.BadRequest(errors);
        }

        var createdCategory = await categoryRepository.CreateCategoryAsync(newCategory);

        var createdCategoryDetailsDto = mapper.Map<CategoryDetailsDto>(createdCategory);

        return TypedResults.Created($"/categories/{createdCategory!.Id}", createdCategoryDetailsDto);
    }

    // PUT /categories/{id}
    public static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> UpdateCategoryAsync(
        int id, UpdateCategoryDto updatedCategory, ICategoryRepository categoryRepository, IValidator<UpdateCategoryDto> validator)
    {
        if (updatedCategory is null)
        {
            return TypedResults.BadRequest(EndpointsConstants.Category.BAD_REQUEST_MESSAGE);
        }

        var validationResult = await validator.ValidateAsync(updatedCategory);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return TypedResults.BadRequest(errors);
        }
        
        var result = await categoryRepository.UpdateCategoryAsync(id, updatedCategory);

        if (!result)
        {
            return TypedResults.NotFound(EndpointsConstants.Category.NOT_FOUND_MESSAGE);
        }

        return TypedResults.NoContent();
    }

    // DELETE /categories/{id}
    public static async Task<Results<NoContent, NotFound<string>>> DeleteCategoryAsync(
        int id, ICategoryRepository categoryRepository)
    {
        var result = await categoryRepository.DeleteCategoryAsync(id);

        if (!result)
        {
            return TypedResults.NotFound(EndpointsConstants.Category.NOT_FOUND_MESSAGE);
        }

        return TypedResults.NoContent();
    }
}
