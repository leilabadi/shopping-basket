using Shop.ProductDomain.Domain.Model;

namespace Shop.ProductDomain.Application.Dtos;

public record ProductDto(Guid Id, string Name, decimal Price, decimal? DiscountPercentage)
{
    public ProductDto(Product product)
        : this(product.Id, product.Name, product.Price, product.DiscountPercentage)
    {
    }
}