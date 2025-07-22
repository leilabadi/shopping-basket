using Shop.BasketDomain.Domain.Model;
using Shop.BasketDomain.Domain.Repositories;
using System.Collections.Concurrent;

namespace Shop.BasketDomain.Infrastructure.Repositories;

public class InMemoryDiscountCodeRepository : IDiscountCodeRepository
{
    private readonly ConcurrentDictionary<string, Discount> _storedDiscountCodes = [];

    public Task SaveDiscountAsync(Discount discount)
    {
        _storedDiscountCodes[discount.Code.ToUpper()] = discount;
        return Task.CompletedTask;
    }

    public Task<Discount?> GetDiscountAsync(string discountCode)
    {
        _storedDiscountCodes.TryGetValue(discountCode.ToUpper(), out var discount);
        return Task.FromResult(discount);
    }
}
