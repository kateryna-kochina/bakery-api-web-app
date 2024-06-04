using Bakery.Dtos;
using FluentValidation;

namespace Bakery.Helpers;

public class ProductValidator
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(p => p.Title)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(p => p.Price)
                .NotNull()
                .NotEmpty()
                .InclusiveBetween(1, 1000);

            RuleFor(p => p.Image)
                .NotNull()
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(p => p.CategoryId)
                .NotNull()
                .NotEmpty();

            RuleFor(p => p.Description)
                .MaximumLength(1000);
        }
    }
}