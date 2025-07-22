namespace Shop.BasketDomain.Domain.Model;

public class BasketProduct(Guid id, string name, decimal price, decimal? discountPercentage = null)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public decimal Price { get; } = price;
    public decimal? DiscountPercentage { get; } = discountPercentage;
}