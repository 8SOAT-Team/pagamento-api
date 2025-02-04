using Pagamento.Domain.Entities;

namespace Pagamento.Apps.UseCases.Dtos;

public record IniciarPagamentoDto(Guid PedidoId, MetodoDePagamento MetodoDePagamento);
