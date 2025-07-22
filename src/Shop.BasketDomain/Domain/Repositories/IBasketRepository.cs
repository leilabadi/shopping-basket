using Shop.BasketDomain.Domain.Model;

namespace Shop.BasketDomain.Domain.Repositories;

public interface IBasketRepository
{
    Task<Guid> SaveBasketAsync(Basket basket);
    Task<Basket?> GetBasketAsync(Guid basketId);
    Task<bool> UpdateBasketAsync(Guid basketId, Basket basket);
    Task<bool> DeleteBasketAsync(Guid basketId);
}