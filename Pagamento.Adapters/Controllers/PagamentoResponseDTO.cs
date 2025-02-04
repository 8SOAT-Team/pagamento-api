namespace Pagamento.Adapters.Controllers;

public record PagamentoResponseDTO(Guid Id, MetodosDePagamento MetodoDePagamento, StatusDoPagamento status, decimal ValorTotal, string PagamentoExternoId, string? UrlPagamento);
