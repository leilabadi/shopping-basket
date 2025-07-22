using Shop.ShippingDomain.Domain.Models;

namespace Shop.ShippingDomain.Application.Dtos;

public record ShippingDto(string Country, decimal Price)
{
    public ShippingDto(Shipping shipping)
        : this(shipping.Country, shipping.Price)
    {
    }

    public ShippingDto()
        : this(string.Empty, 0)
    {
    }
}
