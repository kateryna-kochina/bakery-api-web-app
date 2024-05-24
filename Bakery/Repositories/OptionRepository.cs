using Bakery.Data;
using Bakery.Dtos;
using Bakery.Mapping;
using Bakery.Models;
using Bakery.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Repositories;

public class OptionRepository : IOptionRepository
{
    private readonly BakeryDbContext _dbContext;

    public OptionRepository(BakeryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Option>> GetOptionsAsync()
    {
        var options = await _dbContext.Options.ToListAsync();

        return options;
    }

    public async Task<Option?> GetOptionByIdAsync(int id)
    {
        var option = await _dbContext.Options
                .FirstOrDefaultAsync(o => o.Id == id);

        if (option is null)
        {
            return null;
        }

        return option;
    }

    public async Task<Option?> CreateOptionAsync(CreateOptionDto newOption)
    {
        var option = newOption.ToEntity();

        _dbContext.Options.Add(option);
        await _dbContext.SaveChangesAsync();

        return option;
    }

    public async Task<bool> UpdateOptionAsync(int id, UpdateOptionDto updatedOption)
    {
        var existingOption = await _dbContext.Options.FindAsync(id);

        if (existingOption is null)
        {
            return false;
        }

        _dbContext.Entry(existingOption)
            .CurrentValues
            .SetValues(updatedOption.ToEntity(id));
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteOptionAsync(int id)
    {
        var existingOption = await _dbContext.Options.FindAsync(id);

        if (existingOption is null)
        {
            return false;
        }

        await _dbContext.Options
            .Where(o => o.Id == id)
            .ExecuteDeleteAsync();

        return true;
    }
}
