using Shop.ProductDomain.Application.Dtos;

namespace Shop.ProductDomain.Application.Services;

public interface IProductService
{
    Task<Guid> CreateProductAsync(CreateProductDto request);
    Task<ProductDto?> GetProductAsync(Guid productId);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<bool> DeleteProductAsync(Guid productId);
}