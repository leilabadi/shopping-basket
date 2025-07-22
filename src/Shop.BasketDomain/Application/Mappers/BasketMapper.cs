using Shop.BasketDomain.Domain.Model;
using Shop.ProductDomain.Application.Dtos;
using Shop.ShippingDomain.Application.Dtos;

namespace Shop.BasketDomain.Application.Mappers;

public static class BasketMapper
{
    public static BasketProduct ToBasketProduct(ProductDto productDto)
    {
        return new BasketProduct(productDto.Id, productDto.Name, productDto.Price, productDto.DiscountPercentage);
    }

    public static BasketShipping ToBasketShipping(ShippingDto shippingDto)
    {
        return new BasketShipping($"{shippingDto.Country} Shipping", shippingDto.Price);
    }
}
