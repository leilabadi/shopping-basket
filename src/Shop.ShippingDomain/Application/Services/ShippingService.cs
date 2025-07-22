using Shop.ShippingDomain.Application.Dtos;
using Shop.ShippingDomain.Domain.Models;
using Shop.ShippingDomain.Domain.Repositories;

namespace Shop.ShippingDomain.Application.Services;

public class ShippingService(IShippingRepository repository) : IShippingService
{
    public Task SaveShippingAsync(ShippingDto dto)
    {
        var shipping = new Shipping(dto.Country, dto.Price);

        return repository.SaveShippingAsync(shipping);
    }

    public async Task<ShippingDto?> GetShippingAsync(string country)
    {
        var shipping = await repository.GetShippingAsync(country);

        return shipping != null
            ? new ShippingDto(shipping)
            : null;
    }

    public async Task<IEnumerable<ShippingDto>> GetAllShippingsAsync()
    {
        var shippings = await repository.GetAllShippingsAsync();

        return shippings.Select(s => new ShippingDto(s));
    }

    public Task<bool> DeleteShippingAsync(string country)
    {
        return repository.DeleteShippingAsync(country);
    }
}