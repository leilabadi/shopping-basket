using Microsoft.AspNetCore.Mvc;
using Shop.ShippingDomain.Application.Dtos;
using Shop.ShippingDomain.Application.Services;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
public class ShippingController(IShippingService shippingService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Save([FromBody] ShippingDto dto)
    {
        await shippingService.SaveShippingAsync(dto);
        return NoContent();
    }

    [HttpGet("{country}")]
    [ProducesResponseType(typeof(ShippingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShippingDto>> Get(string country)
    {
        var shipping = await shippingService.GetShippingAsync(country);

        return shipping != null
            ? Ok(shipping)
            : NotFound($"Shipping for country '{country}' not found");
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ShippingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ShippingDto>>> GetAll()
    {
        var shippings = await shippingService.GetAllShippingsAsync();
        return Ok(shippings);
    }

    [HttpDelete("{country}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string country)
    {
        var deleted = await shippingService.DeleteShippingAsync(country);

        return deleted
            ? NoContent()
            : NotFound($"Shipping for country '{country}' not found");
    }
}