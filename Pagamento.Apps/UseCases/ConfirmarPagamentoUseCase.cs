using CleanArch.UseCase.Faults;
using CleanArch.UseCase.Logging;
using Pagamento.Apps.UseCases.Dtos;
using Pagamento.Domain.Entities;
using ILogger = CleanArch.UseCase.Logging.ILogger;

namespace Pagamento.Apps.UseCases;

public class ConfirmarPagamentoUseCase(
    ILogger logger,
    IPagamentoGateway pagamentoGateway) : UseCase<ConfirmarPagamentoDto, Domain.Entities.Pagamento>(logger)
{
    private readonly IPagamentoGateway _pagamentoGateway = pagamentoGateway;

    protected override async Task<Domain.Entities.Pagamento?> Execute(ConfirmarPagamentoDto dto)
    {
        var pagamento = await _pagamentoGateway.GetByIdAsync(dto.PagamentoId);

        if (pagamento == null)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento não encontrado"));
            return null;
        }

        if (pagamento.PedidoId == Guid.Empty)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pedido não encontrado"));
            return null;
        }

        pagamento.FinalizarPagamento(dto.Status == StatusPagamento.Autorizado);
        var pagamentoAtualizado = await _pagamentoGateway.UpdatePagamentoAsync(pagamento);

        //if (pagamento.EstaAutorizado())
        //{
        //    pedido.IniciarPreparo(pagamento);
        //    await _pedidoGateway.UpdateAsync(pedido);
        //}

        return pagamentoAtualizado;
    }
}