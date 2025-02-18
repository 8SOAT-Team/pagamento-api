namespace Pagamentos.Adapters.Controllers;
[ExcludeFromCodeCoverage]
public record PagamentoResponseDto(
    Guid Id,
    MetodosDePagamento MetodoDePagamento,
    StatusDoPagamento Status,
    decimal ValorTotal,
    string PagamentoExternoId,
    string? UrlPagamento,
    Guid PedidoId);