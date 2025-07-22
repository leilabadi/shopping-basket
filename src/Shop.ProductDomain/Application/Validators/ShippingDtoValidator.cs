using FluentValidation;
using Shop.ProductDomain.Application.Dtos;

namespace Shop.ProductDomain.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Must(name => !string.IsNullOrWhiteSpace(name));

        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);

        RuleFor(x => x.DiscountPercentage)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .When(x => x.DiscountPercentage.HasValue);
    }
}
