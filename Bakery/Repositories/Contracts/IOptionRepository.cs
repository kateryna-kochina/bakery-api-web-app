using Bakery.Dtos;
using Bakery.Models;

namespace Bakery.Repositories.Contracts;

public interface IOptionRepository
{
    Task<List<Option>> GetOptionsAsync();
    Task<Option?> GetOptionByIdAsync(int id);
    Task<Option?> CreateOptionAsync(CreateOptionDto newOption);
    Task<bool> UpdateOptionAsync(int id, UpdateOptionDto updatedOption);
    Task<bool> DeleteOptionAsync(int id);
}
