using CleanArch.UseCase.Faults;
using Pagamento.Apps.UseCases.Dtos;
using ILogger = CleanArch.UseCase.Logging.ILogger;

namespace Pagamento.Apps.UseCases;

public class IniciarPagamentoUseCase(
    ILogger logger,
    IPedidoGateway pedidoGateway,
    IPagamentoGateway pagamentoGateway,
    IFornecedorPagamentoGateway fornecedorPagamentoGateway) : UseCase<IniciarPagamentoDto, Domain.Entities.Pagamento>(logger)
{
    private readonly IPedidoGateway _pedidoGateway = pedidoGateway;
    private readonly IPagamentoGateway _pagamentoGateway = pagamentoGateway;
    private readonly IFornecedorPagamentoGateway _fornecedorPagamentoGateway = fornecedorPagamentoGateway;
    protected override async Task<Domain.Entities.Pagamento?> Execute(IniciarPagamentoDto command)
    {
        var pedido = await _pedidoGateway.GetPedidoCompletoAsync(command.PedidoId);

        if (pedido is null)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pedido não encontrado"));
            return null;
        }

        if (pedido.Pagamento is not null)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento já iniciado"));
            return null;
        }

        pedido.IniciarPagamento(command.MetodoDePagamento);

        var fornecedorResponse = await _fornecedorPagamentoGateway.IniciarPagamento(command.MetodoDePagamento, pedido.Cliente?.Email ?? "",
            pedido.ValorTotal, pedido.Pagamento!.Id.ToString(), pedido.Id);

        pedido.Pagamento!.AssociarPagamentoExterno(fornecedorResponse.IdExterno, fornecedorResponse.UrlPagamento);

        await _pedidoGateway.AtualizarPedidoPagamentoIniciadoAsync(pedido);

        return pedido.Pagamento;
    }
}
