using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Pagamentos.Apps.UseCases;
using Pagamentos.Tests.IntegrationTests.Builder;

namespace Pagamentos.Tests.UnitTests.Domain.Pagamentos;

public class ObterPagamentoByPedidoUseCaseTest
{
    private readonly Mock<IPagamentoGateway> _mockPagamentoGateway;
    private readonly ObterPagamentoByPedidoUseCase _useCase;

    public ObterPagamentoByPedidoUseCaseTest()
    {
        _mockPagamentoGateway = new Mock<IPagamentoGateway>();
        _useCase = new ObterPagamentoByPedidoUseCase(new Mock<ILogger<ObterPagamentoByPedidoUseCase>>().Object,
            _mockPagamentoGateway.Object);
    }

    [Fact]
    public async Task Execute_DeveRetornarPagamentosQuandoEncontrarPagamentos()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Build();

        _mockPagamentoGateway.Setup(gateway => gateway.FindPagamentoByPedidoIdAsync(pagamento.PedidoId))
            .ReturnsAsync([pagamento]);

        // Act
        var result = await _useCase.ResolveAsync(pagamento.PedidoId);

        // Assert
        result.Should().NotBeNull();
        result.HasValue.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.Should().ContainSingle(p => p.Id == pagamento.Id);
    }

    [Fact]
    public async Task Execute_DeveRetornarNullQuandoNaoEncontrarPagamentos()
    {
        // Arrange
        _mockPagamentoGateway.Setup(gateway => gateway.FindPagamentoByPedidoIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // Act
        var result = await _useCase.ResolveAsync(Guid.NewGuid());

        // Assert
        result.Should().NotBeNull();
        result.HasValue.Should().BeFalse();
        result.Value.Should().BeNull();
    }
}