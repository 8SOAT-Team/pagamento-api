using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Pagamento.Adapters.Controllers;
using Pagamento.Infrastructure.Pagamentos;
using System.Net.Http.Json;
using System.Net;
using Pagamento.Adapters.Types;

namespace Tests.UnitTests.FrameworksAndDrivers;
public class PagamentoWebhookExtensionsTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IPagamentoController> _mockController;

    public PagamentoWebhookExtensionsTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _mockController = new Mock<IPagamentoController>();
    }

    [Fact]
    public async Task AddEndpointWebhook_ShouldReturnOk_WhenTypeIsNotPayment()
    {
        // Arrange
        var client = _factory.CreateClient();
        var queryString = "?type=other&type=payment";
        var requestContent = new PagamentoWebhookDTO
        {
            // Preencha com os dados esperados do payload
        };

        // Act
        var response = await client.PostAsJsonAsync("/pagamento/mp/webhook" + queryString, requestContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AddEndpointWebhook_ShouldCallController_WhenTypeIsPayment()
    {
        // Arrange
        var client = _factory.CreateClient();
        var dataId = "some-id";
        var queryString = $"?type=payment&data.id={dataId}";
        var requestContent = new PagamentoWebhookDTO
        {
            // Preencha com os dados esperados do payload
        };

        //defina um novo objeto do tipo PagamentoResponseDTO preenchido com os dados esperados
        var pagamentoResponse = new PagamentoResponseDTO(
            Id: Guid.NewGuid(),
            MetodoDePagamento: MetodosDePagamento.Cartao, // Exemplo de valor para o enum MetodosDePagamento
            status: StatusDoPagamento.Autorizado, // Exemplo de valor para o enum StatusDoPagamento
            ValorTotal: 100.50m,
            PagamentoExternoId: "12345ABC",
            UrlPagamento: "https://pagamento.exemplo.com"
        );

        var expectedResult = Result<PagamentoResponseDTO>.Succeed(pagamentoResponse);

        _mockController.Setup(c => c.ReceberWebhookPagamento(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedResult);

        // Act
        var response = await client.PostAsJsonAsync("/pagamento/mp/webhook" + queryString, requestContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockController.Verify(c => c.ReceberWebhookPagamento(It.Is<string>(s => s == dataId), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AddEndpointWebhook_ShouldReturnBadRequest_WhenControllerFails()
    {
        // Arrange
        var client = _factory.CreateClient();
        var dataId = "invalid-id";
        var queryString = $"?type=payment&data.id={dataId}";
        var pagamentoWebhook = new PagamentoWebhookDTO
        {
            Id = 12345, // Valor para a propriedade Id
            Action = "payment_received", // Ação que descreve o evento
            ApiVersion = "1.0", // Versão da API
            Data = new PagamentoWebhookDataDTO
            {
                Id = "67890" // Id relacionado aos dados do webhook
            },
            DateCreated = DateTime.UtcNow, // Data de criação
            LiveMode = true, // Se está em modo ao vivo ou teste
            Type = "payment", // Tipo de evento do webhook
            UserId = "user_123" // ID do usuário que acionou o evento
        };


        var expectedResult = Result<PagamentoResponseDTO>.Succeed(new PagamentoResponseDTO(
            Id: Guid.NewGuid(),
            MetodoDePagamento: MetodosDePagamento.Cartao, // Exemplo de valor para o enum MetodosDePagamento
            status: StatusDoPagamento.Autorizado, // Exemplo de valor para o enum StatusDoPagamento
            ValorTotal: 100.50m,
            PagamentoExternoId: "12345ABC",
            UrlPagamento: "https://pagamento.exemplo.com"
        ));

        _mockController.Setup(c => c.ReceberWebhookPagamento(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedResult);

        // Act
        var response = await client.PostAsJsonAsync("/pagamento/mp/webhook" + queryString, expectedResult);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
