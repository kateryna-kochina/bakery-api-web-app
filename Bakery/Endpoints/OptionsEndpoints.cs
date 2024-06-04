using AutoMapper;
using Bakery.Dtos;
using Bakery.Repositories.Contracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bakery.Endpoints;

public static class OptionsEndpoints
{
    public static RouteGroupBuilder MapOptionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("options").WithTags("Options");

        group.MapGet("/", GetOptionsAsync)
            .WithName(nameof(GetOptionsAsync))
            .Produces<List<OptionDetailsDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetOptionByIdAsync)
            .WithName(nameof(GetOptionByIdAsync))
            .Produces<OptionDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateOptionAsync)
            .WithName(nameof(CreateOptionAsync))
            .Produces<OptionDetailsDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id}", UpdateOptionAsync)
            .WithName(nameof(UpdateOptionAsync))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", DeleteOptionAsync)
            .WithName(nameof(DeleteOptionAsync))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }

    // GET /options
    public static async Task<Ok<List<OptionDetailsDto>>> GetOptionsAsync(
        IOptionRepository optionRepository, IMapper mapper)
    {
        var options = await optionRepository.GetOptionsAsync();

        var optionsDtos = mapper.Map<List<OptionDetailsDto>>(options);

        return TypedResults.Ok(optionsDtos);
    }

    // GET /options/{id}
    public static async Task<Results<Ok<OptionDetailsDto>, NotFound<string>>> GetOptionByIdAsync(
        int id, IOptionRepository optionRepository, IMapper mapper)
    {
        var option = await optionRepository.GetOptionByIdAsync(id);

        if (option is null)
        {
            return TypedResults.NotFound(EndpointsConstants.Option.NOT_FOUND_MESSAGE);
        }

        var optionDto = mapper.Map<OptionDetailsDto>(option);

        return TypedResults.Ok(optionDto);
    }

    // POST /options
    public static async Task<Results<Created<OptionDetailsDto>, BadRequest<string>>> CreateOptionAsync(
        CreateOptionDto newOption, IOptionRepository optionRepository, IMapper mapper, IValidator<CreateOptionDto> validator)
    {
        if (newOption is null)
        {
            return TypedResults.BadRequest(EndpointsConstants.Option.BAD_REQUEST_MESSAGE);
        }

        ValidationResult validationResult = await validator.ValidateAsync(newOption);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return TypedResults.BadRequest(errors);
        }

        var createdOption = await optionRepository.CreateOptionAsync(newOption);

        var createdOptionDetailsDto = mapper.Map<OptionDetailsDto>(createdOption);

        return TypedResults.Created($"/options/{createdOption!.Id}", createdOptionDetailsDto);
    }

    // PUT /options/{id}
    public static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> UpdateOptionAsync(
        int id, UpdateOptionDto updatedOption, IOptionRepository optionRepository, IValidator<UpdateOptionDto> validator)
    {
        if (updatedOption is null)
        {
            return TypedResults.BadRequest(EndpointsConstants.Option.BAD_REQUEST_MESSAGE);
        }

        var validationResult = await validator.ValidateAsync(updatedOption);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return TypedResults.BadRequest(errors);
        }

        var result = await optionRepository.UpdateOptionAsync(id, updatedOption);

        if (!result)
        {
            return TypedResults.NotFound(EndpointsConstants.Option.NOT_FOUND_MESSAGE);
        }

        return TypedResults.NoContent();
    }

    // DELETE /options/{id}
    public static async Task<Results<NoContent, NotFound<string>>> DeleteOptionAsync(
        int id, IOptionRepository optionRepository)
    {
        var result = await optionRepository.DeleteOptionAsync(id);

            if (!result)
            {
                return TypedResults.NotFound(EndpointsConstants.Option.NOT_FOUND_MESSAGE);
            }

            return TypedResults.NoContent();
    }
}
