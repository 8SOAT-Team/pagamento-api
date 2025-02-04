using ILogger = CleanArch.UseCase.Logging.ILogger;

namespace Pagamento.Apps.UseCases;

public class ObterPagamentoByPedidoUseCase(
    ILogger logger,
    IPagamentoGateway pagamentoGateway) : UseCase<Guid, List<Domain.Entities.Pagamento>>(logger)
{
    private readonly IPagamentoGateway _pagamentoGateway = pagamentoGateway;

    protected override async Task<List<Domain.Entities.Pagamento>?> Execute(Guid pedidoId)
    {
        var pagamentos = await _pagamentoGateway.FindPagamentoByPedidoIdAsync(pedidoId);
        return pagamentos?.Count > 0 ? pagamentos : null;
    }
}
