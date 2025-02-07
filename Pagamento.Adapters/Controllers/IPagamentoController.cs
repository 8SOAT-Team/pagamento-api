using Pagamento.Adapters.Types;

namespace Pagamento.Adapters.Controllers;
public interface IPagamentoController
{
    Task<Result<List<PagamentoResponseDTO>>> GetPagamentoByPedidoAsync(Guid pedidoId);
    Task<Result<PagamentoResponseDTO>> ConfirmarPagamento(Guid pagamentoId, StatusDoPagamento status);
    Task<Result<PagamentoResponseDTO>> IniciarPagamento(Guid pedidoId, MetodosDePagamento metodoDePagamento, decimal ValorTotal, string EmailPagador, string webhookUrl, string token);
    Task<Result<PagamentoResponseDTO>> ReceberWebhookPagamento(string pagamentoExternoId, string token);
}
