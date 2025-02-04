using Pagamento.Adapters.Types;

namespace Pagamento.Adapters.Controllers;
public interface IPagamentoController
{
    Task<Result<List<PagamentoResponseDTO>>> GetPagamentoByPedidoAsync(Guid pedidoId);
    Task<Result<PagamentoResponseDTO>> ConfirmarPagamento(Guid pagamentoId, StatusDoPagamento status);
    Task<Result<PagamentoResponseDTO>> IniciarPagamento(Guid pedidoId, MetodosDePagamento metodoDePagamento);
    Task<Result<PagamentoResponseDTO>> ReceberWebhookPagamento(string pagamentoExternoId);
}
