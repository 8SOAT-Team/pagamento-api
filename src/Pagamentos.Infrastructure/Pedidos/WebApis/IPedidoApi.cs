using Refit;

namespace Pagamentos.Infrastructure.Pedidos.WebApis;

public interface IPedidoApi
{
    [Post("/v1/pedido/{pedidoId}/status-pagamento")]
    Task<IApiResponse> AtualizaPedidoStatusPagamento(Guid pedidoId,
        [Body] AtualizaPedidoStatusPagamentoRequest request, [Header("x-requestid")] string? requestId = null);
}