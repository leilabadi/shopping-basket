using AutoFixture;
using FluentAssertions;
using Shop.ShippingDomain.Application.Dtos;
using Shop.ShippingDomain.Application.Services;
using Shop.ShippingDomain.Infrastructure.Repositories;

namespace Shop.ShippingDomain.Tests.Application.Services;

public class ShippingServiceTests
{
    private readonly InMemoryShippingRepository _repository = new();
    private readonly ShippingService _sut;
    private readonly Fixture _fixture = new();

    public ShippingServiceTests()
    {
        _sut = new ShippingService(_repository);
    }

    [Fact]
    public async Task SaveShippingAsync_WhenCalled_SavesShipping()
    {
        // Arrange
        var shipping = _fixture.Create<ShippingDto>();

        // Act
        await _sut.SaveShippingAsync(shipping);

        // Assert
        var result = await _sut.GetShippingAsync(shipping.Country);

        result.Should().NotBeNull();
        result.Country.Should().Be(shipping.Country);
        result.Price.Should().Be(shipping.Price);
    }

    [Fact]
    public async Task GetShippingAsync_WhenPassingCountryNameWithDifferentCase_ReturnsCaseInsensitiveMatch()
    {
        // Arrange
        var shipping = _fixture.Build<ShippingDto>()
            .With(x => x.Country, "US")
            .Create();

        await _sut.SaveShippingAsync(shipping);

        // Act
        var result = await _sut.GetShippingAsync(shipping.Country.ToLowerInvariant());

        // Assert
        result.Should().NotBeNull();
        result.Country.Should().Be(shipping.Country);
        result.Price.Should().Be(shipping.Price);
    }

    [Fact]
    public async Task GetShippingAsync_WhenCountryIsUnknown_ReturnsNull()
    {
        // Act
        var result = await _sut.GetShippingAsync(_fixture.Create<string>());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllShippingsAsync_WhenMultipleShippingsSaved_ReturnsAllShippings()
    {
        // Arrange
        var shippings = _fixture.CreateMany<ShippingDto>(3).ToList();
        var shipping1 = shippings[0];
        var shipping2 = shippings[1];
        var shipping3 = shippings[2];

        await _sut.SaveShippingAsync(shipping1);
        await _sut.SaveShippingAsync(shipping2);
        await _sut.SaveShippingAsync(shipping3);

        // Act
        var all = await _sut.GetAllShippingsAsync();

        // Assert
        all.Should().NotBeNull();
        all.Should().HaveCount(3);
        all.Should().Contain(x => x.Country == shipping1.Country && x.Price == shipping1.Price);
        all.Should().Contain(x => x.Country == shipping2.Country && x.Price == shipping2.Price);
        all.Should().Contain(x => x.Country == shipping3.Country && x.Price == shipping3.Price);
    }

    [Fact]
    public async Task DeleteShippingAsync_WhenShippingExists_RemovesShipping()
    {
        // Arrange
        var shipping = _fixture.Create<ShippingDto>();
        await _sut.SaveShippingAsync(shipping);

        // Act
        var deleted = await _sut.DeleteShippingAsync(shipping.Country);

        // Assert
        deleted.Should().BeTrue();

        var result = await _sut.GetShippingAsync(shipping.Country);
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteShippingAsync_WhenCountryIsUnknown_ReturnsFalse()
    {
        // Act
        var deleted = await _sut.DeleteShippingAsync(_fixture.Create<string>());

        // Assert
        deleted.Should().BeFalse();
    }
}