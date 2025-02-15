using CleanArch.UseCase.Faults;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.UseCases;

public class ConfirmarStatusPagamentoUseCase(ILogger<ConfirmarStatusPagamentoUseCase> logger, 
    IFornecedorPagamentoGateway fornecedorPagamentoGateway,
    IPagamentoGateway pagamentoGateway) : UseCase<ConfirmarStatusPagamentoUseCase, string, Pagamento>(logger)
{
    protected override async Task<Pagamento?> Execute(string pagamentoExternoId)
    {
        var pagamento = await pagamentoGateway.FindPagamentoByExternoIdAsync(pagamentoExternoId);
        
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
        
        var pagamentoExterno = await fornecedorPagamentoGateway.ObterPagamento(pagamento.PagamentoExternoId!);

        if (pagamentoExterno is null)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento Externo não encontrado"));
            return null;
        }
      

        if (pagamentoExterno.StatusPagamento == StatusPagamento.Pendente)
        {
            return pagamento;
        }

        pagamento.FinalizarPagamento(pagamentoExterno.StatusPagamento == StatusPagamento.Autorizado);
        var pagamentoAtualizado = await pagamentoGateway.UpdatePagamentoAsync(pagamento);

        return pagamentoAtualizado;
    }
}
