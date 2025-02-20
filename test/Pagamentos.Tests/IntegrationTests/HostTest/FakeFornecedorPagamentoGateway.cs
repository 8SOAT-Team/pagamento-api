using Pagamentos.Apps.UseCases;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Tests.IntegrationTests.HostTest;

public class FakeFornecedorPagamentoGateway : IFornecedorPagamentoGateway
{
    public Task<FornecedorCriarPagamentoResponseDto> IniciarPagamento(IniciarPagamentoDto request,
        CancellationToken cancellationToken = default)
    {
        var pagamentoId = Guid.NewGuid().ToString();
        return Task.FromResult(
            new FornecedorCriarPagamentoResponseDto(pagamentoId, $"https://fastorder.com.br/pagamento/{pagamentoId}"));
    }

    public Task<FornecedorGetPagamentoResponseDto> ObterPagamento(string idExterno,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            new FornecedorGetPagamentoResponseDto(Guid.NewGuid().ToString(), StatusPagamento.Autorizado));
    }
}