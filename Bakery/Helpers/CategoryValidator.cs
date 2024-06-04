using Bakery.Dtos;
using FluentValidation;

namespace Bakery.Helpers;

public class CategoryValidator
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(c => c.CategoryName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(25);
        }
    }

    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(c => c.CategoryName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(25);
        }
    }
}