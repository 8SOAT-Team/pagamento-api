using Pagamento.Domain.Entities;
using Pagamento.Apps.UseCases.Dtos;

namespace Pagamento.Apps.UseCases;

public interface IFornecedorPagamentoGateway
{
    Task<FornecedorCriarPagamentoResponseDto> IniciarPagamento(MetodoDePagamento metodoDePagamento,
            string emailPagador, decimal valorTotal, string referenciaExternaId, Guid pedidoId, string webhookUrl = "",
            string token = "", CancellationToken cancellationToken = default);
    Task<FornecedorGetPagamentoResponseDto> ObterPagamento(string IdExterno, string token, CancellationToken cancellationToken = default);
}
