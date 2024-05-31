using AutoMapper;
using Bakery.Dtos;
using Bakery.Repositories.Contracts;

namespace Bakery.Endpoints;

public static class OptionsEndpoints
{
    public static RouteGroupBuilder MapOptionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("options").WithTags("Options");

        // GET /options
        group.MapGet("/", async (IOptionRepository optionRepository, IMapper mapper) =>
        {
            var options = await optionRepository.GetOptionsAsync();

            var optionsDtos = mapper.Map<List<OptionDetailsDto>>(options);

            return Results.Ok(optionsDtos);
        })
        .WithName("GetOptions")
        .Produces<List<OptionDetailsDto>>(StatusCodes.Status200OK);

        // GET /options/{id}
        group.MapGet("/{id}", async (int id, IOptionRepository optionRepository, IMapper mapper) =>
        {
            var option = await optionRepository.GetOptionByIdAsync(id);

            if (option is null)
            {
                return Results.NotFound();
            }

            var optionDto = mapper.Map<OptionDetailsDto>(option);

            return Results.Ok(optionDto);
        })
        .WithName("GetOptionById")
        .Produces<OptionDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /options
        group.MapPost("/", async (CreateOptionDto newOption, IOptionRepository optionRepository, IMapper mapper) =>
        {
            var createdOption = await optionRepository.CreateOptionAsync(newOption);

            var createdOptionDetailsDto = mapper.Map<OptionDetailsDto>(createdOption);

            return Results.Created(
                $"/options/{createdOption!.Id}",
                createdOptionDetailsDto);
        })
        .WithName("CreateOption")
        .Produces<OptionDetailsDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // PUT /options/{id}
        group.MapPut("/{id}", async (int id, UpdateOptionDto updatedOption, IOptionRepository optionRepository) =>
        {
            var result = await optionRepository.UpdateOptionAsync(id, updatedOption);

            if (!result)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .WithName("UpdateOption")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /options/{id}
        group.MapDelete("/{id}", async (int id, IOptionRepository optionRepository) =>
        {
            var result = await optionRepository.DeleteOptionAsync(id);

            if (!result)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .WithName("DeleteOption")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
