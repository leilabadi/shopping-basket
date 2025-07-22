using Shop.ProductDomain.Domain.Model;

namespace Shop.ProductDomain.Domain.Repositories;

public interface IProductRepository
{
    Task<Guid> AddProductAsync(Product product);
    Task<Product?> GetProductAsync(Guid productId);
    Task<IReadOnlyList<Product>> GetAllProductsAsync();
    Task<bool> DeleteProductAsync(Guid productId);
}