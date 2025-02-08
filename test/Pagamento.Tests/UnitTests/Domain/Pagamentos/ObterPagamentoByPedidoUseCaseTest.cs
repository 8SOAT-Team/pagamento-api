using Moq;
using CleanArch.UseCase.Logging;
using Pagamento.Apps.UseCases;
using Pagamento.Domain.Entities;


namespace Tests.UnitTests.Domain.Pagamentos;
public class ObterPagamentoByPedidoUseCaseTest
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<IPagamentoGateway> _mockPagamentoGateway;
    private readonly ObterPagamentoByPedidoUseCaseTests _useCase;

    public ObterPagamentoByPedidoUseCaseTest()
    {
        _mockLogger = new Mock<ILogger>();
        _mockPagamentoGateway = new Mock<IPagamentoGateway>();
        _useCase = new ObterPagamentoByPedidoUseCaseTests(_mockLogger.Object, _mockPagamentoGateway.Object);
    }

    [Fact]
    public async Task Execute_DeveRetornarPagamentosQuandoEncontrarPagamentos()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido> { itemPedido };
        var pedido = new Pedido(pedidoId, Guid.NewGuid(), listaItens);
        var pagamentos = new List<Pagamento.Domain.Entities.Pagamento>
            {
                new Pagamento.Domain.Entities.Pagamento(pedido.Id,MetodoDePagamento.Pix, 100.0m, "idExterno"),
                new Pagamento.Domain.Entities.Pagamento( pedido.Id,MetodoDePagamento.Pix, 50.0m, "idExterno")
            };

        _mockPagamentoGateway.Setup(gateway => gateway.FindPagamentoByPedidoIdAsync(pedidoId))
            .ReturnsAsync(pagamentos);

        // Act
        var result = await _useCase.Execute(pedidoId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(100.0m, result[0].ValorTotal);
        Assert.Equal(50.0m, result[1].ValorTotal);
    }

    [Fact]
    public async Task Execute_DeveRetornarNullQuandoNaoEncontrarPagamentos()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        _mockPagamentoGateway.Setup(gateway => gateway.FindPagamentoByPedidoIdAsync(pedidoId))
            .ReturnsAsync(new List<Pagamento.Domain.Entities.Pagamento>());

        // Act
        var result = await _useCase.Execute(pedidoId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Execute_DeveRetornarNullQuandoGatewayRetornarNull()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        _mockPagamentoGateway.Setup(gateway => gateway.FindPagamentoByPedidoIdAsync(pedidoId))
            .ReturnsAsync((List<Pagamento.Domain.Entities.Pagamento>?)null);

        // Act
        var result = await _useCase.Execute(pedidoId);

        // Assert
        Assert.Null(result);
    }

}


public class ObterPagamentoByPedidoUseCaseTests : ObterPagamentoByPedidoUseCase
{
    public ObterPagamentoByPedidoUseCaseTests(ILogger logger, IPagamentoGateway pagamentoGateway)
        : base(logger, pagamentoGateway) { }

    public new Task<List<Pagamento.Domain.Entities.Pagamento>?> Execute(Guid pedidoId)
    {
        return base.Execute(pedidoId);
    }

}