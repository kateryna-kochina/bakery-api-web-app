using AutoMapper;
using Bakery.Data;
using Bakery.Dtos;
using Bakery.Models;
using Bakery.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Repositories;

public class OptionRepository : IOptionRepository
{
    private readonly BakeryDbContext _dbContext;
    private readonly IMapper _mapper;

    public OptionRepository(BakeryDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
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
        var option = _mapper.Map<Option>(newOption);

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

        var updatedOptionDto = _mapper.Map(updatedOption, existingOption);

        _dbContext.Entry(existingOption)
            .CurrentValues
            .SetValues(updatedOptionDto);
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
