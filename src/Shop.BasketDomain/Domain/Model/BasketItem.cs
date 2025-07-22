namespace Shop.BasketDomain.Domain.Model;

public class BasketItem
{
    public BasketProduct Product { get; }
    public decimal CalculatedPrice { get; set; }

    public bool DiscountApplied => CalculatedPrice != Product.Price;

    public BasketItem(BasketProduct product)
    {
        Product = product;
        // Apply product-level discount if available
        if (product.DiscountPercentage.HasValue)
        {
            CalculatedPrice = product.Price * (1 - product.DiscountPercentage.Value / 100);
        }
        else
        {
            CalculatedPrice = product.Price;
        }
    }
}