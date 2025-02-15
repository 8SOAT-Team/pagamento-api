using Pagamentos.Apps.UseCases;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Adapters.Presenters;
using Pagamentos.Adapters.Types;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Adapters.Controllers;

public class PagamentoController(
    ILoggerFactory loggerFactory,
    IPagamentoGateway pagamentoGateway,
    IFornecedorPagamentoGateway fornecedorPagamentoGateway) : IPagamentoController
{
    public async Task<Result<PagamentoResponseDTO>> ConfirmarPagamento(Guid pagamentoId, StatusDoPagamento status)
    {
        var useCase =
            new ConfirmarPagamentoUseCase(loggerFactory.CreateLogger<ConfirmarPagamentoUseCase>(), pagamentoGateway);
        var useCaseResult = await useCase.ResolveAsync(new ConfirmarPagamentoDto(pagamentoId, (StatusPagamento)status));

        return ControllerResultBuilder<PagamentoResponseDTO, Pagamento>
            .ForUseCase(useCase)
            .WithInstance(pagamentoId)
            .WithResult(useCaseResult)
            .AdaptUsing(PagamentoPresenter.ToPagamentoDto)
            .Build();
    }

    public async Task<Result<List<PagamentoResponseDTO>>> GetPagamentoByPedidoAsync(Guid pedidoId)
    {
        var useCase = new ObterPagamentoByPedidoUseCase(loggerFactory.CreateLogger<ObterPagamentoByPedidoUseCase>(),
            pagamentoGateway);
        var useCaseResult = await useCase.ResolveAsync(pedidoId);

        return ControllerResultBuilder<List<PagamentoResponseDTO>, List<Pagamento>>
            .ForUseCase(useCase)
            .WithInstance(pedidoId)
            .WithResult(useCaseResult)
            .AdaptUsing(PagamentoPresenter.ToListPagamentoDto)
            .Build();
    }

    public async Task<Result<PagamentoResponseDTO>> IniciarPagamento(Guid pedidoId,
        MetodosDePagamento metodoDePagamento, decimal valorTotal, string emailPagador)
    {
        var useCase = new IniciarPagamentoUseCase(loggerFactory.CreateLogger<IniciarPagamentoUseCase>(),
            pagamentoGateway, fornecedorPagamentoGateway);
        var useCaseResult = await useCase.ResolveAsync(new IniciarPagamentoDto(pedidoId,
            (MetodoDePagamento)metodoDePagamento, valorTotal, emailPagador));

        return ControllerResultBuilder<PagamentoResponseDTO, Pagamento>
            .ForUseCase(useCase)
            .WithInstance(pedidoId)
            .WithResult(useCaseResult)
            .AdaptUsing(PagamentoPresenter.ToPagamentoDto)
            .Build();
    }

    public async Task<Result<PagamentoResponseDTO>> ReceberWebhookPagamento(string pagamentoExternoId)
    {
        var useCase =
            new ConfirmarStatusPagamentoUseCase(loggerFactory.CreateLogger<ConfirmarStatusPagamentoUseCase>(),
                fornecedorPagamentoGateway, pagamentoGateway);
        var useCaseResult = await useCase.ResolveAsync(pagamentoExternoId);

        return ControllerResultBuilder<PagamentoResponseDTO, Pagamento>
            .ForUseCase(useCase)
            .WithInstance(pagamentoExternoId)
            .WithResult(useCaseResult)
            .AdaptUsing(PagamentoPresenter.ToPagamentoDto)
            .Build();
    }
}