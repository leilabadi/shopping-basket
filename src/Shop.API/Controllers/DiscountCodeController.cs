using Microsoft.AspNetCore.Mvc;
using Shop.BasketDomain.Application.Dtos;
using Shop.BasketDomain.Application.Services;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
public class DiscountCodeController(IDiscountCodeService discountCodeService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(AddDiscountDto dto)
    {
        await discountCodeService.AddDiscountCode(dto);

        return CreatedAtAction(nameof(Get), new { code = dto.Code }, $"Discount code {dto.Code} created.");
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(GetDiscountResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string code)
    {
        var discount = await discountCodeService.GetDiscountCode(code);

        return discount != null
            ? Ok(discount)
            : NotFound($"Discount code {code} not found.");
    }
}
