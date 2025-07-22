using Shop.BasketDomain.Application.Dtos;

namespace Shop.BasketDomain.Application.Services;

public interface IDiscountCodeService
{
    Task AddDiscountCode(AddDiscountDto dto);
    Task<GetDiscountResult?> GetDiscountCode(string discountCode);
}
