namespace Shop.ProductDomain.Domain.Model;

public class Product
{
    public Guid Id { get; }
    public string Name { get; }
    public decimal Price { get; }
    public decimal? DiscountPercentage { get; }

    private Product(Guid id, string name, decimal price, decimal? discountPercentage = null)
    {
        Id = id;
        Name = name;
        Price = price;
        DiscountPercentage = discountPercentage;
    }

    public static Product Create(string name, decimal price, decimal? discountPercentage = null)
    {
        return new Product(Guid.NewGuid(), name, price, discountPercentage);
    }
}