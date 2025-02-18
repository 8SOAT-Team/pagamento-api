using Bogus;
using FluentAssertions;
using Pagamentos.Adapters.Presenters;
using Pagamentos.Domain.Entities;
using Pagamentos.Tests.IntegrationTests.Builder;

namespace Pagamentos.Tests.UnitTests.Adapters;

public class PagamentoPresenterTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void ToPagamentoDTO_DeveRetornarDTO_Corretamente()
    {
        // Arrange
        var pagamento = new Pagamento(_faker.Random.Guid(), MetodoDePagamento.Pix, 100m, "idExterno");
        pagamento.AssociarPagamentoExterno(_faker.Random.Guid().ToString(), _faker.Internet.Url());
        pagamento.FinalizarPagamento(true);

        // Act
        var dto = PagamentoPresenter.ToPagamentoDto(pagamento);

        // Assert
        dto.Should().BeEquivalentTo(pagamento);
    }

    [Fact]
    public void ToListPagamentoDTO_DeveRetornarListaDTO_Corretamente()
    {
        // Arrange
        var pagamentos = _faker.Make(_faker.Random.Int(1, 10), PagamentoBuilder.Build).ToList();

        // Act
        var dtos = PagamentoPresenter.ToListPagamentoDto(pagamentos);
        
        // Assert
        dtos.Should().BeEquivalentTo(pagamentos);
    }

    [Fact]
    public void ToPagamento_DeveRetornarPagamento_Corretamente()
    {
        // Arrange
        var pagamento = new Pagamento(_faker.Random.Guid(), MetodoDePagamento.Pix, 100m, "idExterno");
        
        // Act
        var result = PagamentoPresenter.ToPagamentoDto(pagamento);
        
        // Assert
        result.Should().BeEquivalentTo(pagamento);
    }
}