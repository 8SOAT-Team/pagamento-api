using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.UseCases.Dtos;

public record IniciarPagamentoDto(Guid PedidoId, MetodoDePagamento MetodoDePagamento, decimal ValorTotal, string EmailPagador);
