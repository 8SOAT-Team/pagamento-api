using System.Diagnostics.CodeAnalysis;

namespace Pagamentos.Domain.Entities;

public abstract class Entity : IEntity
{
    private readonly List<DomainEvent> _domainEvents = [];

    public Guid Id { get; protected init; }

    protected void RaiseEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearEvents()
    {
        _domainEvents.Clear();
    }

    public IReadOnlyList<DomainEvent> ReleaseEvents()
    {
        var events = new DomainEvent[_domainEvents.Count];
        _domainEvents.CopyTo(events);
        _domainEvents.Clear();
        return events;
    }
}

public interface IEntity
{
    public Guid Id { get; }
}

public interface IAggregateRoot : IEntity
{
}

[ExcludeFromCodeCoverage]
public abstract record DomainEvent
{
    public Guid EventId { get; protected init; } = Guid.NewGuid();
    public DateTime Timestamp { get; protected init; } = DateTime.UtcNow;
}

public abstract record DomainEventResponse
{
    public Guid EventId { get; protected init; }
    public DateTime Timestamp { get; protected init; }
}