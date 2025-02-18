
using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.UseCases.Dtos;
[ExcludeFromCodeCoverage]
public record FornecedorGetPagamentoResponseDto(string IdExterno, Guid PagamentoId, StatusPagamento StatusPagamento)
{
    public static implicit operator Task<object>(FornecedorGetPagamentoResponseDto v)
    {
        throw new NotImplementedException();
    }
}
