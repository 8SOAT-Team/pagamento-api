namespace Pagamentos.Apps.UseCases;

public class ObterPagamentoByPedidoUseCase(
    ILogger<ObterPagamentoByPedidoUseCase> logger,
    IPagamentoGateway pagamentoGateway)
    : UseCase<ObterPagamentoByPedidoUseCase, Guid, List<Domain.Entities.Pagamento>>(logger)
{
    protected override async Task<List<Domain.Entities.Pagamento>?> Execute(Guid pedidoId)
    {
        var pagamentos = await pagamentoGateway.FindPagamentoByPedidoIdAsync(pedidoId);
        return pagamentos?.Count > 0 ? pagamentos : null;
    }
}