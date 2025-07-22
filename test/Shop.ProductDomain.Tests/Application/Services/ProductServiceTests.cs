using AutoFixture;
using FluentAssertions;
using Shop.ProductDomain.Application.Dtos;
using Shop.ProductDomain.Application.Services;
using Shop.ProductDomain.Infrastructure.Repositories;

namespace Shop.ProductDomain.Tests.Application.Services;

public class ProductServiceTests
{
    private readonly InMemoryProductRepository _repository = new();
    private readonly ProductService _sut;
    private readonly Fixture _fixture = new();

    public ProductServiceTests()
    {
        _sut = new ProductService(_repository);
    }

    [Fact]
    public async Task CreateProductAsync_WhenCalled_SavesProductAndReturnsId()
    {
        // Arrange
        var createDto = _fixture.Create<CreateProductDto>();

        // Act
        var productId = await _sut.CreateProductAsync(createDto);

        // Assert
        var product = await _sut.GetProductAsync(productId);

        product.Should().NotBeNull();
        product.Name.Should().Be(createDto.Name);
        product.Price.Should().Be(createDto.Price);
        product.DiscountPercentage.Should().Be(createDto.DiscountPercentage);
    }

    [Fact]
    public async Task GetProductAsync_WhenProductExists_ReturnsProductDto()
    {
        // Arrange
        var createDto = _fixture.Create<CreateProductDto>();
        var productId = await _sut.CreateProductAsync(createDto);

        // Act
        var product = await _sut.GetProductAsync(productId);

        // Assert
        product.Should().NotBeNull();
        product.Id.Should().Be(productId);
        product.Name.Should().Be(createDto.Name);
        product.Price.Should().Be(createDto.Price);
        product.DiscountPercentage.Should().Be(createDto.DiscountPercentage);
    }

    [Fact]
    public async Task GetProductAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Act
        var product = await _sut.GetProductAsync(Guid.NewGuid());

        // Assert
        product.Should().BeNull();
    }

    [Fact]
    public async Task GetAllProductsAsync_WhenMultipleProductsSaved_ReturnsAllProducts()
    {
        // Arrange
        var createDtos = _fixture.CreateMany<CreateProductDto>(3).ToList();
        var ids = new List<Guid>();
        foreach (var dto in createDtos)
        {
            ids.Add(await _sut.CreateProductAsync(dto));
        }

        // Act
        var all = await _sut.GetAllProductsAsync();

        // Assert
        all.Should().NotBeNull();
        all.Should().HaveCount(3);
        foreach (var (dto, id) in createDtos.Zip(ids))
        {
            all.Should().Contain(x => x.Id == id && x.Name == dto.Name && x.Price == dto.Price && x.DiscountPercentage == dto.DiscountPercentage);
        }
    }

    [Fact]
    public async Task DeleteProductAsync_WhenProductExists_RemovesProduct()
    {
        // Arrange
        var createDto = _fixture.Create<CreateProductDto>();
        var productId = await _sut.CreateProductAsync(createDto);

        // Act
        var deleted = await _sut.DeleteProductAsync(productId);

        // Assert
        deleted.Should().BeTrue();

        var product = await _sut.GetProductAsync(productId);
        product.Should().BeNull();
    }

    [Fact]
    public async Task DeleteProductAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        // Act
        var deleted = await _sut.DeleteProductAsync(Guid.NewGuid());

        // Assert
        deleted.Should().BeFalse();
    }
}
