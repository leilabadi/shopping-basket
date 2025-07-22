using Shop.BasketDomain.Application.Dtos;
using Shop.BasketDomain.Application.Factories;
using Shop.BasketDomain.Application.Mappers;
using Shop.BasketDomain.Domain.Factories;
using Shop.BasketDomain.Domain.Model;
using Shop.BasketDomain.Domain.Repositories;
using Shop.BasketDomain.Domain.Services;
using Shop.ProductDomain.Application.Services;
using Shop.ShippingDomain.Application.Services;

namespace Shop.BasketDomain.Application.Services;

public class BasketService
(
    IBasketRepository basketRepository,
    IProductService productService,
    IPriceCalculationService priceCalculationService,
    ITaxCalculationService taxCalculationService,
    IShippingService shippingService,
    IDiscountCodeService discountCodeService,
    BasketFactory basketFactory
) : IBasketService
{
    public async Task<Guid> CreateBasketAsync()
    {
        var defaultShipping = await shippingService.GetShippingAsync("UK");
        if (defaultShipping == null) throw new InvalidOperationException("Default shipping not available");

        var basketShipping = BasketMapper.ToBasketShipping(defaultShipping);
        var basket = basketFactory.CreateBasket(basketShipping);

        return await basketRepository.SaveBasketAsync(basket);
    }

    public async Task<GetBasketResponse> GetBasketAsync(Guid basketId)
    {
        var basket = await basketRepository.GetBasketAsync(basketId);
        if (basket == null) return GetBasketResponse.Failure();

        var subtotalWithoutVat = priceCalculationService.CalculateTotalCost(basket);
        var vat = taxCalculationService.CalculateVAT(subtotalWithoutVat);
        var totalWithVat = taxCalculationService.AddVAT(subtotalWithoutVat);

        var dto = BasketDtoFactory.Create(basket, subtotalWithoutVat, vat, totalWithVat);

        return GetBasketResponse.Success(dto);
    }

    public async Task<AddItemResult> AddItemAsync(Guid basketId, AddItemDto request)
    {
        var basket = await basketRepository.GetBasketAsync(basketId);
        if (basket == null) return AddItemResult.BasketNotFound;

        var product = await productService.GetProductAsync(request.ProductId);
        if (product == null) return AddItemResult.ProductNotFound;

        var basketProduct = BasketMapper.ToBasketProduct(product);

        basket.AddItem(basketProduct, request.Quantity);
        await basketRepository.UpdateBasketAsync(basketId, basket);

        return AddItemResult.Success;
    }

    public async Task<RemoveItemResult> RemoveItemAsync(Guid basketId, Guid productId)
    {
        var basket = await basketRepository.GetBasketAsync(basketId);
        if (basket == null) return RemoveItemResult.BasketNotFound;

        var product = await productService.GetProductAsync(productId);
        if (product == null) return RemoveItemResult.ItemNotInBasket;

        if (basket.RemoveItem(product.Id))
        {
            await basketRepository.UpdateBasketAsync(basketId, basket);
        }

        return RemoveItemResult.Success;
    }

    public async Task<AddDiscountResult> AddDiscountAsync(Guid basketId, string discountCode)
    {
        var basket = await basketRepository.GetBasketAsync(basketId);
        if (basket == null) return AddDiscountResult.BasketNotFound;

        var discount = await discountCodeService.GetDiscountCode(discountCode);
        if (discount == null) return AddDiscountResult.InvalidDiscountCode;

        var basketDiscount = new Discount(discountCode, discount.PercentageOff);

        basket.AddDiscount(basketDiscount);
        await basketRepository.UpdateBasketAsync(basketId, basket);

        return AddDiscountResult.Success;
    }

    public async Task<SetShippingCountryResult> SetShippingCountryAsync(Guid basketId, string countryCode)
    {
        var basket = await basketRepository.GetBasketAsync(basketId);
        if (basket == null) return SetShippingCountryResult.BasketNotFound;

        var shipping = await shippingService.GetShippingAsync(countryCode);
        if (shipping == null) return SetShippingCountryResult.ShippingNotAvailable;

        var basketShipping = BasketMapper.ToBasketShipping(shipping);

        basket.SetShipping(basketShipping);
        await basketRepository.UpdateBasketAsync(basketId, basket);

        return SetShippingCountryResult.Success;
    }

    public async Task<bool> DeleteBasketAsync(Guid basketId)
    {
        return await basketRepository.DeleteBasketAsync(basketId);
    }
}