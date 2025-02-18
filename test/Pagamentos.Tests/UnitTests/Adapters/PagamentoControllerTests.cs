using Bogus;
using CleanArch.UseCase.Options;
using Microsoft.Extensions.Logging;
using Moq;
using Pagamentos.Adapters.Controllers;
using Pagamentos.Adapters.Types;
using Pagamentos.Apps.UseCases;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Domain.Entities;
using Pagamentos.Tests.IntegrationTests.Builder;

namespace Pagamentos.Tests.UnitTests.Adapters;

public class PagamentoControllerTests
{
    private readonly Mock<ILoggerFactory> _mockLogger;
    private readonly Mock<IPagamentoGateway> _mockPagamentoGateway;
    private readonly Mock<IFornecedorPagamentoGateway> _mockFornecedorPagamentoGateway;
    private readonly PagamentoController _controller;
    private readonly Faker _faker = new();

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
        const StatusDoPagamento status = StatusDoPagamento.Autorizado;
        var pagamento = new Pagamento(pagamentoId, Guid.NewGuid(), MetodoDePagamento.Pix, 100m, "idExterno");
        _mockPagamentoGateway.Setup(x => x.GetByIdAsync(It.Is<Guid>(x => x == pagamentoId)))
            .ReturnsAsync(pagamento);

        _mockPagamentoGateway.Setup(x => x.UpdatePagamentoAsync(It.Is<Pagamento>(x => x.Id == pagamento.Id)))
            .ReturnsAsync(pagamento);

        _mockLogger.Setup(x => x.CreateLogger(typeof(ConfirmarPagamentoUseCase).FullName!))
            .Returns(new Mock<ILogger<ConfirmarPagamentoUseCase>>().Object);


        // Act
        var result = await _controller.ConfirmarPagamento(pagamentoId, status);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<PagamentoResponseDto>>(result);
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

        _mockLogger.Setup(x => x.CreateLogger(typeof(ObterPagamentoByPedidoUseCase).FullName!))
            .Returns(new Mock<ILogger<ObterPagamentoByPedidoUseCase>>().Object);

        _mockPagamentoGateway.Setup(x => x.FindPagamentoByPedidoIdAsync(pedidoId)).ReturnsAsync(pagamentoList);

        // Act
        var result = await _controller.GetPagamentoByPedidoAsync(pedidoId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<List<PagamentoResponseDto>>>(result);
        Assert.Single(result.Value); // Verifica se a lista tem um único item
    }

    [Fact]
    public async Task IniciarPagamento_DeveRetornarResultadoCorreto()
    {
        // Arrange
        var dto = new IniciarPagamentoDtoBuilder().Generate();

        _mockPagamentoGateway.Setup(x => x.FindPagamentoByPedidoIdAsync(dto.PedidoId))
            .ReturnsAsync([]);

        var fornecedorResponse =
            new FornecedorCriarPagamentoResponseDto(Guid.NewGuid().ToString(), _faker.Internet.Url());
        _mockFornecedorPagamentoGateway
            .Setup(x => x.IniciarPagamento(It.Is<IniciarPagamentoDto>(x => x.PedidoId == dto.PedidoId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(fornecedorResponse);

        _mockPagamentoGateway.Setup(x => x.CreateAsync(It.Is<Pagamento>(x => x.PedidoId == dto.PedidoId)))
            .ReturnsAsync(new Pagamento(dto.PedidoId, MetodoDePagamento.Pix, 100m, fornecedorResponse.IdExterno));

        _mockLogger.Setup(x => x.CreateLogger(typeof(IniciarPagamentoUseCase).FullName!))
            .Returns(new Mock<ILogger<IniciarPagamentoUseCase>>().Object);

        // Act
        var result = await _controller.IniciarPagamento(dto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<PagamentoResponseDto>>(result);
        Assert.Equal(dto.PedidoId, result.Value!.PedidoId);
    }
}