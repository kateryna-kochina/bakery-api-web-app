using Bakery.Dtos;
using Bakery.Mapping;
using Bakery.Repositories.Contracts;

namespace Bakery.Endpoints;

public static class OptionsEndpoints
{
    public static RouteGroupBuilder MapOptionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("options").WithTags("Options");

        // GET /options
        group.MapGet("/", async (IOptionRepository optionRepository) =>
        {
            var options = await optionRepository.GetOptionsAsync();

            var optionsDtos = options
                .Select(o => o.ToOptionDetailsDto())
                .ToList();

            return Results.Ok(optionsDtos);
        })
        .WithName("GetOptions")
        .Produces<List<OptionDetailsDto>>(StatusCodes.Status200OK);

        // GET /options/{id}
        group.MapGet("/{id}", async (int id, IOptionRepository optionRepository) =>
        {
            var option = await optionRepository.GetOptionByIdAsync(id);

            if (option is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(option.ToOptionDetailsDto());
        })
        .WithName("GetOptionById")
        .Produces<OptionDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /options
        group.MapPost("/", async (CreateOptionDto newOption, IOptionRepository optionRepository) =>
        {
            var createdOption = await optionRepository.CreateOptionAsync(newOption);

            return Results.Created(
                $"/options/{createdOption!.Id}",
                createdOption.ToOptionDetailsDto());
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
