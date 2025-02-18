using Pagamentos.Domain.Entities;
using Pagamentos.Apps.UseCases.Dtos;

namespace Pagamentos.Apps.UseCases;

public interface IFornecedorPagamentoGateway
{
    Task<FornecedorCriarPagamentoResponseDto> IniciarPagamento(IniciarPagamentoDto request,
        CancellationToken cancellationToken = default);

    Task<FornecedorGetPagamentoResponseDto> ObterPagamento(string idExterno,
        CancellationToken cancellationToken = default);
}