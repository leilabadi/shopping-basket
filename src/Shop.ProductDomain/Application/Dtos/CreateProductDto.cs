namespace Shop.ProductDomain.Application.Dtos;

public record CreateProductDto(string Name, decimal Price, decimal? DiscountPercentage = null)
{
}