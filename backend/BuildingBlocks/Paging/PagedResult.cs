namespace BuildingBlocks.Paging;

/// <summary>
/// Request parameters shared by any module's "list" queries.
/// </summary>
public sealed class PageRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public int Skip => (Math.Max(Page, 1) - 1) * Math.Max(PageSize, 1);
    public int Take => Math.Max(PageSize, 1);
}

/// <summary>
/// Generic paged response envelope used across module Application layers.
/// </summary>
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public long TotalCount { get; init; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static PagedResult<T> Create(IReadOnlyList<T> items, long totalCount, PageRequest request) => new()
    {
        Items = items,
        Page = request.Page,
        PageSize = request.PageSize,
        TotalCount = totalCount
    };
}
