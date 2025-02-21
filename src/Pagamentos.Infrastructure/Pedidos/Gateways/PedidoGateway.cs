using Pagamentos.Apps.Gateways;
using Pagamentos.Infrastructure.Pedidos.Mappers;
using Pagamentos.Infrastructure.Pedidos.WebApis;
using StatusPagamento = Pagamentos.Domain.Entities.StatusPagamento;

namespace Pagamentos.Infrastructure.Pedidos.Gateways;

public class PedidoGateway(IPedidoApi pedidoApi) : IPedidoGateway
{
    public async Task AtualizaStatusPagamentoAsync(Guid pedidoId, StatusPagamento status)
    {
        await pedidoApi.AtualizaPedidoStatusPagamento(pedidoId,
            new AtualizaPedidoStatusPagamentoRequest(status.ToStatusPagamento()));
    }
}