using Shop.ShippingDomain.Application.Dtos;

namespace Shop.ShippingDomain.Application.Services;

public interface IShippingService
{
    Task SaveShippingAsync(ShippingDto dto);
    Task<ShippingDto?> GetShippingAsync(string country);
    Task<IEnumerable<ShippingDto>> GetAllShippingsAsync();
    Task<bool> DeleteShippingAsync(string country);
}