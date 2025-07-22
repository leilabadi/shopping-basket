using Shop.BasketDomain.Domain.Model;
using Shop.BasketDomain.Domain.Repositories;

namespace Shop.BasketDomain.Infrastructure.Repositories;

public class InMemoryBasketRepository : IBasketRepository
{
    private readonly Dictionary<Guid, Basket> _storedBaskets = [];

    public Task<Guid> SaveBasketAsync(Basket basket)
    {
        _storedBaskets[basket.Id] = basket;
        return Task.FromResult(basket.Id);
    }

    public Task<Basket?> GetBasketAsync(Guid basketId)
    {
        _storedBaskets.TryGetValue(basketId, out var basket);
        return Task.FromResult(basket);
    }

    public Task<bool> UpdateBasketAsync(Guid basketId, Basket basket)
    {
        if (!_storedBaskets.ContainsKey(basketId))
        {
            return Task.FromResult(false);
        }

        _storedBaskets[basketId] = basket;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteBasketAsync(Guid basketId)
    {
        return Task.FromResult(_storedBaskets.Remove(basketId));
    }
}