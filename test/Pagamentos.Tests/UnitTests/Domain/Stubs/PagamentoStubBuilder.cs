using Bogus;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Tests.UnitTests.Domain.Stubs;

internal sealed class PagamentoStubBuilder : Faker<Pagamento>
{
    private PagamentoStubBuilder()
    {
        CustomInstantiator(f => new Pagamento(f.Random.Guid(),
            MetodoDePagamento.Pix, f.Random.Decimal(1, 1000), f.Random.Guid().ToString()));
    }

    public PagamentoStubBuilder WithStatus(StatusPagamento status)
    {
        RuleFor(x => x.Status, status);
        return this;
    }

    public static PagamentoStubBuilder NewBuilder() => new();
    public static Pagamento Create() => new PagamentoStubBuilder().Generate();
    public static List<Pagamento> CreateMany(int qty) => new PagamentoStubBuilder().Generate(qty);
    public static Pagamento Autorizado() => NewBuilder().WithStatus(StatusPagamento.Autorizado).Generate();
    public static Pagamento Rejeitado() => NewBuilder().WithStatus(StatusPagamento.Rejeitado).Generate();
    public static Pagamento Cancelado() => NewBuilder().WithStatus(StatusPagamento.Cancelado).Generate();
}