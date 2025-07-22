using Shop.BasketDomain.Application.Dtos;
using Shop.BasketDomain.Domain.Model;

namespace Shop.BasketDomain.Application.Factories;

public static class BasketDtoFactory
{
    public static BasketDto Create(Basket basket, decimal subtotalWithoutVat, decimal vat, decimal totalWithVat)
    {
        return new BasketDto
        (
            basket.Id,
            basket.Items.Select(item => new BasketItemDto(item)).ToList(),
            new BasketShippingDto(basket.Shipping),
            basket.Discounts.Select((d, i) => $"Discount {i + 1}").ToList(),
            subtotalWithoutVat,
            vat,
            totalWithVat
        );
    }
}
