namespace BuildingBlocks.DomainEvents;

/// <summary>
/// Marker interface for domain events raised by entities.
/// This project intentionally has no message-bus / MediatR dependency
/// (AGENTS.md section 3: "in-process interface calls only, no message bus").
/// A module's own Infrastructure layer may dispatch these synchronously
/// after SaveChanges if/when needed.
/// </summary>
public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
