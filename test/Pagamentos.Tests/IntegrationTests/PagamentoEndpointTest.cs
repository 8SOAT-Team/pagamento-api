using Pagamentos.Adapters.Controllers;
using System.Net;
using System.Net.Http.Json;
using Bogus;
using FluentAssertions;
using Pagamentos.Api.Pagamento;
using Pagamentos.Domain.Entities;
using Pagamentos.Tests.IntegrationTests.Builder;
using Pagamentos.Tests.IntegrationTests.HostTest;

namespace Pagamentos.Tests.IntegrationTests;

public class PagamentoEndpointTest : IClassFixture<FastOrderWebApplicationFactory>
{
    private readonly FastOrderWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker = new();

    public PagamentoEndpointTest(FastOrderWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private Task<Pagamento> InsertPagamentoAsync()
    {
        return InsertPagamentoAsync(PagamentoBuilder.Build());
    }

    private async Task<Pagamento> InsertPagamentoAsync(Pagamento pagamento)
    {
        _factory.Context!.Pagamentos.Add(pagamento);
        await _factory.Context.SaveChangesAsync();
        return pagamento;
    }

    [Fact]
    public async Task Post_IniciarPagamento_ReturnsCreated()
    {
        // Arrange
        var pedidoId = _faker.Random.Guid();
        var request = new NovoPagamentoRequest
        {
            MetodoDePagamento = MetodosDePagamento.Master,
            Itens = _faker.Make(_faker.Random.Int(1, 10), NovoPagamentoItemRequestBuilder.Build).ToList(),
            Pagador = NovoPagamentoPagadorRequestBuilder.Build()
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/pagamento/pedido/{pedidoId}", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Patch_ConfirmarPagamento_ReturnsOk()
    {
        // Arrange
        var pagamento = await InsertPagamentoAsync();
        var request = new ConfirmarPagamentoDTO(StatusDoPagamento.Autorizado);

        // Act
        var response = await _client.PatchAsJsonAsync($"/pagamento/{pagamento.Id}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_GetPagamentoByPedido_ReturnsOk()
    {
        // Arrange
        var pagamento = await InsertPagamentoAsync();

        // Act
        var response = await _client.GetAsync($"/pagamento/pedido/{pagamento.PedidoId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_DadoQueOPedidoInformadoEhEmpty_Deve_RetornarBadRequest()
    {
        // Arrange
        var pedidoId = Guid.Empty;
        var request = new NovoPagamentoRequest { MetodoDePagamento = MetodosDePagamento.Master };

        // Act
        var response = await _client.PostAsJsonAsync($"/pagamento/pedido/{pedidoId}", request);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_DadoQueJaExistePagamentoParaOPedidoId_E_JaFoiIniciado_Deve_RetornarBadRequest()
    {
        // Arrange
        var pagamento = PagamentoBuilder.CreateBuilder()
            .ComStatus(StatusPagamento.Autorizado)
            .Generate();
        pagamento = await InsertPagamentoAsync(pagamento);
        var request = new NovoPagamentoRequest { MetodoDePagamento = MetodosDePagamento.Master };

        // Act
        var response = await _client.PostAsJsonAsync($"/pagamento/pedido/{pagamento.PedidoId}", request);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Patch_ConfirmarPagamento_ReturnsBadRequest()
    {
        // Arrange
        var pagamentoId = Guid.Empty;
        var request = new NovoPagamentoRequest { MetodoDePagamento = MetodosDePagamento.Master };
        
        // Act
        var response = await _client.PatchAsJsonAsync($"/pagamento/{pagamentoId}", request);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}