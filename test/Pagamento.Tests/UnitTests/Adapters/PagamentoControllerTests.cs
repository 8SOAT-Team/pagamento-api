using Moq;
using Pagamento.Adapters.Controllers;
using Pagamento.Apps.UseCases;
using Pagamento.Apps.UseCases.Dtos;
using Pagamento.Domain.Entities;
using CleanArch.UseCase.Logging;
using Pagamento.Adapters.Types;
using CleanArch.UseCase.Options;

namespace Tests.UnitTests.Adapters;

public class PagamentoControllerTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<IPagamentoGateway> _mockPagamentoGateway;
    private readonly Mock<IFornecedorPagamentoGateway> _mockFornecedorPagamentoGateway;
    private readonly PagamentoController _controller;

    public PagamentoControllerTests()
    {
        _mockLogger = new Mock<ILogger>();
        _mockPagamentoGateway = new Mock<IPagamentoGateway>();
        _mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        _controller = new PagamentoController(_mockLogger.Object, _mockPagamentoGateway.Object, _mockFornecedorPagamentoGateway.Object);
    }

    [Fact]
    public async Task ConfirmarPagamento_DeveRetornarResultadoCorreto()
    {
        // Arrange
        var pagamentoId = Guid.NewGuid();
        var status = StatusDoPagamento.Autorizado;
        var pagamento = new Pagamento.Domain.Entities.Pagamento(pagamentoId, MetodoDePagamento.Pix, 100m, "idExterno");
        _mockPagamentoGateway.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Pagamento.Domain.Entities.Pagamento(pagamentoId, MetodoDePagamento.Pix, 100m, "idExterno"));

        var useCaseResult = Any<Pagamento.Domain.Entities.Pagamento>.Some(new Pagamento.Domain.Entities.Pagamento(pagamentoId, MetodoDePagamento.Pix, 100m, "idExterno"));

        var useCase = new ConfirmarPagamentoUseCase(_mockLogger.Object, _mockPagamentoGateway.Object);
        

        // Act
        var result = await _controller.ConfirmarPagamento(pagamentoId, status);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<PagamentoResponseDTO>>(result);
               
    }

    [Fact]
    public async Task GetPagamentoByPedidoAsync_DeveRetornarListaDePagamentos()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pagamentoList = new List<Pagamento.Domain.Entities.Pagamento>
        {
            new Pagamento.Domain.Entities.Pagamento(pedidoId, MetodoDePagamento.Pix, 100m, "idExterno")
        };

        _mockPagamentoGateway.Setup(x => x.FindPagamentoByPedidoIdAsync(pedidoId)).ReturnsAsync(pagamentoList);

        // Act
        var result = await _controller.GetPagamentoByPedidoAsync(pedidoId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<List<PagamentoResponseDTO>>>(result);
        Assert.Single(result.Value);  // Verifica se a lista tem um único item
    }

    [Fact]
    public async Task IniciarPagamento_DeveRetornarResultadoCorreto()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var metodoDePagamento = MetodosDePagamento.Master;
        var valorTotal = 150m;
        var emailPagador = "email@example.com";
        var webhookUrl = "http://webhook.url";
        var token = "token123";
        var dto = new IniciarPagamentoDto(pedidoId, MetodoDePagamento.Master, valorTotal, emailPagador, webhookUrl, token);

        var pagamento = new Pagamento.Domain.Entities.Pagamento(pedidoId, MetodoDePagamento.Master, valorTotal, "idExterno");
        var useCaseResult = Any<Pagamento.Domain.Entities.Pagamento>.Some(pagamento);
        var useCase = new IniciarPagamentoUseCase(_mockLogger.Object, _mockPagamentoGateway.Object, _mockFornecedorPagamentoGateway.Object);

        _mockPagamentoGateway.Setup(x => x.GetByIdAsync(pedidoId)).ReturnsAsync(pagamento);
        _mockFornecedorPagamentoGateway.Setup(x => x.IniciarPagamento(It.IsAny<MetodoDePagamento>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(new FornecedorCriarPagamentoResponseDto("idExterno", "urlPagamento" ));

        // Act
        var result = await _controller.IniciarPagamento(pedidoId, metodoDePagamento, valorTotal, emailPagador, webhookUrl, token);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<PagamentoResponseDTO>>(result);        
    }

    
}
