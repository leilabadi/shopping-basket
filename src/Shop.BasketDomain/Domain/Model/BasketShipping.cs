namespace Shop.BasketDomain.Domain.Model;

public record BasketShipping(string Name, decimal Price, decimal CalculatedPrice = 0)
{
}