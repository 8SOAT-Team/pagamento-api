using Pagamento.Adapters.Presenters;
using Pagamento.Adapters.Types;
using Pagamento.Apps.UseCases;
using Pagamento.Apps.UseCases.Dtos;
using Pagamento.Domain.Entities;
using ILogger = CleanArch.UseCase.Logging.ILogger;

namespace Pagamento.Adapters.Controllers;

public class PagamentoController(
    ILogger logger,
    IPedidoGateway pedidoGateway,
    IPagamentoGateway pagamentoGateway,
    IFornecedorPagamentoGateway fornecedorPagamentoGateway) : IPagamentoController
{
    private readonly ILogger _logger = logger;
    private readonly IPedidoGateway _pedidoGateway = pedidoGateway;
    private readonly IPagamentoGateway _pagamentoGateway = pagamentoGateway;
    private readonly IFornecedorPagamentoGateway _fornecedorPagamentoGateway = fornecedorPagamentoGateway;

    public async Task<Result<PagamentoResponseDTO>> ConfirmarPagamento(Guid pagamentoId, StatusDoPagamento status)
    {
        var useCase = new ConfirmarPagamentoUseCase(_logger, _pagamentoGateway, _pedidoGateway);
        var useCaseResult = await useCase.ResolveAsync(new ConfirmarPagamentoDto(pagamentoId, (StatusPagamento)status));

        return ControllerResultBuilder<PagamentoResponseDTO, Domain.Entities.Pagamento>
           .ForUseCase(useCase)
           .WithInstance(pagamentoId)
           .WithResult(useCaseResult)
           .AdaptUsing(PagamentoPresenter.ToPagamentoDTO)
           .Build();
    }

    public async Task<Result<List<PagamentoResponseDTO>>> GetPagamentoByPedidoAsync(Guid pedidoId)
    {
        var useCase = new ObterPagamentoByPedidoUseCase(_logger, _pagamentoGateway);
        var useCaseResult = await useCase.ResolveAsync(pedidoId);

        return ControllerResultBuilder<List<PagamentoResponseDTO>, List<Domain.Entities.Pagamento>>
           .ForUseCase(useCase)
           .WithInstance(pedidoId)
           .WithResult(useCaseResult)
           .AdaptUsing(PagamentoPresenter.ToListPagamentoDTO)
           .Build();
    }

    public async Task<Result<PagamentoResponseDTO>> IniciarPagamento(Guid pedidoId, MetodosDePagamento metodoDePagamento)
    {
        var useCase = new IniciarPagamentoUseCase(_logger, _pedidoGateway, _pagamentoGateway, _fornecedorPagamentoGateway);
        var useCaseResult = await useCase.ResolveAsync(new IniciarPagamentoDto(pedidoId, (MetodoDePagamento)metodoDePagamento));

        return ControllerResultBuilder<PagamentoResponseDTO, Domain.Entities.Pagamento>
           .ForUseCase(useCase)
           .WithInstance(pedidoId)
           .WithResult(useCaseResult)
           .AdaptUsing(PagamentoPresenter.ToPagamentoDTO)
           .Build();
    }

    public async Task<Result<PagamentoResponseDTO>> ReceberWebhookPagamento(string pagamentoExternoId)
    {
        var useCase = new ConfirmarStatusPagamentoUseCase(_logger, _fornecedorPagamentoGateway, _pagamentoGateway);
        var useCaseResult = await useCase.ResolveAsync(pagamentoExternoId);

        return ControllerResultBuilder<PagamentoResponseDTO, Domain.Entities.Pagamento>
           .ForUseCase(useCase)
           .WithInstance(pagamentoExternoId)
           .WithResult(useCaseResult)
           .AdaptUsing(PagamentoPresenter.ToPagamentoDTO)
           .Build();
    }
}
