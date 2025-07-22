using FluentAssertions;
using Moq;
using Shop.BasketDomain.Application.Dtos;
using Shop.BasketDomain.Application.Services;
using Shop.BasketDomain.Domain.Factories;
using Shop.BasketDomain.Domain.Model;
using Shop.BasketDomain.Domain.Repositories;
using Shop.BasketDomain.Domain.Services;
using Shop.ProductDomain.Application.Dtos;
using Shop.ProductDomain.Application.Services;
using Shop.ShippingDomain.Application.Dtos;
using Shop.ShippingDomain.Application.Services;
using Shop.ShippingDomain.Domain.Models;

namespace Shop.BasketDomain.Tests.Application.Services;

public class BasketServiceTests
{
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IProductService> _productServiceMock;
    private readonly Mock<IPriceCalculationService> _priceCalculationServiceMock;
    private readonly Mock<ITaxCalculationService> _taxCalculationServiceMock;
    private readonly Mock<IShippingService> _shippingServiceMock;
    private readonly Mock<IDiscountCodeService> _discountCodeServiceMock;
    private readonly BasketFactory _basketFactory;
    private readonly BasketService _basketService;
    private readonly BasketProduct _productBread;
    private readonly BasketProduct _productButter;
    private readonly Shipping _defaultShipping;
    private readonly Discount _testDiscount;

    public BasketServiceTests()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _productServiceMock = new Mock<IProductService>();
        _priceCalculationServiceMock = new Mock<IPriceCalculationService>();
        _taxCalculationServiceMock = new Mock<ITaxCalculationService>();
        _shippingServiceMock = new Mock<IShippingService>();
        _discountCodeServiceMock = new Mock<IDiscountCodeService>();
        _basketFactory = new BasketFactory();

        _basketService = new BasketService(
            _basketRepositoryMock.Object,
            _productServiceMock.Object,
            _priceCalculationServiceMock.Object,
            _taxCalculationServiceMock.Object,
            _shippingServiceMock.Object,
            _discountCodeServiceMock.Object,
            _basketFactory);

        _productBread = new BasketProduct(Guid.NewGuid(), "Bread", 1.00m);
        _productButter = new BasketProduct(Guid.NewGuid(), "Butter", 0.80m);
        _defaultShipping = new Shipping("UK", 2.50m);
        _testDiscount = new Discount("TEST10", 10);
    }


    [Fact]
    public async Task CreateBasketAsync_ShouldCreateBasketWithDefaultUKShipping()
    {
        // Arrange
        var basketId = Guid.NewGuid();

        var shippingDto = new ShippingDto(_defaultShipping);

        _shippingServiceMock.Setup(s => s.GetShippingAsync("UK")).ReturnsAsync(shippingDto);
        _basketRepositoryMock.Setup(r => r.SaveBasketAsync(It.IsAny<Basket>())).ReturnsAsync(basketId);

        // Act
        var result = await _basketService.CreateBasketAsync();

        // Assert
        result.Should().Be(basketId);
        _shippingServiceMock.Verify(s => s.GetShippingAsync("UK"), Times.Once);
        _basketRepositoryMock.Verify(r => r.SaveBasketAsync(It.IsAny<Basket>()), Times.Once);
    }


    [Fact]
    public async Task GetBasketAsync_BasketExists_ShouldReturnBasketDto()
    {
        // Arrange
        var basketShipping = new BasketShipping("UK", 2.50m);
        var basket = _basketFactory.CreateBasket(basketShipping);
        basket.AddItem(_productBread);
        basket.AddItem(_productButter);
        basket.AddDiscount(_testDiscount);

        _basketRepositoryMock.Setup(r => r.GetBasketAsync(basket.Id)).ReturnsAsync(basket);
        _priceCalculationServiceMock.Setup(p => p.CalculateTotalCost(basket)).Returns(4.30m);
        _taxCalculationServiceMock.Setup(t => t.CalculateVAT(4.30m)).Returns(0.86m);
        _taxCalculationServiceMock.Setup(t => t.AddVAT(4.30m)).Returns(5.16m);

        // Act
        var result = await _basketService.GetBasketAsync(basket.Id);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.Dto.Should().NotBeNull();
        result.Dto.BasketId.Should().Be(basket.Id);
        result.Dto.Items.Should().HaveCount(2);
        result.Dto.Items[0].Name.Should().Be("Bread");
        result.Dto.Items[1].Name.Should().Be("Butter");
        result.Dto.Shipping.Name.Should().Be("UK");
        result.Dto.AppliedDiscounts.Should().HaveCount(1);
        result.Dto.SubtotalWithoutVat.Should().Be(4.30m);
        result.Dto.VAT.Should().Be(0.86m);
        result.Dto.TotalWithVat.Should().Be(5.16m);
    }

    [Fact]
    public async Task GetBasketAsync_BasketDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        _basketRepositoryMock.Setup(r => r.GetBasketAsync(basketId)).ReturnsAsync((Basket?)null);

        // Act
        var result = await _basketService.GetBasketAsync(basketId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.Dto.Should().BeNull();
    }


    [Fact]
    public async Task AddItemAsync_BasketAndProductExist_ShouldAddItemAndReturnTrue()
    {
        // Arrange
        var basketShipping = new BasketShipping("UK", 2.50m);

        var basketId = Guid.NewGuid();
        var basket = _basketFactory.CreateBasket(basketShipping);
        var request = new AddItemDto(_productBread.Id, 2);

        var productDto = new ProductDto(_productBread.Id, _productBread.Name, _productBread.Price, _productBread.DiscountPercentage);

        _basketRepositoryMock.Setup(r => r.GetBasketAsync(basketId)).ReturnsAsync(basket);
        _productServiceMock.Setup(p => p.GetProductAsync(request.ProductId)).ReturnsAsync(productDto);
        _basketRepositoryMock.Setup(r => r.UpdateBasketAsync(basketId, basket)).ReturnsAsync(true);

        // Act
        var result = await _basketService.AddItemAsync(basketId, request);

        // Assert
        result.Should().Be(AddItemResult.Success);
        basket.Items.Should().HaveCount(2);
        _basketRepositoryMock.Verify(r => r.UpdateBasketAsync(basketId, basket), Times.Once);
    }

    [Fact]
    public async Task AddItemAsync_BasketDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var request = new AddItemDto(_productBread.Id, 1);
        _basketRepositoryMock.Setup(r => r.GetBasketAsync(basketId)).ReturnsAsync((Basket?)null);

        // Act
        var result = await _basketService.AddItemAsync(basketId, request);

        // Assert
        result.Should().Be(AddItemResult.BasketNotFound);
    }

    [Fact]
    public async Task AddItemAsync_ProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var basketShipping = new BasketShipping("UK", 2.50m);

        var basketId = Guid.NewGuid();
        var basket = _basketFactory.CreateBasket(basketShipping);
        var request = new AddItemDto(Guid.NewGuid(), 1);

        _basketRepositoryMock.Setup(r => r.GetBasketAsync(basketId)).ReturnsAsync(basket);
        _productServiceMock.Setup(p => p.GetProductAsync(request.ProductId)).ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _basketService.AddItemAsync(basketId, request);

        // Assert
        result.Should().Be(AddItemResult.ProductNotFound);
    }


    [Fact]
    public async Task RemoveItemAsync_BasketAndProductExistAndItemRemoved_ShouldReturnTrue()
    {
        // Arrange
        var basketShipping = new BasketShipping("UK", 2.50m);

        var basketId = Guid.NewGuid();
        var basket = _basketFactory.CreateBasket(basketShipping);
        basket.AddItem(_productBread);

        var productDto = new ProductDto(_productBread.Id, _productBread.Name, _productBread.Price, _productBread.DiscountPercentage);

        _basketRepositoryMock.Setup(r => r.GetBasketAsync(basketId)).ReturnsAsync(basket);
        _productServiceMock.Setup(p => p.GetProductAsync(_productBread.Id)).ReturnsAsync(productDto);
        _basketRepositoryMock.Setup(r => r.UpdateBasketAsync(basketId, basket)).ReturnsAsync(true);

        // Act
        var result = await _basketService.RemoveItemAsync(basketId, _productBread.Id);

        // Assert
        result.Should().Be(RemoveItemResult.Success);
        basket.Items.Should().HaveCount(0);
        _basketRepositoryMock.Verify(r => r.UpdateBasketAsync(basketId, basket), Times.Once);
    }

    [Fact]
    public async Task RemoveItemAsync_ItemNotInBasket_ShouldReturnFalse()
    {
        // Arrange
        var basketShipping = new BasketShipping("UK", 2.50m);

        var basketId = Guid.NewGuid();
        var basket = _basketFactory.CreateBasket(basketShipping);

        var productDto = new ProductDto(_productBread.Id, _productBread.Name, _productBread.Price, _productBread.DiscountPercentage);

        _basketRepositoryMock.Setup(r => r.GetBasketAsync(basketId)).ReturnsAsync(basket);
        _productServiceMock.Setup(p => p.GetProductAsync(_productBread.Id)).ReturnsAsync(productDto);

        // Act
        var result = await _basketService.RemoveItemAsync(basketId, Guid.NewGuid());

        // Assert
        result.Should().Be(RemoveItemResult.ItemNotInBasket);
        _basketRepositoryMock.Verify(r => r.UpdateBasketAsync(It.IsAny<Guid>(), It.IsAny<Basket>()), Times.Never);
    }


    [Fact]
    public async Task AddDiscountCodeAsync_ValidDiscountCode_ShouldAddDiscountAndReturnTrue()
    {
        // Arrange
        var basketShipping = new BasketShipping("UK", 2.50m);

        var basketId = Guid.NewGuid();
        var basket = _basketFactory.CreateBasket(basketShipping);
        var discountCode = "TEST10";

        var discountResult = new GetDiscountResult(discountCode, 10);

        _basketRepositoryMock.Setup(r => r.GetBasketAsync(basketId)).ReturnsAsync(basket);
        _discountCodeServiceMock.Setup(d => d.GetDiscountCode("TEST10")).ReturnsAsync(discountResult);
        _basketRepositoryMock.Setup(r => r.UpdateBasketAsync(basketId, basket)).ReturnsAsync(true);

        // Act
        var result = await _basketService.AddDiscountAsync(basketId, discountCode);

        // Assert
        result.Should().Be(AddDiscountResult.Success);

        var fetechedBasket = await _basketService.GetBasketAsync(basketId);
        fetechedBasket.IsSuccessful.Should().BeTrue();
        fetechedBasket.Dto.Should().NotBeNull();
        fetechedBasket.Dto.AppliedDiscounts.Should().HaveCount(1);

        var discount = fetechedBasket.Dto.AppliedDiscounts[0];

        _basketRepositoryMock.Verify(r => r.UpdateBasketAsync(basketId, basket), Times.Once);
    }

    [Fact]
    public async Task AddDiscountCodeAsync_InvalidDiscountCode_ShouldReturnFalse()
    {
        // Arrange
        var basketShipping = new BasketShipping("UK", 2.50m);

        var basketId = Guid.NewGuid();
        var basket = _basketFactory.CreateBasket(basketShipping);
        var discountCode = "INVALID";

        _basketRepositoryMock.Setup(r => r.GetBasketAsync(basketId)).ReturnsAsync(basket);
        _discountCodeServiceMock.Setup(d => d.GetDiscountCode("INVALID")).ReturnsAsync((GetDiscountResult?)null);

        // Act
        var result = await _basketService.AddDiscountAsync(basketId, discountCode);

        // Assert
        result.Should().Be(AddDiscountResult.InvalidDiscountCode);
    }


    [Fact]
    public async Task SetShippingAsync_ValidCountry_ShouldSetShippingAndReturnTrue()
    {
        // Arrange
        var basketShipping = new BasketShipping("UK Shipping", 2.50m);
        var basketId = Guid.NewGuid();
        var basket = _basketFactory.CreateBasket(basketShipping);
        var frenchShipping = new BasketShipping("France Shipping", 5.00m);
        var shippingDto = new ShippingDto("France", frenchShipping.Price);

        _basketRepositoryMock.Setup(r => r.GetBasketAsync(basketId)).ReturnsAsync(basket);
        _shippingServiceMock.Setup(s => s.GetShippingAsync("France")).ReturnsAsync(shippingDto);
        _basketRepositoryMock.Setup(r => r.UpdateBasketAsync(basketId, basket)).ReturnsAsync(true);

        // Act
        var result = await _basketService.SetShippingCountryAsync(basketId, "France");

        // Assert
        result.Should().Be(SetShippingCountryResult.Success);
        _basketRepositoryMock.Verify(r => r.UpdateBasketAsync(basketId, basket), Times.Once);
        basket.Shipping.Name.Should().Be("France Shipping");
    }


    [Fact]
    public async Task DeleteBasketAsync_ShouldCallRepositoryDelete()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        _basketRepositoryMock.Setup(r => r.DeleteBasketAsync(basketId)).ReturnsAsync(true);

        // Act
        var result = await _basketService.DeleteBasketAsync(basketId);

        // Assert
        result.Should().BeTrue();
        _basketRepositoryMock.Verify(r => r.DeleteBasketAsync(basketId), Times.Once);

        var basket = await _basketService.GetBasketAsync(basketId);
        basket.IsSuccessful.Should().BeFalse();
        basket.Dto.Should().BeNull();
    }
}