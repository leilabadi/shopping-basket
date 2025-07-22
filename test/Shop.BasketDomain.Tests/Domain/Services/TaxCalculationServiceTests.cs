using FluentAssertions;
using Shop.BasketDomain.Domain.Services;

namespace Shop.BasketDomain.Tests.Domain.Services;

public class TaxCalculationServiceTests
{
    private readonly TaxCalculationService _sut = new();

    [Fact]
    public void CalculateVAT_WhenCalled_ReturnsCorrectVAT()
    {
        // Arrange
        decimal amount = 100m;

        // Act
        var vat = _sut.CalculateVAT(amount);

        // Assert
        vat.Should().Be(20m);
    }

    [Fact]
    public void AddVAT_WhenCalled_ReturnsAmountIncludingVAT()
    {
        // Arrange
        decimal amount = 100m;

        // Act
        var total = _sut.AddVAT(amount);

        // Assert
        total.Should().Be(120m);
    }

    [Fact]
    public void RemoveVAT_WhenCalled_ReturnsNetAmount()
    {
        // Arrange
        decimal gross = 120m;

        // Act
        var net = _sut.RemoveVAT(gross);

        // Assert
        net.Should().Be(100m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(200)]
    public void CalculateVAT_WithVariousAmounts_ReturnsExpected(decimal amount)
    {
        // Act
        var vat = _sut.CalculateVAT(amount);

        // Assert
        vat.Should().Be(amount * 0.20m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(200)]
    public void AddVAT_WithVariousAmounts_ReturnsExpected(decimal amount)
    {
        // Act
        var total = _sut.AddVAT(amount);

        // Assert
        total.Should().Be(amount * 1.20m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(60)]
    [InlineData(240)]
    public void RemoveVAT_WithVariousGrossAmounts_ReturnsExpected(decimal gross)
    {
        // Act
        var net = _sut.RemoveVAT(gross);

        // Assert
        net.Should().Be(gross / 1.20m);
    }
}
