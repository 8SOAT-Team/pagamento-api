using Bogus;
using Pagamentos.Domain.Entities;
using Pagamentos.Domain.Exceptions;

namespace Pagamentos.Tests.UnitTests.Domain.Entities;

public sealed class PagamentoTest
{
    private readonly Faker _faker = new();

    [Fact]
    public void Construtor_NaoInformadoId_InformadoPedidoIdEOutrosDados_Deve_CriarEAtribuirValores()
    {
        //Arrange
        var pedidoId = _faker.Random.Guid();
        //Act
        var pagamento = new Pagamento(pedidoId, MetodoDePagamento.Pix, 100m, "idExterno");

        //Assert
        Assert.NotNull(pagamento);
    }

    [Fact]
    public void DeveLancarExceptionQuandoPagamentoIdInvalido()
    {
        //Act
        var act = () =>
            new Pagamento(Guid.Empty, MetodoDePagamento.Pix, 100m, "idExterno");

        //Assert
        Assert.Throws<DomainExceptionValidation>(() => act());
    }

    [Fact]
    public void DeveLancarExceptionQuandoValorInvalido()
    {
        //Act
        var act = () =>
            new Pagamento(Guid.NewGuid(), MetodoDePagamento.Pix, -1m, "idExterno");
        //Assert
        Assert.Throws<DomainExceptionValidation>(() => act());
    }

    [Fact]
    public void DeveLancarExcepetionQuandoMetodoDePagamentoInvalido()
    {
        //Act
        var act = () =>
            new Pagamento(Guid.NewGuid(), (MetodoDePagamento)999, 100m, "idExterno");
        //Assert
        Assert.ThrowsAny<DomainExceptionValidation>(() => act());
    }

    [Fact]
    public void DeveLancarExceptionQuandoPedidoIdInvalido()
    {
        //Act
        var act = () =>
            new Pagamento(Guid.Empty, Guid.NewGuid(), MetodoDePagamento.Pix, 100m,
                "idExterno");
        //Assert
        Assert.Throws<DomainExceptionValidation>(() => act());
    }
}