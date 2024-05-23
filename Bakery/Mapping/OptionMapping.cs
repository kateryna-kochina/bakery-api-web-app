using Bakery.Dtos;
using Bakery.Models;

namespace Bakery.Mapping;

public static class OptionMapping
{
    public static OptionDetailsDto ToOptionDetailsDto(this Option option)
    {
        return new OptionDetailsDto(
            option.Id,
            option.OptionName,
            option.Coefficient
        );
    }

    public static Option ToEntity(this CreateOptionDto createdOption)
    {
        return new Option()
        {
            OptionName = createdOption.OptionName,
            Coefficient = createdOption.Coefficient
        };
    }

    public static Option ToEntity(this UpdateOptionDto updatedOption, int id)
    {
        return new Option()
        {
            Id = id,
            OptionName = updatedOption.OptionName,
            Coefficient = updatedOption.Coefficient
        };
    }

    public static Option ToEntity(this OptionDetailsDto optionDto)
    {
        return new Option()
        {
            OptionName = optionDto.OptionName,
            Coefficient = optionDto.Coefficient
        };
    }
}
