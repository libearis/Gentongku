using Catalog.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Gentongku.Api.Controllers;

/// <summary>Trivial passthrough endpoints proving the Catalog module is wired end-to-end.</summary>
[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogQueries _catalogQueries;

    public CatalogController(ICatalogQueries catalogQueries) => _catalogQueries = catalogQueries;

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct) => Ok(await _catalogQueries.ListCategoriesAsync(ct));

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] int take = 20, CancellationToken ct = default) =>
        Ok(await _catalogQueries.ListProductsAsync(take, ct));

    [HttpGet("products/{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id, CancellationToken ct)
    {
        var product = await _catalogQueries.GetProductAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }
}
