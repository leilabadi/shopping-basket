using Shop.BasketDomain.Domain.Model;

namespace Shop.BasketDomain.Domain.Services;

public class PriceCalculationService : IPriceCalculationService
{
    public decimal CalculateTotalCost(Basket basket)
    {
        // 1. Use item CalculatedPrice (includes product-level discount)
        var itemsTotal = basket.Items.Sum(item => item.CalculatedPrice);
        decimal subtotal = itemsTotal + basket.Shipping.CalculatedPrice;

        // 2. Apply basket-level percentage discounts sequentially to the running subtotal
        foreach (var discount in basket.Discounts)
        {
            subtotal -= subtotal * (discount.PercentageOff / 100m);
            if (subtotal < 0) subtotal = 0;
        }

        return Math.Max(0, decimal.Round(subtotal, 2)); // Ensure total doesn't go below 0 and round to 2 decimals
    }
}
