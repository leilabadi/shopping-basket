using Shop.ProductDomain.Domain.Model;
using Shop.ProductDomain.Domain.Repositories;

namespace Shop.ProductDomain.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _storedProducts = [];

    public Task<Guid> AddProductAsync(Product product)
    {
        _storedProducts[product.Id] = product;
        return Task.FromResult(product.Id);
    }

    public Task<Product?> GetProductAsync(Guid productId)
    {
        _storedProducts.TryGetValue(productId, out var product);
        return Task.FromResult(product);
    }

    public Task<IReadOnlyList<Product>> GetAllProductsAsync()
    {
        var products = _storedProducts.Values.ToList().AsReadOnly();
        return Task.FromResult<IReadOnlyList<Product>>(products);
    }

    public Task<bool> DeleteProductAsync(Guid productId)
    {
        return Task.FromResult(_storedProducts.Remove(productId));
    }
}