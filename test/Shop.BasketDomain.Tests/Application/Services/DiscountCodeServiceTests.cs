using AutoFixture;
using FluentAssertions;
using Shop.BasketDomain.Application.Dtos;
using Shop.BasketDomain.Application.Services;
using Shop.BasketDomain.Infrastructure.Repositories;

namespace Shop.BasketDomain.Tests.Application.Services;

public class DiscountCodeServiceTests
{
    private readonly InMemoryDiscountCodeRepository _repository = new();
    private readonly DiscountCodeService _sut;
    private readonly Fixture _fixture = new();

    public DiscountCodeServiceTests()
    {
        _sut = new DiscountCodeService(_repository);
    }

    [Fact]
    public async Task AddDiscountCode_WhenCalled_SavesDiscount()
    {
        // Arrange
        var dto = _fixture.Build<AddDiscountDto>()
            .With(x => x.Code, "SAVE10")
            .With(x => x.PercentageOff, 10m)
            .Create();

        // Act
        await _sut.AddDiscountCode(dto);

        // Assert
        var result = await _sut.GetDiscountCode(dto.Code);

        result.Should().NotBeNull();
        result.Code.Should().Be(dto.Code);
        result.PercentageOff.Should().Be(dto.PercentageOff);
    }

    [Fact]
    public async Task GetDiscountCode_WhenDiscountExists_ReturnsDiscount()
    {
        // Arrange
        var dto = _fixture.Build<AddDiscountDto>()
            .With(x => x.Code, "SAVE20")
            .With(x => x.PercentageOff, 20m)
            .Create();

        await _sut.AddDiscountCode(dto);

        // Act
        var result = await _sut.GetDiscountCode(dto.Code);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be(dto.Code);
        result.PercentageOff.Should().Be(dto.PercentageOff);
    }

    [Fact]
    public async Task GetDiscountCode_WhenDiscountDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _sut.GetDiscountCode("NONEXISTENT");

        // Assert
        result.Should().BeNull();
    }
}
