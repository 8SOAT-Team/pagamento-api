using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.UseCases.Dtos;

public record ConfirmarPagamentoDto(Guid PagamentoId, StatusPagamento Status);