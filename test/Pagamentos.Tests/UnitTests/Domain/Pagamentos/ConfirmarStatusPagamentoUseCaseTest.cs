using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Pagamentos.Apps.Gateways;
using Pagamentos.Apps.Handlers;
using Pagamentos.Apps.Types;
using Pagamentos.Apps.UseCases;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Domain.Entities;
using Pagamentos.Domain.Entities.DomainEvents;
using Pagamentos.Tests.IntegrationTests.Builder;

namespace Pagamentos.Tests.UnitTests.Domain.Pagamentos;

public class ConfirmarStatusPagamentoUseCaseTest
{
    private readonly Faker _faker = new();

    private readonly ILogger<ConfirmarStatusPagamentoUseCase> _mockLogger =
        new Mock<ILogger<ConfirmarStatusPagamentoUseCase>>().Object;

    private readonly Mock<IPagamentoHandler> _pagamentoHandler;
    private readonly Mock<IPedidoGateway> _pedidoGateway;

    public ConfirmarStatusPagamentoUseCaseTest()
    {
        _pagamentoHandler = new Mock<IPagamentoHandler>();
        _pedidoGateway = new Mock<IPedidoGateway>();
    }

    [Fact]
    public async Task Execute_DeveAdicionarErroQuandoPagamentoExternoNaoEncontrado()
    {
        // Arrange
        var mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();
        var pagamentoExternoId = _faker.Random.Guid().ToString();
        var pagamento = PagamentoBuilder.CreateBuilder().ComPagamentoExternoId(pagamentoExternoId).Generate();

        mockPagamentoGateway.Setup(g => g.FindPagamentoByExternoIdAsync(pagamentoExternoId))
            .ReturnsAsync(pagamento);

        mockFornecedorPagamentoGateway
            .Setup(g => g.ObterPagamento(pagamento.PagamentoExternoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FornecedorGetPagamentoResponseDto?)null);

        var useCase = new ConfirmarStatusPagamentoUseCase(_mockLogger, mockFornecedorPagamentoGateway.Object,
            mockPagamentoGateway.Object, _pagamentoHandler.Object);

        // Act
        var resultado = await useCase.ResolveAsync(pagamentoExternoId);

        // Assert
        Assert.Null(resultado.Value);
        var useCaseErrors = useCase.GetErrors();
        Assert.Single(useCaseErrors);
        Assert.Equal("Pagamento Externo não encontrado", useCaseErrors.FirstOrDefault().Description);
    }

    [Fact]
    public async Task Execute_DeveAdicionarErroQuandoPagamentoNaoEncontrado()
    {
        // Arrange
        var mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();
        var pagamentoExternoId = _faker.Random.Guid().ToString();

        mockPagamentoGateway.Setup(g => g.FindPagamentoByExternoIdAsync(pagamentoExternoId))
            .ReturnsAsync((Pagamento?)null);

        var useCase = new ConfirmarStatusPagamentoUseCase(_mockLogger, mockFornecedorPagamentoGateway.Object,
            mockPagamentoGateway.Object, _pagamentoHandler.Object);

        // Act
        var resultado = await useCase.ResolveAsync(pagamentoExternoId);

        // Assert
        Assert.Null(resultado.Value);
        var useCaseErrors = useCase.GetErrors();
        Assert.Single(useCaseErrors);
        Assert.Equal("Pagamento Externo não encontrado", useCaseErrors.FirstOrDefault().Description);
    }

    [Fact]
    public async Task Execute_DeveRetornarPagamentoQuandoStatusPendente()
    {
        // Arrange
        var mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();
        var pagamentoExternoId = _faker.Random.Guid().ToString();
        var pagamento = PagamentoBuilder.CreateBuilder().ComPagamentoExternoId(pagamentoExternoId).Generate();

        mockPagamentoGateway.Setup(g => g.FindPagamentoByPedidoIdAsync(Guid.Parse(pagamentoExternoId)))
            .ReturnsAsync([pagamento]);

        mockPagamentoGateway.Setup(g => g.UpdatePagamentoAsync(pagamento))
            .ReturnsAsync(pagamento);

        var fornecedorResponse =
            new FornecedorGetPagamentoResponseDto(pagamento.PagamentoExternoId, StatusPagamento.Pendente);
        mockFornecedorPagamentoGateway
            .Setup(g => g.ObterPagamento(pagamento.PagamentoExternoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fornecedorResponse);

        var useCase = new ConfirmarStatusPagamentoUseCase(_mockLogger, mockFornecedorPagamentoGateway.Object,
            mockPagamentoGateway.Object, _pagamentoHandler.Object);

        // Act
        var resultado = await useCase.ResolveAsync(pagamentoExternoId);

        // Assert
        Assert.NotNull(resultado.Value);
        Assert.Equal(pagamento.Id, resultado.Value.Id);
        mockPagamentoGateway.Verify(g => g.UpdatePagamentoAsync(It.IsAny<Pagamento>()), Times.Never);
    }

    [Fact]
    public async Task Execute_DeveFinalizarEPagarQuandoStatusAutorizado()
    {
        // Arrange
        var mockFornecedorPagamentoGateway = new Mock<IFornecedorPagamentoGateway>();
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();
        var pagamentoExternoId = _faker.Random.Guid().ToString();
        var pagamento = PagamentoBuilder.CreateBuilder().ComPagamentoExternoId(pagamentoExternoId).ComStatus(StatusPagamento.Pendente).Generate();

        var pagamentoExterno =
            new FornecedorGetPagamentoResponseDto(pagamento.PagamentoExternoId, StatusPagamento.Autorizado);
        mockFornecedorPagamentoGateway
            .Setup(g => g.ObterPagamento(pagamento.PagamentoExternoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagamentoExterno);
        
        mockPagamentoGateway.Setup(g => g.FindPagamentoByPedidoIdAsync(Guid.Parse(pagamentoExterno.IdExterno)))
            .ReturnsAsync([pagamento]);
        
        _pagamentoHandler.Setup(g => g.HandleAsync(It.IsAny<DomainEvent>()))
            .ReturnsAsync(Result<Pagamento>.Succeed(pagamento));
        
        mockPagamentoGateway.Setup(g => g.UpdatePagamentoAsync(It.IsAny<Pagamento>()))
            .ReturnsAsync(pagamento);
        
        var evento = new PagamentoConfirmadoDomainEvent(pagamento.Id);
        mockPagamentoGateway.Setup(g => g.GetByIdAsync(evento.PagamentoId))
            .ReturnsAsync(pagamento);
        
        _pedidoGateway.Setup(g => g.AtualizaStatusPagamentoAsync(pagamento.PedidoId, pagamento.Status))
            .Returns(Task.CompletedTask);

        var useCase = new ConfirmarStatusPagamentoUseCase(_mockLogger, mockFornecedorPagamentoGateway.Object,
            mockPagamentoGateway.Object, _pagamentoHandler.Object);

        // Act
        var resultado = await useCase.ResolveAsync(pagamentoExternoId);

        // Assert
        Assert.NotNull(resultado.Value);
        Assert.Equal(pagamento.Id, resultado.Value.Id);
        mockPagamentoGateway.Verify(g => g.UpdatePagamentoAsync(It.IsAny<Pagamento>()), Times.Once);
    }
}