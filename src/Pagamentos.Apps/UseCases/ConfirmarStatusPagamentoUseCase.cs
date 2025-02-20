using CleanArch.UseCase.Faults;
using Pagamentos.Apps.Handlers;
using Pagamentos.Apps.Types;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.UseCases;

public class ConfirmarStatusPagamentoUseCase(
    ILogger<ConfirmarStatusPagamentoUseCase> logger,
    IFornecedorPagamentoGateway fornecedorPagamentoGateway,
    IPagamentoGateway pagamentoGateway,
    IPagamentoHandler pagamentoHandler) : UseCase<ConfirmarStatusPagamentoUseCase, string, Pagamento>(logger)
{
    protected override async Task<Pagamento?> Execute(string pagamentoExternoId)
    {
        var fornecedorPagamentoResponse = await fornecedorPagamentoGateway.ObterPagamento(pagamentoExternoId);

        if (fornecedorPagamentoResponse is null)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento Externo não encontrado"));
            return null;
        }

        var pagamentos =
            await pagamentoGateway.FindPagamentoByPedidoIdAsync(Guid.Parse(fornecedorPagamentoResponse.IdExterno));
        var pagamento = pagamentos.FirstOrDefault();

        if (pagamento is null)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento não encontrado"));
            return null;
        }

        if (pagamento.EstaPendente() is false)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento não está pendente"));
            return null;
        }

        if (fornecedorPagamentoResponse.StatusPagamento == StatusPagamento.Pendente)
        {
            return pagamento;
        }

        pagamento.FinalizarPagamento(fornecedorPagamentoResponse.StatusPagamento == StatusPagamento.Autorizado);

        var problems = new List<AppProblemDetails>();
        foreach (var @event in pagamento.ReleaseEvents())
        {
            var result = await pagamentoHandler.HandleAsync(@event!);
            result.Match(p => pagamento = p, p => problems.AddRange(p));
        }

        if (problems.Count != 0)
        {
            AddError(problems.Select(p => new UseCaseError(UseCaseErrorType.InternalError, p.Detail)));
            return pagamento;
        }

        var pagamentoAtualizado = await pagamentoGateway.UpdatePagamentoAsync(pagamento);

        return pagamentoAtualizado;
    }
}