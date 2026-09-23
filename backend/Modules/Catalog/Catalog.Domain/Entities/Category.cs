using BuildingBlocks.Entities;

namespace Catalog.Domain.Entities;

// One of the three read-benchmark tables (~42 rows), AGENTS.md section 6.1.
public class Category : BaseEntity
{
    public string Name { get; private set; } = default!;

    private Category() { }

    public static Category Create(string name) => new() { Name = name.Trim() };
}
