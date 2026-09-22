namespace BuildingBlocks.Entities;

/// <summary>
/// Base class for all aggregate roots / entities across modules.
/// Provides identity, audit timestamps, and a domain-event buffer.
/// See AGENTS.md section 3 (BuildingBlocks: shared kernel) and section 12 (coding conventions).
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; protected set; }

    private readonly List<DomainEvents.IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvents.IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvents.IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
