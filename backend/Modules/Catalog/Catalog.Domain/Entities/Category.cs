using BuildingBlocks.Entities;

namespace Catalog.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = default!;

    private Category() { }

    public static Category Create(string name) => new() { Name = name.Trim() };
}
