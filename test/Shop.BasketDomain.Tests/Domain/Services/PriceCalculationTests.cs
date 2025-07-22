using FluentAssertions;
using Shop.BasketDomain.Domain.Factories;
using Shop.BasketDomain.Domain.Model;
using Shop.BasketDomain.Domain.Services;

namespace Shop.BasketDomain.Tests.Domain.Services;

public class PriceCalculationServiceTests
{
    private readonly BasketShipping _shipping = new("Standard Shipping", 0);
    private readonly BasketProduct _productApple = new(Guid.NewGuid(), "Apple", 1.00m);
    private readonly BasketProduct _productBanana = new(Guid.NewGuid(), "Banana", 0.80m);
    private readonly BasketProduct _productCarrot = new(Guid.NewGuid(), "Carrot", 1.15m);
    private readonly PriceCalculationService _sut = new();
    private readonly BasketFactory _basketFactory = new();

    [Fact]
    public void CalculateTotalCost_NoDiscounts_ReturnsSumOfProductPrices()
    {
        // Arrange
        var basket = _basketFactory.CreateBasket(_shipping);
        basket.AddItem(_productApple, 1);
        basket.AddItem(_productBanana, 1);
        basket.AddItem(_productCarrot, 1);

        // Act
        var total = _sut.CalculateTotalCost(basket);

        // Assert
        total.Should().Be(2.95m);
    }

    [Fact]
    public void CalculateTotalCost_BasketDiscount_AppliesPercentageOffToBasketTotal()
    {
        // Arrange
        var basket = _basketFactory.CreateBasket(_shipping, [new Discount("BASKET20", 20)]);
        basket.AddItem(_productApple, 1);
        basket.AddItem(_productBanana, 1);
        basket.AddItem(_productCarrot, 1);

        // Act
        var total = _sut.CalculateTotalCost(basket);

        // Assert
        total.Should().Be(2.36m);
    }

    [Fact]
    public void CalculateTotalCost_MultipleBasketDiscounts_AppliesAllSequentially()
    {
        // Arrange
        var basket = _basketFactory.CreateBasket(_shipping, [new Discount("BASKET10", 10), new Discount("BASKET20", 20)]);
        basket.AddItem(_productApple, 1);
        basket.AddItem(_productBanana, 1);

        // Act
        var total = _sut.CalculateTotalCost(basket);

        // Assert
        total.Should().Be(1.30m);
    }

    [Fact]
    public void CalculateTotalCost_EmptyBasket_ReturnsZero()
    {
        // Arrange
        var basket = _basketFactory.CreateBasket(_shipping, []);

        // Act
        var total = _sut.CalculateTotalCost(basket);

        // Assert
        total.Should().Be(0m);
    }

    [Fact]
    public void CalculateTotalCost_NonZeroShipping_IncludesShippingInTotal()
    {
        // Arrange
        var shippingWithCost = new BasketShipping("Express Shipping", 2.00m, 2.00m);
        var basket = _basketFactory.CreateBasket(shippingWithCost, []);
        basket.AddItem(_productApple, 1);

        // Act
        var total = _sut.CalculateTotalCost(basket);

        // Assert
        total.Should().Be(3.00m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void CalculateTotalCost_WithVariousShippingOnly_ReturnsShipping(decimal shippingCost)
    {
        // Arrange
        var shipping = new BasketShipping("Test Shipping", shippingCost, shippingCost);
        var basket = _basketFactory.CreateBasket(shipping, []);

        // Act
        var total = _sut.CalculateTotalCost(basket);

        // Assert
        total.Should().Be(shippingCost);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(100)]
    public void CalculateTotalCost_BasketDiscountsWithZeroProducts_ReturnsZero(decimal discount)
    {
        // Arrange
        var basket = _basketFactory.CreateBasket(_shipping, [new Discount("BASKET", discount)]);

        // Act
        var total = _sut.CalculateTotalCost(basket);

        // Assert
        total.Should().Be(0m);
    }

    [Fact]
    public void CalculateTotalCost_DiscountOver100Percent_TotalIsZero()
    {
        // Arrange
        var basket = _basketFactory.CreateBasket(_shipping, [new Discount("BASKET200", 200)]);
        basket.AddItem(_productApple, 1);

        // Act
        var total = _sut.CalculateTotalCost(basket);

        // Assert
        total.Should().Be(0m);
    }
}