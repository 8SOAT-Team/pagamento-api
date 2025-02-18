using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.UseCases.Dtos;
[ExcludeFromCodeCoverage]
public record ConfirmarPagamentoDto(Guid PagamentoId, StatusPagamento Status);