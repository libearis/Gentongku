using Catalog.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Gentongku.Api.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController(ICatalogQueries catalogQueries) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct) => Ok(await catalogQueries.ListCategoriesAsync(ct));

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] int take = 20, CancellationToken ct = default) =>
        Ok(await catalogQueries.ListProductsAsync(take, ct));

    [HttpGet("products/{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id, CancellationToken ct)
    {
        var product = await catalogQueries.GetProductAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }
}
