
using Pagamento.Apps.UseCases.Dtos;
using Pagamento.Domain.Entities;

namespace Pagamento.Tests.UnitTests.Domain.Pagamentos.Dtos;
public class IniciarPagamentoDtoTest
{
    [Fact]
    public void DeveCriarIniciarPagamentoDtoCorretamente()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var metodoPagamento = MetodoDePagamento.Master;
        var valorTotal = 100.0m;
        var emailPagador = "test@example.com";
        var webhookUrl = "http://example.com/webhook";
        var token = "sampleToken";

        // Act
        var dto = new IniciarPagamentoDto(pedidoId, metodoPagamento, valorTotal, emailPagador, webhookUrl, token);

        // Assert
        Assert.Equal(pedidoId, dto.PedidoId);
        Assert.Equal(metodoPagamento, dto.MetodoDePagamento);
        Assert.Equal(valorTotal, dto.ValorTotal);
        Assert.Equal(emailPagador, dto.EmailPagador);
        Assert.Equal(webhookUrl, dto.webhookUrl);
        Assert.Equal(token, dto.token);
    }

    [Fact]
    public void DeveSerIgualQuandoPropriedadesSaoIguais()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var metodoPagamento = MetodoDePagamento.Master;
        var valorTotal = 100.0m;
        var emailPagador = "test@example.com";
        var webhookUrl = "http://example.com/webhook";
        var token = "sampleToken";

        var dto1 = new IniciarPagamentoDto(pedidoId, metodoPagamento, valorTotal, emailPagador, webhookUrl, token);
        var dto2 = new IniciarPagamentoDto(pedidoId, metodoPagamento, valorTotal, emailPagador, webhookUrl, token);

        // Act & Assert
        Assert.Equal(dto1, dto2);
    }

    [Fact]
    public void DeveSerDiferenteQuandoPropriedadesSaoDiferentes()
    {
        // Arrange
        var pedidoId1 = Guid.NewGuid();
        var pedidoId2 = Guid.NewGuid();
        var metodoPagamento = MetodoDePagamento.Master;
        var valorTotal = 100.0m;
        var emailPagador = "test@example.com";
        var webhookUrl = "http://example.com/webhook";
        var token = "sampleToken";

        var dto1 = new IniciarPagamentoDto(pedidoId1, metodoPagamento, valorTotal, emailPagador, webhookUrl, token);
        var dto2 = new IniciarPagamentoDto(pedidoId2, metodoPagamento, valorTotal, emailPagador, webhookUrl, token);

        // Act & Assert
        Assert.NotEqual(dto1, dto2);
    }

    [Fact]
    public void DevePermitirDesestruturacao()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var metodoPagamento = MetodoDePagamento.Master;
        var valorTotal = 100.0m;
        var emailPagador = "test@example.com";
        var webhookUrl = "http://example.com/webhook";
        var token = "sampleToken";
        var dto = new IniciarPagamentoDto(pedidoId, metodoPagamento, valorTotal, emailPagador, webhookUrl, token);

        // Act
        var (pedidoIdDesestruturado, metodoPagamentoDesestruturado, valorTotalDesestruturado, emailPagadorDesestruturado, webhookUrlDesestruturado, tokenDesestruturado) = dto;

        // Assert
        Assert.Equal(pedidoId, pedidoIdDesestruturado);
        Assert.Equal(metodoPagamento, metodoPagamentoDesestruturado);
        Assert.Equal(valorTotal, valorTotalDesestruturado);
        Assert.Equal(emailPagador, emailPagadorDesestruturado);
        Assert.Equal(webhookUrl, webhookUrlDesestruturado);
        Assert.Equal(token, tokenDesestruturado);
    }
}
