using Bogus;
using Pagamentos.Apps.UseCases.Dtos;

namespace Pagamentos.Tests.IntegrationTests.Builder;

internal sealed class IniciarPagamentoDtoBuilder : Faker<IniciarPagamentoDto>
{
    public IniciarPagamentoDtoBuilder()
    {
        RuleFor(x => x.PedidoId, Guid.NewGuid);
        RuleFor(x => x.Pagador, new IniciarPagamentoPagadorDtoBuilder().Generate());
        RuleFor(x => x.Itens, f => f.Make(2, () => new IniciarPagamentoItemDtoBuilder().Generate()).ToList());
    }
}