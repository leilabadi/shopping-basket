using Microsoft.AspNetCore.Mvc;
using Shop.ProductDomain.Application.Dtos;
using Shop.ProductDomain.Application.Services;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create(CreateProductDto request)
    {
        var productId = await productService.CreateProductAsync(request);

        return CreatedAtAction(nameof(Get), new { productId }, productId);
    }

    [HttpGet("{productId}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> Get(Guid productId)
    {
        var product = await productService.GetProductAsync(productId);

        return product != null
            ? Ok(product)
            : NotFound($"Product with ID {productId} not found");
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpDelete("{productId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid productId)
    {
        var success = await productService.DeleteProductAsync(productId);

        return success
            ? Ok("Product deleted successfully")
            : NotFound($"Product with ID {productId} not found");
    }
}