using CleanArch.UseCase.Faults;
using Pagamento.Domain.Entities;
using ILogger = CleanArch.UseCase.Logging.ILogger;

namespace Pagamento.Apps.UseCases;

public class ConfirmarStatusPagamentoUseCase(string token, ILogger logger, 
    IFornecedorPagamentoGateway fornecedorPagamentoGateway,
    IPagamentoGateway pagamentoGateway) : UseCase<string, Domain.Entities.Pagamento>(logger)
{
    private readonly IFornecedorPagamentoGateway _fornecedorPagamentoGateway = fornecedorPagamentoGateway;
    private readonly IPagamentoGateway _pagamentoGateway = pagamentoGateway;

    protected override async Task<Domain.Entities.Pagamento?> Execute(string pagamentoExternoId)
    {
        var pagamentoExterno = await _fornecedorPagamentoGateway.ObterPagamento(pagamentoExternoId, token);

        if (pagamentoExterno is null)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento Externo não encontrado"));
            return null;
        }

        var pagamento = await _pagamentoGateway.GetByIdAsync(pagamentoExterno.PagamentoId);

        if (pagamento is null)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento não encontrado"));
            return null;
        }

        if (pagamentoExterno.StatusPagamento == StatusPagamento.Pendente)
        {
            return pagamento;
        }

        pagamento.FinalizarPagamento(pagamentoExterno.StatusPagamento == StatusPagamento.Autorizado);
        var pagamentoAtualizado = await _pagamentoGateway.UpdatePagamentoAsync(pagamento);

        return pagamentoAtualizado;
    }
}
