using Shop.BasketDomain.Application.Dtos;
using Shop.BasketDomain.Domain.Model;
using Shop.BasketDomain.Domain.Repositories;

namespace Shop.BasketDomain.Application.Services;

public class DiscountCodeService(IDiscountCodeRepository repository) : IDiscountCodeService
{
    public async Task AddDiscountCode(AddDiscountDto dto)
    {
        var discount = new Discount(dto.Code, dto.PercentageOff);

        await repository.SaveDiscountAsync(discount);
    }

    public async Task<GetDiscountResult?> GetDiscountCode(string discountCode)
    {
        var reault = await repository.GetDiscountAsync(discountCode);

        return reault != null
            ? new GetDiscountResult(reault.Code, reault.PercentageOff)
            : null;
    }
}