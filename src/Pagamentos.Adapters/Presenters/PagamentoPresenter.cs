using Pagamentos.Adapters.Controllers;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Adapters.Presenters;

public static class PagamentoPresenter
{
    public static List<PagamentoResponseDto> ToListPagamentoDto(List<Pagamento> pagamentos)
        => pagamentos.Select(ToPagamentoDto).ToList();

    public static PagamentoResponseDto ToPagamentoDto(Pagamento pagamento)
        => new(pagamento.Id,
            (MetodosDePagamento)pagamento.MetodoDePagamento,
            (StatusDoPagamento)pagamento.Status,
            pagamento.ValorTotal,
            pagamento.PagamentoExternoId!,
            pagamento.UrlPagamento,
            pagamento.PedidoId);
}