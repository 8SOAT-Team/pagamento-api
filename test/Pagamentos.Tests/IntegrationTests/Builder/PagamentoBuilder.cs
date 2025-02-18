using Bogus;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Tests.IntegrationTests.Builder;

internal sealed class PagamentoBuilder : Faker<Pagamento>
{
    public PagamentoBuilder()
    {
        CustomInstantiator(f => new Pagamento(
            pedidoId: f.Random.Guid(),
            metodoDePagamento: MetodoDePagamento.Pix,
            valorTotal: f.Finance.Amount(1),
            pagamentoExternoId: null
        ));

        RuleFor(x => x.Status, StatusPagamento.Pendente);
    }

    public PagamentoBuilder ComStatus(StatusPagamento status)
    {
        RuleFor(x => x.Status, status);
        return this;
    }

    public PagamentoBuilder ComPagamentoExternoId(string pagamentoExternoId)
    {
        RuleFor(x => x.PagamentoExternoId, pagamentoExternoId);
        return this;
    }

    public PagamentoBuilder ComPagamentoUrl(string url)
    {
        RuleFor(x => x.UrlPagamento, url);
        return this;
    }

    public PagamentoBuilder ComPedidoId(Guid pedidoId)
    {
        RuleFor(x => x.PedidoId, pedidoId);
        return this;
    }

    public static Pagamento Build() => new PagamentoBuilder().Generate();
    public static PagamentoBuilder CreateBuilder() => new();
}