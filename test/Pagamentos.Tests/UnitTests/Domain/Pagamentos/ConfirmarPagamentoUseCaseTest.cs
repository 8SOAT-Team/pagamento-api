using Microsoft.Extensions.Logging;
using Moq;
using Pagamentos.Apps.UseCases;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Domain.Entities;
using Pagamentos.Tests.IntegrationTests.Builder;

namespace Pagamentos.Tests.UnitTests.Domain.Pagamentos;

public class ConfirmarPagamentoUseCaseTest
{
    [Fact]
    public async Task ResolveAsync_DeveAdicionarErroQuandoPagamentoNaoEncontrado()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ConfirmarPagamentoUseCase>>();
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();

        var pagamentoId = Guid.NewGuid();
        var dto = new ConfirmarPagamentoDto(pagamentoId, StatusPagamento.Autorizado);

        mockPagamentoGateway.Setup(g => g.GetByIdAsync(pagamentoId)).ReturnsAsync((Pagamento?)null);

        var useCase = new ConfirmarPagamentoUseCase(mockLogger.Object, mockPagamentoGateway.Object);

        // Act
        var resultado = await useCase.ResolveAsync(dto);

        // Assert
        Assert.Null(resultado);
        var useCaseErrors = useCase.GetErrors();
        Assert.Single(useCaseErrors);
        Assert.Equal("Pagamento não encontrado", useCaseErrors.FirstOrDefault().Description);
    }

    [Fact]
    public async Task ResolveAsync_DeveFinalizarPagamentoERetornarObjetoAtualizado()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Build();

        var mockLogger = new Mock<ILogger<ConfirmarPagamentoUseCase>>();
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();
        var dto = new ConfirmarPagamentoDto(pagamento.Id, StatusPagamento.Autorizado);
        mockPagamentoGateway.Setup(g => g.GetByIdAsync(pagamento.Id)).ReturnsAsync(pagamento);

        var useCase = new ConfirmarPagamentoUseCase(mockLogger.Object, mockPagamentoGateway.Object);
        
        // Act
        var resultado = await useCase.ResolveAsync(dto);
        
        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(StatusPagamento.Autorizado, resultado.Value!.Status);
    }
}