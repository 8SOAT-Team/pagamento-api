using CleanArch.UseCase.Faults;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.UseCases;

public class IniciarPagamentoUseCase(
    ILogger<IniciarPagamentoUseCase> logger,
    IPagamentoGateway pagamentoGateway,
    IFornecedorPagamentoGateway fornecedorPagamentoGateway)
    : UseCase<IniciarPagamentoUseCase, IniciarPagamentoDto, Pagamento>(logger)
{
    protected override async Task<Pagamento?> Execute(IniciarPagamentoDto command)
    {
        if (command.PedidoId == Guid.Empty)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pedido não encontrado"));
            return null;
        }

        var pagamento = await pagamentoGateway.FindPagamentoByPedidoIdAsync(command.PedidoId);
        if (pagamento.Count > 0)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, "Pagamento já iniciado"));
            return null;
        }


        var valorTotal = command.Itens.Sum(item => item.PrecoUnitario * item.Quantidade);
        
        var novoPagamento = new Pagamento(command.PedidoId, command.MetodoDePagamento, valorTotal, null);

        var fornecedorResponse = await fornecedorPagamentoGateway.IniciarPagamento(command);

        novoPagamento.AssociarPagamentoExterno(fornecedorResponse.IdExterno, fornecedorResponse.UrlPagamento);
        novoPagamento = await pagamentoGateway.CreateAsync(novoPagamento);

        return novoPagamento;
    }
}