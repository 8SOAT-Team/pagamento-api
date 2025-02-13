using Pagamento.Domain.Entities;

namespace Pagamento.Apps.UseCases.Dtos;

public record ConfirmarPagamentoDto(Guid PagamentoId, StatusPagamento Status);