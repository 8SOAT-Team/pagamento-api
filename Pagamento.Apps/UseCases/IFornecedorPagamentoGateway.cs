using Pagamento.Domain.Entities;
using Pagamento.Apps.UseCases.Dtos;

namespace Pagamento.Apps.UseCases;

public interface IFornecedorPagamentoGateway
{
    Task<FornecedorCriarPagamentoResponseDto> IniciarPagamento(MetodoDePagamento metodoDePagamento,
            string emailPagador, decimal valorTotal, string referenciaExternaId, Guid pedidoId, CancellationToken cancellationToken = default);
    Task<FornecedorGetPagamentoResponseDto> ObterPagamento(string IdExterno, CancellationToken cancellationToken = default);
}
