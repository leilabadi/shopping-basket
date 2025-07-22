using Shop.BasketDomain.Domain.Model;

namespace Shop.BasketDomain.Domain.Repositories;

public interface IDiscountCodeRepository
{
    Task SaveDiscountAsync(Discount discount);
    Task<Discount?> GetDiscountAsync(string discountCode);
}
