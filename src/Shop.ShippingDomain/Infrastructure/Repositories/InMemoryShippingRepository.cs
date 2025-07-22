using Shop.ShippingDomain.Domain.Models;
using Shop.ShippingDomain.Domain.Repositories;

namespace Shop.ShippingDomain.Infrastructure.Repositories;

public class InMemoryShippingRepository : IShippingRepository
{
    private readonly Dictionary<string, Shipping> _storedShippings = [];

    public Task SaveShippingAsync(Shipping shipping)
    {
        _storedShippings[shipping.Country.ToUpper()] = shipping;
        return Task.CompletedTask;
    }

    public Task<Shipping?> GetShippingAsync(string country)
    {
        _storedShippings.TryGetValue(country.ToUpper(), out var shipping);
        return Task.FromResult(shipping);
    }

    public Task<IEnumerable<Shipping>> GetAllShippingsAsync()
    {
        return Task.FromResult(_storedShippings.Values.AsEnumerable());
    }

    public Task<bool> DeleteShippingAsync(string country)
    {
        return Task.FromResult(_storedShippings.Remove(country.ToUpper()));
    }
}
