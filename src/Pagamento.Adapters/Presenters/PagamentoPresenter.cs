using Pagamento.Adapters.Controllers;

namespace Pagamento.Adapters.Presenters;

public static class PagamentoPresenter
{
    public static List<PagamentoResponseDTO> ToListPagamentoDTO(List<Pagamento.Domain.Entities.Pagamento> pagamentos)
    => pagamentos.Select(p => ToPagamentoDTO(p)).ToList();

    public static PagamentoResponseDTO ToPagamentoDTO(Pagamento.Domain.Entities.Pagamento pagamento)
    => new(pagamento.Id,
        (MetodosDePagamento)pagamento.MetodoDePagamento,
        (StatusDoPagamento)pagamento.Status,
        pagamento.ValorTotal,
        pagamento.PagamentoExternoId!,
        pagamento.UrlPagamento);
}
