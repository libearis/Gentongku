namespace BuildingBlocks.DomainEvents;

// Intentionally no message-bus/MediatR dependency; in-process interface calls only.
public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
