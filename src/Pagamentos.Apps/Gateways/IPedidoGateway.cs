using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.Gateways;

public interface IPedidoGateway
{
    Task AtualizaStatusPagamentoAsync(Guid pedidoId, StatusPagamento status);
}