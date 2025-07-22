using Shop.ShippingDomain.Domain.Models;

namespace Shop.ShippingDomain.Domain.Repositories;

public interface IShippingRepository
{
    Task SaveShippingAsync(Shipping shipping);
    Task<Shipping?> GetShippingAsync(string country);
    Task<IEnumerable<Shipping>> GetAllShippingsAsync();
    Task<bool> DeleteShippingAsync(string country);
}
