using Microsoft.AspNetCore.Mvc;
using Shop.BasketDomain.Application.Dtos;
using Shop.BasketDomain.Application.Services;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
public class BasketController(IBasketService basketService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> CreateBasket()
    {
        var basketId = await basketService.CreateBasketAsync();

        return CreatedAtAction(nameof(GetBasket), new { basketId }, basketId);
    }

    [HttpGet("{basketId}")]
    [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BasketDto>> GetBasket(Guid basketId)
    {
        var result = await basketService.GetBasketAsync(basketId);

        return result.IsSuccessful
            ? Ok(result.Dto)
            : NotFound($"Basket with ID {basketId} not found");
    }

    [HttpPost("{basketId}/items")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddItem(Guid basketId, AddItemDto request)
    {
        var result = await basketService.AddItemAsync(basketId, request);

        return result switch
        {
            AddItemResult.Success => Ok($"Added {request.Quantity} of product {request.ProductId} to basket"),
            AddItemResult.BasketNotFound => BadRequest($"Basket with ID {basketId} not found"),
            AddItemResult.ProductNotFound => BadRequest($"Product with ID {request.ProductId} not found"),
            _ => throw new InvalidOperationException("Unexpected add item status")
        };
    }

    [HttpDelete("{basketId}/items")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveItem(Guid basketId, Guid productId)
    {
        var result = await basketService.RemoveItemAsync(basketId, productId);

        return result switch
        {
            RemoveItemResult.Success => Ok($"Removed {productId} from basket"),
            RemoveItemResult.BasketNotFound => BadRequest($"Basket with ID {basketId} not found"),
            RemoveItemResult.ItemNotInBasket => NotFound($"Item with ID {productId} not found in basket {basketId}"),
            _ => throw new InvalidOperationException("Unexpected remove item status")
        };
    }

    [HttpPost("{basketId}/discounts")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddDiscount(Guid basketId, string discountCode)
    {
        if (string.IsNullOrWhiteSpace(discountCode))
        {
            return BadRequest("Discount code cannot be empty");
        }

        var result = await basketService.AddDiscountAsync(basketId, discountCode);

        return result switch
        {
            AddDiscountResult.Success => Ok($"Applied discount code: {discountCode}"),
            AddDiscountResult.BasketNotFound => BadRequest($"Basket with ID {basketId} not found"),
            AddDiscountResult.InvalidDiscountCode => BadRequest($"Invalid discount code: {discountCode}"),
            _ => throw new InvalidOperationException("Unexpected add discount status")
        };
    }

    [HttpPost("{basketId}/shipping")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SetShippingCountry(Guid basketId, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return BadRequest("Discount code cannot be empty");
        }

        var result = await basketService.SetShippingCountryAsync(basketId, countryCode);

        return result switch
        {
            SetShippingCountryResult.Success => Ok($"Updated shipping to {countryCode}"),
            SetShippingCountryResult.BasketNotFound => BadRequest($"Basket with ID {basketId} not found"),
            SetShippingCountryResult.ShippingNotAvailable => BadRequest($"Shipping not available for country: {countryCode}"),
            _ => throw new InvalidOperationException("Unexpected set shipping status")
        };
    }

    [HttpDelete("{basketId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteBasket(Guid basketId)
    {
        var deleted = await basketService.DeleteBasketAsync(basketId);

        return deleted
            ? Ok($"Basket with ID {basketId} deleted successfully")
            : NotFound($"Basket with ID {basketId} not found");
    }
}