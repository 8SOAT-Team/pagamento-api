using Moq;
using Xunit;
using Pagamentos.Adapters.Controllers;
using Pagamentos.Adapters.Presenters;
using Pagamentos.Apps.UseCases;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Domain.Entities;
using CleanArch.UseCase.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pagamentos.Adapters.Types;
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
        var pagamento = new Pagamentos.Domain.Entities.Pagamento(pagamentoId, MetodoDePagamento.Pix, 100m, "idExterno");
        _mockPagamentoGateway.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Pagamentos.Domain.Entities.Pagamento(pagamentoId, MetodoDePagamento.Pix, 100m, "idExterno"));

        var useCaseResult = Any<Pagamentos.Domain.Entities.Pagamento>.Some(new Pagamentos.Domain.Entities.Pagamento(pagamentoId, MetodoDePagamento.Pix, 100m, "idExterno"));

        var useCase = new ConfirmarPagamentoUseCase(_mockLogger.Object, _mockPagamentoGateway.Object);
        

        // Act
        var result = await _controller.ConfirmarPagamento(pagamentoId, status);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<PagamentoResponseDTO>>(result);
        Assert.Equal(pagamentoId, result.Value.Id);  // Certifique-se que o valor retornado seja o esperado
        
    }

    [Fact]
    public async Task GetPagamentoByPedidoAsync_DeveRetornarListaDePagamentos()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pagamentoList = new List<Pagamentos.Domain.Entities.Pagamento>
        {
            new Pagamentos.Domain.Entities.Pagamento(pedidoId, MetodoDePagamento.Pix, 100m, "idExterno")
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

        var pagamento = new Pagamentos.Domain.Entities.Pagamento(pedidoId, MetodoDePagamento.Master, valorTotal, "idExterno");
        var useCaseResult = Any<Pagamentos.Domain.Entities.Pagamento>.Some(pagamento);
        var useCase = new IniciarPagamentoUseCase(_mockLogger.Object, _mockPagamentoGateway.Object, _mockFornecedorPagamentoGateway.Object);

        _mockPagamentoGateway.Setup(x => x.GetByIdAsync(pedidoId)).ReturnsAsync(pagamento);
        _mockFornecedorPagamentoGateway.Setup(x => x.IniciarPagamento(It.IsAny<MetodoDePagamento>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(new FornecedorCriarPagamentoResponseDto("idExterno", "urlPagamento" ));

        // Act
        var result = await _controller.IniciarPagamento(pedidoId, metodoDePagamento, valorTotal, emailPagador, webhookUrl, token);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Result<PagamentoResponseDTO>>(result);
        Assert.Equal(pedidoId, result.Value.Id);
    }

    
}
