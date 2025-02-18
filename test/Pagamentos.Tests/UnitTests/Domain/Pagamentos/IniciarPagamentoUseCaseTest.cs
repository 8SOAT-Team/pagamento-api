using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Pagamentos.Apps.UseCases;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Domain.Entities;
using Pagamentos.Tests.IntegrationTests.Builder;

namespace Pagamentos.Tests.UnitTests.Domain.Pagamentos;

public sealed class IniciarPagamentoUseCaseTest
{
    private readonly Mock<IPagamentoGateway> _mockPagamentoGateway;
    private readonly Mock<IFornecedorPagamentoGateway> _mockFornecedorPagamentoGateway;
    private readonly IniciarPagamentoUseCase _useCase;
    private readonly Faker _faker = new();

    public IniciarPagamentoUseCaseTest()
    {
        Mock<ILogger<IniciarPagamentoUseCase>> mockLogger = new();
        _mockPagamentoGateway = new Mock<IPagamentoGateway>();
        _mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        _useCase = new IniciarPagamentoUseCase(mockLogger.Object, _mockPagamentoGateway.Object,
            _mockFornecedorPagamentoGateway.Object);
    }

    [Fact]
    public async Task Execute_DeveIniciarPagamentoQuandoPedidoValido()
    {
        // Arrange
        var comando = new IniciarPagamentoDtoBuilder().Generate();
        _mockPagamentoGateway.Setup(g => g.FindPagamentoByPedidoIdAsync(comando.PedidoId))
            .ReturnsAsync([]);

        var fornecedorPagamento =
            new FornecedorCriarPagamentoResponseDto(_faker.Random.Guid().ToString(), _faker.Internet.Url());
        _mockFornecedorPagamentoGateway.Setup(g => g.IniciarPagamento(comando, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fornecedorPagamento);

        var pagamento = PagamentoBuilder.CreateBuilder()
            .ComPedidoId(comando.PedidoId)
            .ComPagamentoExternoId(fornecedorPagamento.IdExterno)
            .ComPagamentoUrl(fornecedorPagamento.UrlPagamento)
            .Generate();
        _mockPagamentoGateway.Setup(x => x.CreateAsync(It.Is<Pagamento>(x => x.PedidoId == comando.PedidoId)))
            .ReturnsAsync(pagamento);

        // Act
        var result = await _useCase.ResolveAsync(comando);

        // Assert
        Assert.NotNull(result.Value);
        result.Value!.Status.Should().Be(StatusPagamento.Pendente);
        result.Value.PagamentoExternoId.Should().Be(fornecedorPagamento.IdExterno);
        result.Value.UrlPagamento.Should().Be(fornecedorPagamento.UrlPagamento);

        _mockFornecedorPagamentoGateway.Verify(p => p.IniciarPagamento(
            It.Is<IniciarPagamentoDto>(x => x.PedidoId == comando.PedidoId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Execute_DeveRetornarErroQuandoPagamentoJaIniciado()
    {
        // Arrange
        var comando = new IniciarPagamentoDtoBuilder().Generate();
        _mockPagamentoGateway.Setup(g => g.FindPagamentoByPedidoIdAsync(comando.PedidoId))
            .ReturnsAsync([PagamentoBuilder.Build()]);

        // Act
        var result = await _useCase.ResolveAsync(comando);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeNull();

        var useCaseErrors = _useCase.GetErrors();
        Assert.Single(useCaseErrors);
        Assert.Equal("Pagamento já iniciado", useCaseErrors.FirstOrDefault().Description);
    }
}