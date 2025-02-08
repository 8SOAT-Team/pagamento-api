
using Pagamento.Domain.Entities;

namespace Pagamento.Apps.UseCases.Dtos;

public record FornecedorGetPagamentoResponseDto(string IdExterno, Guid PagamentoId, StatusPagamento StatusPagamento)
{
    public static implicit operator Task<object>(FornecedorGetPagamentoResponseDto v)
    {
        throw new NotImplementedException();
    }
}
