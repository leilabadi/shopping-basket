using Shop.BasketDomain.Domain.Model;

namespace Shop.BasketDomain.Application.Dtos;

public record BasketShippingDto(string Name, decimal OriginalPrice, decimal CalculatedPrice)
{
    public BasketShippingDto(BasketShipping basketShipping)
        : this(basketShipping.Name, basketShipping.Price, basketShipping.CalculatedPrice)
    {
    }
}
