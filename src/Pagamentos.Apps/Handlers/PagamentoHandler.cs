using Pagamentos.Apps.Gateways;
using Pagamentos.Apps.Types;
using Pagamentos.Apps.UseCases;
using Pagamentos.Domain.Entities;
using Pagamentos.Domain.Entities.DomainEvents;

namespace Pagamentos.Apps.Handlers;

public interface IPagamentoHandler
{
    Task<Result<Pagamento>> HandleAsync(DomainEvent @event);
}

public class PagamentoHandler : IPagamentoHandler
{
    private readonly IPedidoGateway _pedidoGateway;
    private readonly IPagamentoGateway _pagamentoGateway;

    public PagamentoHandler(IPedidoGateway pedidoGateway, IPagamentoGateway pagamentoGateway)
    {
        _pedidoGateway = pedidoGateway;
        _pagamentoGateway = pagamentoGateway;
    }

    public Task<Result<Pagamento>> HandleAsync(DomainEvent @event) => @event switch
    {
        PagamentoConfirmadoDomainEvent e => HandleAsync(e),
        _ => throw new Exception("Evento não suportado")
    };

    public async Task<Result<Pagamento>> HandleAsync(PagamentoConfirmadoDomainEvent @event)
    {
        var pagamento = await _pagamentoGateway.GetByIdAsync(@event.PagamentoId);
        await _pedidoGateway.AtualizaStatusPagamentoAsync(pagamento!.PedidoId, pagamento.Status);
        return Result<Pagamento>.Succeed(pagamento);
    }
}