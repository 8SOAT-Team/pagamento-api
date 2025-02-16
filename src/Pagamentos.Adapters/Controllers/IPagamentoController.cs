using Pagamentos.Adapters.Types;
using Pagamentos.Apps.UseCases.Dtos;

namespace Pagamentos.Adapters.Controllers;

public interface IPagamentoController
{
    Task<Result<List<PagamentoResponseDTO>>> GetPagamentoByPedidoAsync(Guid pedidoId);
    Task<Result<PagamentoResponseDTO>> ConfirmarPagamento(Guid pagamentoId, StatusDoPagamento status);
    Task<Result<PagamentoResponseDTO>> IniciarPagamento(IniciarPagamentoDto iniciarPagamentoPagadorDto);
    Task<Result<PagamentoResponseDTO>> ReceberWebhookPagamento(string pagamentoExternoId);
}