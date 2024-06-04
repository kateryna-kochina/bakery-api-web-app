using Bakery.Dtos;
using FluentValidation;

namespace Bakery.Helpers;

public class OptionValidator
{
    public class CreateOptionDtoValidator : AbstractValidator<CreateOptionDto>
    {
        public CreateOptionDtoValidator()
        {
            RuleFor(o => o.OptionName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(25);

            RuleFor(o => o.Coefficient)
                .NotNull();
        }
    }

    public class UpdateOptionDtoValidator : AbstractValidator<UpdateOptionDto>
    {
        public UpdateOptionDtoValidator()
        {
            RuleFor(o => o.OptionName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(25);

            RuleFor(o => o.Coefficient)
                .NotNull();
        }
    }
}