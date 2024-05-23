using Bakery.Data;
using Bakery.Dtos;
using Bakery.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Endpoints;

public static class OptionsEndpoints
{
    public static RouteGroupBuilder MapOptionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("options");

        // GET /options
        group.MapGet("/", async (BakeryDbContext dbContext) =>
        {
            var options = await dbContext.Options.ToListAsync();

            var optionsDtos = options
                .Select(o => o.ToOptionSummaryDto())
                .ToList();

            return Results.Ok(optionsDtos);
        });

        // GET /options/{id}
        group.MapGet("/{id}", async (int id, BakeryDbContext dbContext) =>
        {
            var option = await dbContext.Options
                .FirstOrDefaultAsync(o => o.Id == id);

            if (option is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(option.ToOptionSummaryDto());
        });

        // POST /options
        group.MapPost("/", async (CreateOptionDto newOption, BakeryDbContext dbContext) =>
        {
            var option = newOption.ToEntity();

            dbContext.Options.Add(option);
            await dbContext.SaveChangesAsync();

            var createdOption = await dbContext.Options
                .FirstAsync(o => o.Id == option.Id);

            return Results.Created(
                $"/products/{createdOption.Id}",
                createdOption.ToOptionSummaryDto());
        });

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
        });

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
        });

        return group;
    }
}