using CleanArch.UseCase.Options;
using Microsoft.Extensions.Logging;
using Moq;
using Pagamentos.Adapters.Controllers;
using Pagamentos.Adapters.Types;
using Pagamentos.Apps.UseCases;
using Pagamentos.Domain.Entities;
using Pagamentos.Tests.IntegrationTests.Builder;

namespace Pagamentos.Tests.UnitTests.Adapters;

public class PagamentoControllerTests
{
    private readonly Mock<ILoggerFactory> _mockLogger;
    private readonly Mock<IPagamentoGateway> _mockPagamentoGateway;
    private readonly Mock<IFornecedorPagamentoGateway> _mockFornecedorPagamentoGateway;
    private readonly PagamentoController _controller;

    public PagamentoControllerTests()
    {
        _mockLogger = new Mock<ILoggerFactory>();
        _mockPagamentoGateway = new Mock<IPagamentoGateway>();
        _mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        _controller = new PagamentoController(_mockLogger.Object, _mockPagamentoGateway.Object,
            _mockFornecedorPagamentoGateway.Object);
    }

    [Fact]
    public async Task ConfirmarPagamento_DeveRetornarResultadoCorreto()
    {
        // Arrange
        var pagamentoId = Guid.NewGuid();
        var status = StatusDoPagamento.Autorizado;
        _mockPagamentoGateway.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Pagamento(pagamentoId, MetodoDePagamento.Pix, 100m, "idExterno"));
        
        _mockLogger.Setup(x => x.CreateLogger<ConfirmarPagamentoUseCase>())
            .Returns(new Mock<ILogger<ConfirmarPagamentoUseCase>>().Object);
        
        // Act
        var result = await _controller.ConfirmarPagamento(pagamentoId, status);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<PagamentoResponseDTO>>(result);
        Assert.Equal(pagamentoId, result.Value.Id);
    }

    [Fact]
    public async Task GetPagamentoByPedidoAsync_DeveRetornarListaDePagamentos()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pagamentoList = new List<Pagamento>
        {
            new(pedidoId, MetodoDePagamento.Pix, 100m, "idExterno")
        };

        _mockLogger.Setup(x => x.CreateLogger<ObterPagamentoByPedidoUseCase>())
            .Returns(new Mock<ILogger<ObterPagamentoByPedidoUseCase>>().Object);
        
        _mockPagamentoGateway.Setup(x => x.FindPagamentoByPedidoIdAsync(pedidoId)).ReturnsAsync(pagamentoList);

        // Act
        var result = await _controller.GetPagamentoByPedidoAsync(pedidoId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<List<PagamentoResponseDTO>>>(result);
        Assert.Single(result.Value); // Verifica se a lista tem um único item
    }

    [Fact]
    public async Task IniciarPagamento_DeveRetornarResultadoCorreto()
    {
        // Arrange
        var dto = new IniciarPagamentoDtoBuilder().Generate();
        var valorTotal = dto.Itens.Sum(x => x.PrecoUnitario * x.Quantidade);
        
        var pagamento =
            new Pagamento(Guid.NewGuid(), dto.PedidoId, MetodoDePagamento.Master, valorTotal, "idExterno");
        var useCaseResult = Any<Pagamento>.Some(pagamento);
        var useCase = new Mock<IniciarPagamentoUseCase>();
        useCase.Setup(x => x.ResolveAsync(dto)).ReturnsAsync(useCaseResult);
        
        // Act
        var result =
            await _controller.IniciarPagamento(dto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<PagamentoResponseDTO>>(result);
        Assert.Equal(dto.PedidoId, result.Value!.PedidoId);
    }
}