using CleanArch.UseCase.Faults;
using CleanArch.UseCase.Logging;
using Moq;
using Pagamento.Apps.UseCases;
using Pagamento.Apps.UseCases.Dtos;
using Pagamento.Domain.Entities;


namespace Tests.UnitTests.Domain.Pagamentos;
public class ConfirmarStatusPagamentoUseCaseTest
{

    [Fact]
    public async Task Execute_DeveAdicionarErroQuandoPagamentoExternoNaoEncontrado()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();

        var pagamentoExternoId = "PagamentoExternoId";

        mockFornecedorPagamentoGateway.Setup(g => g.ObterPagamento(pagamentoExternoId,It.IsAny<string>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync((FornecedorGetPagamentoResponseDto?)null);

        var useCase = new ConfirmarStatusPagamentoTest(mockLogger.Object, mockFornecedorPagamentoGateway.Object, mockPagamentoGateway.Object);

        // Act
        var resultado = await useCase.Execute(pagamentoExternoId);

        // Assert
        Assert.Null(resultado);
        IReadOnlyCollection<UseCaseError> useCaseErrors = useCase.GetErrors();
        Assert.Single(useCaseErrors);
        Assert.Equal("Pagamento Externo não encontrado", useCaseErrors.FirstOrDefault().Description);
    }

    [Fact]
    public async Task Execute_DeveAdicionarErroQuandoPagamentoNaoEncontrado()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();

        var pagamentoExternoId = "PagamentoExternoId";
        var pagamentoId = Guid.NewGuid();
        var pagamentoExterno = new FornecedorGetPagamentoResponseDto(pagamentoExternoId, pagamentoId, StatusPagamento.Autorizado);


        mockFornecedorPagamentoGateway.Setup(g => g.ObterPagamento(pagamentoExternoId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagamentoExterno);

        mockPagamentoGateway.Setup(g => g.GetByIdAsync(pagamentoExterno.PagamentoId)).ReturnsAsync((Pagamento.Domain.Entities.Pagamento?)null);

        var useCase = new ConfirmarStatusPagamentoTest(mockLogger.Object, mockFornecedorPagamentoGateway.Object, mockPagamentoGateway.Object);

        // Act
        var resultado = await useCase.Execute(pagamentoExternoId);

        // Assert
        Assert.Null(resultado);
        IReadOnlyCollection<UseCaseError> useCaseErrors = useCase.GetErrors();
        Assert.Single(useCaseErrors);
        Assert.Equal("Pagamento não encontrado", useCaseErrors.FirstOrDefault().Description);
    }

    [Fact]
    public async Task Execute_DeveRetornarPagamentoQuandoStatusPendente()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();

        var pagamentoExternoId = "PagamentoExternoId";
        var pagamentoId = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();

        var pagamentoExterno = new FornecedorGetPagamentoResponseDto(pagamentoExternoId, pagamentoId, StatusPagamento.Pendente);

        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido> { itemPedido };
        var pedido = new Pedido(pedidoId, Guid.NewGuid(), listaItens);

        var pagamento = new Pagamento.Domain.Entities.Pagamento(pedido.Id, MetodoDePagamento.Pix, 100m, "idExterno");

        mockFornecedorPagamentoGateway.Setup(g => g.ObterPagamento(pagamentoExternoId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagamentoExterno);

        mockPagamentoGateway.Setup(g => g.GetByIdAsync(pagamentoId)).ReturnsAsync(pagamento);

        var useCase = new ConfirmarStatusPagamentoTest(mockLogger.Object, mockFornecedorPagamentoGateway.Object, mockPagamentoGateway.Object);

        // Act
        var resultado = await useCase.Execute(pagamentoExternoId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(pagamentoId, resultado.Id);
        mockPagamentoGateway.Verify(g => g.UpdatePagamentoAsync(It.IsAny<Pagamento.Domain.Entities.Pagamento>()), Times.Never);
    }

    [Fact]
    public async Task Execute_DeveFinalizarEPagarQuandoStatusAutorizado()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();

        var pagamentoExternoId = "PagamentoExternoId";
        var pagamentoId = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();

        var pagamentoExterno = new FornecedorGetPagamentoResponseDto(pagamentoExternoId, pagamentoId, StatusPagamento.Autorizado);

        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido> { itemPedido };
        var pedido = new Pedido(pedidoId, Guid.NewGuid(), listaItens);

        var pagamento = new Pagamento.Domain.Entities.Pagamento(pedido.Id,MetodoDePagamento.Pix, 100m, "idExterno");

        mockFornecedorPagamentoGateway.Setup(g => g.ObterPagamento(pagamentoExternoId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagamentoExterno);

        mockPagamentoGateway.Setup(g => g.GetByIdAsync(pagamentoId)).ReturnsAsync(pagamento);

        mockPagamentoGateway.Setup(g => g.UpdatePagamentoAsync(It.IsAny<Pagamento.Domain.Entities.Pagamento>())).ReturnsAsync(pagamento);

        var useCase = new ConfirmarStatusPagamentoTest(mockLogger.Object, mockFornecedorPagamentoGateway.Object, mockPagamentoGateway.Object);

        // Act
        var resultado = await useCase.Execute(pagamentoExternoId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(pagamentoId, resultado.Id);
        mockPagamentoGateway.Verify(g => g.UpdatePagamentoAsync(It.IsAny<Pagamento.Domain.Entities.Pagamento>()), Times.Once);
    }

}


public class ConfirmarStatusPagamentoTest : ConfirmarStatusPagamentoUseCase
{
    public ConfirmarStatusPagamentoTest(ILogger logger,
    IFornecedorPagamentoGateway fornecedorPagamentoGateway,
    IPagamentoGateway pagamentoGateway)
        : base("token",logger, fornecedorPagamentoGateway, pagamentoGateway) { }

    public new Task<Pagamento.Domain.Entities.Pagamento?> Execute(string pagamentoExternoId)
    {
        return base.Execute(pagamentoExternoId);
    }

}