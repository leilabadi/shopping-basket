using Shop.ProductDomain.Application.Dtos;
using Shop.ProductDomain.Domain.Model;
using Shop.ProductDomain.Domain.Repositories;

namespace Shop.ProductDomain.Application.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<Guid> CreateProductAsync(CreateProductDto request)
    {
        var product = Product.Create(request.Name, request.Price, request.DiscountPercentage);

        return await productRepository.AddProductAsync(product);
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        var product = await productRepository.GetProductAsync(productId);

        return product != null
            ? new ProductDto(product)
            : null;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await productRepository.GetAllProductsAsync();

        return products.Select(p => new ProductDto(p));
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        return await productRepository.DeleteProductAsync(productId);
    }
}