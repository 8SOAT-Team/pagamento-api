using Pagamentos.Apps.UseCases;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Adapters.Presenters;
using Pagamentos.Apps.Handlers;
using Pagamentos.Apps.Types;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Adapters.Controllers;

public class PagamentoController(
    ILoggerFactory loggerFactory,
    IPagamentoGateway pagamentoGateway,
    IFornecedorPagamentoGateway fornecedorPagamentoGateway,
    IPagamentoHandler pagamentoHandler) : IPagamentoController
{
    public async Task<Result<PagamentoResponseDto>> ConfirmarPagamento(Guid pagamentoId, StatusDoPagamento status)
    {
        var useCase =
            new ConfirmarPagamentoUseCase(loggerFactory.CreateLogger<ConfirmarPagamentoUseCase>(), pagamentoGateway);
        var useCaseResult = await useCase.ResolveAsync(new ConfirmarPagamentoDto(pagamentoId, (StatusPagamento)status));

        return ControllerResultBuilder<PagamentoResponseDto, Pagamento>
            .ForUseCase(useCase)
            .WithInstance(pagamentoId)
            .WithResult(useCaseResult)
            .AdaptUsing(PagamentoPresenter.ToPagamentoDto)
            .Build();
    }

    public async Task<Result<List<PagamentoResponseDto>>> GetPagamentoByPedidoAsync(Guid pedidoId)
    {
        var useCase = new ObterPagamentoByPedidoUseCase(loggerFactory.CreateLogger<ObterPagamentoByPedidoUseCase>(),
            pagamentoGateway);
        var useCaseResult = await useCase.ResolveAsync(pedidoId);

        return ControllerResultBuilder<List<PagamentoResponseDto>, List<Pagamento>>
            .ForUseCase(useCase)
            .WithInstance(pedidoId)
            .WithResult(useCaseResult)
            .AdaptUsing(PagamentoPresenter.ToListPagamentoDto)
            .Build();
    }

    public async Task<Result<PagamentoResponseDto>> IniciarPagamento(IniciarPagamentoDto iniciarPagamentoDto)
    {
        var useCase = new IniciarPagamentoUseCase(loggerFactory.CreateLogger<IniciarPagamentoUseCase>(),
            pagamentoGateway, fornecedorPagamentoGateway);
        var useCaseResult = await useCase.ResolveAsync(iniciarPagamentoDto);

        return ControllerResultBuilder<PagamentoResponseDto, Pagamento>
            .ForUseCase(useCase)
            .WithInstance(iniciarPagamentoDto.PedidoId)
            .WithResult(useCaseResult)
            .AdaptUsing(PagamentoPresenter.ToPagamentoDto)
            .Build();
    }

    public async Task<Result<PagamentoResponseDto>> ReceberWebhookPagamento(string pagamentoExternoId)
    {
        var useCase =
            new ConfirmarStatusPagamentoUseCase(loggerFactory.CreateLogger<ConfirmarStatusPagamentoUseCase>(),
                fornecedorPagamentoGateway, pagamentoGateway, pagamentoHandler);
        var useCaseResult = await useCase.ResolveAsync(pagamentoExternoId);

        return ControllerResultBuilder<PagamentoResponseDto, Pagamento>
            .ForUseCase(useCase)
            .WithInstance(pagamentoExternoId)
            .WithResult(useCaseResult)
            .AdaptUsing(PagamentoPresenter.ToPagamentoDto)
            .Build();
    }
}