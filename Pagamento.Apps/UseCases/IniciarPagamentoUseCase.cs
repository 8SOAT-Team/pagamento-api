using CleanArch.UseCase.Faults;
using Pagamento.Apps.UseCases.Dtos;
using ILogger = CleanArch.UseCase.Logging.ILogger;

namespace Pagamento.Apps.UseCases;

public class IniciarPagamentoUseCase(
    ILogger logger,
    IPagamentoGateway pagamentoGateway,
    IFornecedorPagamentoGateway fornecedorPagamentoGateway) : UseCase<IniciarPagamentoDto, Domain.Entities.Pagamento>(logger)
{
    private readonly IPagamentoGateway _pagamentoGateway = pagamentoGateway;
    private readonly IFornecedorPagamentoGateway _fornecedorPagamentoGateway = fornecedorPagamentoGateway;
    protected override async Task<Domain.Entities.Pagamento?> Execute(IniciarPagamentoDto command)
    {

        if (command.PedidoId == Guid.Empty)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pedido não encontrado"));
            return null;
        }

        var pagamento = _pagamentoGateway.FindPagamentoByPedidoIdAsync(command.PedidoId);
        if (pagamento is not null)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento já iniciado"));
            return null;
        }

        Domain.Entities.Pagamento Pagamento = new Domain.Entities.Pagamento(command.PedidoId, command.MetodoDePagamento, command.ValorTotal, null);

        var fornecedorResponse = await _fornecedorPagamentoGateway.IniciarPagamento(command.MetodoDePagamento, command.EmailPagador ?? "",
            command.ValorTotal, pagamento.Id.ToString(), command.PedidoId, command.webhookUrl, command.token);

        Pagamento.AssociarPagamentoExterno(fornecedorResponse.IdExterno, fornecedorResponse.UrlPagamento);

        //await _pedidoGateway.AtualizarPedidoPagamentoIniciadoAsync(pedido);

        return Pagamento;
    }
}
