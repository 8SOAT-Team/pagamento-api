namespace Pagamentos.Domain.Entities.DomainEvents;

public record PagamentoConfirmadoDomainEvent(Guid PagamentoId) : DomainEvent;