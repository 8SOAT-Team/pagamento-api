using Bogus;
using Pagamento.Domain.Entities;
using Pagamento.Tests.UnitTests.Domain.Stubs.Pedidos;


namespace Tests.UnitTests.Domain.Stubs.Pedidos;

internal sealed class PagamentoStubBuilder : Faker<Pagamento.Domain.Entities.Pagamento>
{
    private PagamentoStubBuilder()
    {
        var pedido = PedidoStubBuilder.Create();
        CustomInstantiator(f => new Pagamento.Domain.Entities.Pagamento(pedido.Id,
            MetodoDePagamento.Pix, f.Random.Decimal(1, 1000), f.Random.Guid().ToString()));
    }

    public PagamentoStubBuilder WithStatus(StatusPagamento status)
    {
        RuleFor(x => x.Status, status);
        return this;
    }

    public static PagamentoStubBuilder NewBuilder() => new();
    public static Pagamento.Domain.Entities.Pagamento Create() => new PagamentoStubBuilder().Generate();
    public static List<Pagamento.Domain.Entities.Pagamento> CreateMany(int qty) => new PagamentoStubBuilder().Generate(qty);
    public static Pagamento.Domain.Entities.Pagamento Autorizado() => NewBuilder().WithStatus(StatusPagamento.Autorizado).Generate();
    public static Pagamento.Domain.Entities.Pagamento Rejeitado() => NewBuilder().WithStatus(StatusPagamento.Rejeitado).Generate();
    public static Pagamento.Domain.Entities.Pagamento Cancelado() => NewBuilder().WithStatus(StatusPagamento.Cancelado).Generate();
}