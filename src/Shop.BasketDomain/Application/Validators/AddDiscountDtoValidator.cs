using FluentValidation;
using Shop.BasketDomain.Application.Dtos;

namespace Shop.BasketDomain.Application.Validators;

public class AddDiscountDtoValidator : AbstractValidator<AddDiscountDto>
{
    public AddDiscountDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .Must(name => !string.IsNullOrWhiteSpace(name));

        RuleFor(x => x.PercentageOff)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}
