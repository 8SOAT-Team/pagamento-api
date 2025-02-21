
using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.UseCases.Dtos;

[ExcludeFromCodeCoverage]
public record FornecedorGetPagamentoResponseDto(string IdExterno, StatusPagamento StatusPagamento);

