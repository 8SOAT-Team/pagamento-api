using Bogus;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Tests.IntegrationTests.Builder;

internal sealed class PagamentoBuilder : Faker<Pagamento>
{
    public PagamentoBuilder()
    {
        CustomInstantiator(f => new Pagamento(
            pedidoId: f.Random.Guid(),
            metodoDePagamento: f.PickRandom<MetodoDePagamento>(),
            valorTotal: f.Finance.Amount(1),
            pagamentoExternoId: null
        ));
    }

    public PagamentoBuilder ComStatus(StatusPagamento status)
    {
        RuleFor(x => x.Status, f => status);
        return this;
    }
    
    public PagamentoBuilder ComPagamentoExternoId(string pagamentoExternoId)
    {
        RuleFor(x => x.PagamentoExternoId, f => pagamentoExternoId);
        return this;
    }

    public static Pagamento Build() => new PagamentoBuilder().Generate();
    public static PagamentoBuilder CreateBuilder() => new();
}