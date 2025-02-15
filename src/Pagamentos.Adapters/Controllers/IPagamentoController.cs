using Pagamentos.Adapters.Types;

namespace Pagamentos.Adapters.Controllers;
public interface IPagamentoController
{
    Task<Result<List<PagamentoResponseDTO>>> GetPagamentoByPedidoAsync(Guid pedidoId);
    Task<Result<PagamentoResponseDTO>> ConfirmarPagamento(Guid pagamentoId, StatusDoPagamento status);
    Task<Result<PagamentoResponseDTO>> IniciarPagamento(Guid pedidoId, MetodosDePagamento metodoDePagamento, decimal ValorTotal, string EmailPagador);
    Task<Result<PagamentoResponseDTO>> ReceberWebhookPagamento(string pagamentoExternoId);
}
