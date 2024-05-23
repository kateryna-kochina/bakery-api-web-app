using Bakery.Data;
using Bakery.Dtos;
using Bakery.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Endpoints;

public static class OptionsEndpoints
{
    public static RouteGroupBuilder MapOptionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("options").WithTags("Options");

        // GET /options
        group.MapGet("/", async (BakeryDbContext dbContext) =>
        {
            var options = await dbContext.Options.ToListAsync();

            var optionsDtos = options
                .Select(o => o.ToOptionDetailsDto())
                .ToList();

            return Results.Ok(optionsDtos);
        })
        .WithName("GetOptions")
        .Produces<List<OptionDetailsDto>>(StatusCodes.Status200OK);

        // GET /options/{id}
        group.MapGet("/{id}", async (int id, BakeryDbContext dbContext) =>
        {
            var option = await dbContext.Options
                .FirstOrDefaultAsync(o => o.Id == id);

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
        group.MapPost("/", async (CreateOptionDto newOption, BakeryDbContext dbContext) =>
        {
            var option = newOption.ToEntity();

            dbContext.Options.Add(option);
            await dbContext.SaveChangesAsync();

            var createdOption = await dbContext.Options
                .FirstAsync(o => o.Id == option.Id);

            return Results.Created(
                $"/options/{createdOption.Id}",
                createdOption.ToOptionDetailsDto());
        })
        .WithName("CreateOption")
        .Produces<OptionDetailsDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // PUT /options/{id}
        group.MapPut("/{id}", async (int id, UpdateOptionDto updatedOption, BakeryDbContext dbContext) =>
        {
            var existingOption = await dbContext.Options.FindAsync(id);

            if (existingOption is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingOption)
                .CurrentValues
                .SetValues(updatedOption.ToEntity(id));
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateOption")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /options/{id}
        group.MapDelete("/{id}", async (int id, BakeryDbContext dbContext) =>
        {
            var existingOption = await dbContext.Options.FindAsync(id);

            if (existingOption is null)
            {
                return Results.NotFound();
            }

            await dbContext.Options
                .Where(o => o.Id == id)
                .ExecuteDeleteAsync();

            return Results.NoContent();
        })
        .WithName("DeleteOption")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
