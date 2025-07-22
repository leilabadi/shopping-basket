using FluentValidation;
using Shop.ShippingDomain.Application.Dtos;

namespace Shop.ShippingDomain.Application.Validators;

public class ShippingDtoValidator : AbstractValidator<ShippingDto>
{
    public ShippingDtoValidator()
    {
        RuleFor(x => x.Country)
            .NotEmpty()
            .Must(country => !string.IsNullOrWhiteSpace(country));

        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}
