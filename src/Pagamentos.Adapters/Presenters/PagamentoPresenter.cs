using Pagamentos.Adapters.Controllers;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Adapters.Presenters;

public static class PagamentoPresenter
{
    public static List<PagamentoResponseDTO> ToListPagamentoDto(List<Pagamento> pagamentos)
    => pagamentos.Select(p => ToPagamentoDto(p)).ToList();

    public static PagamentoResponseDTO ToPagamentoDto(Pagamento pagamento)
    => new(pagamento.Id,
        (MetodosDePagamento)pagamento.MetodoDePagamento,
        (StatusDoPagamento)pagamento.Status,
        pagamento.ValorTotal,
        pagamento.PagamentoExternoId!,
        pagamento.UrlPagamento);
}
