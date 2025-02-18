using Pagamentos.Adapters.Types;
using Pagamentos.Apps.UseCases.Dtos;

namespace Pagamentos.Adapters.Controllers;

public interface IPagamentoController
{
    Task<Result<List<PagamentoResponseDto>>> GetPagamentoByPedidoAsync(Guid pedidoId);
    Task<Result<PagamentoResponseDto>> ConfirmarPagamento(Guid pagamentoId, StatusDoPagamento status);
    Task<Result<PagamentoResponseDto>> IniciarPagamento(IniciarPagamentoDto iniciarPagamentoPagadorDto);
    Task<Result<PagamentoResponseDto>> ReceberWebhookPagamento(string pagamentoExternoId);
}