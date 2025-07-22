using Shop.BasketDomain.Domain.Model;

namespace Shop.BasketDomain.Application.Dtos;

public record BasketItemDto(string Name, decimal OriginalPrice, decimal CalculatedPrice, bool DiscountApplied)
{
    public BasketItemDto(BasketItem item)
        : this(item.Product.Name, item.Product.Price, item.CalculatedPrice, item.DiscountApplied)
    {
    }
}
