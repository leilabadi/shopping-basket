using Shop.BasketDomain.Application.Dtos;

namespace Shop.BasketDomain.Application.Services;
public interface IBasketService
{
    Task<Guid> CreateBasketAsync();
    Task<GetBasketResponse> GetBasketAsync(Guid basketId);
    Task<AddItemResult> AddItemAsync(Guid basketId, AddItemDto request);
    Task<RemoveItemResult> RemoveItemAsync(Guid basketId, Guid productId);
    Task<AddDiscountResult> AddDiscountAsync(Guid basketId, string discountCode);
    Task<SetShippingCountryResult> SetShippingCountryAsync(Guid basketId, string countryCode);
    Task<bool> DeleteBasketAsync(Guid basketId);
}