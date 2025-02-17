using CleanArch.UseCase.Faults;
using Pagamentos.Domain.Entities;
using Pagamentos.Apps.UseCases.Dtos;

namespace Pagamentos.Apps.UseCases;

public class ConfirmarPagamentoUseCase(
    ILogger<ConfirmarPagamentoUseCase> logger,
    IPagamentoGateway pagamentoGateway) : UseCase<ConfirmarPagamentoUseCase, ConfirmarPagamentoDto, Pagamento>(logger)
{
    protected override async Task<Pagamento?> Execute(ConfirmarPagamentoDto dto)
    {
        var pagamento = await pagamentoGateway.GetByIdAsync(dto.PagamentoId);

        if (pagamento == null)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento não encontrado"));
            return null;
        }

        pagamento.FinalizarPagamento(dto.Status == StatusPagamento.Autorizado);
        var pagamentoAtualizado = await pagamentoGateway.UpdatePagamentoAsync(pagamento);

        return pagamentoAtualizado;
    }
}