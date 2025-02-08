using Bogus;
using Pagamento.Domain.Entities;

namespace Pagamento.Tests.IntegrationTests.Builder;
internal class PedidoBuilder : Faker<Pedido>
{
    public PedidoBuilder()
    {
        CustomInstantiator(f => new Pedido(clienteId: f.Random.Guid(), itensDoPedido: new List<ItemDoPedido>()
            {
                new ItemDoPedidoBuilder().Build(),
                new ItemDoPedidoBuilder().Build()
            }));
    }

    public PedidoBuilder(Guid clientId)
    {
        CustomInstantiator(f => new Pedido(clienteId: clientId, itensDoPedido: new List<ItemDoPedido>()
            {
                new ItemDoPedidoBuilder().Build(),
                new ItemDoPedidoBuilder().Build()
            }));
    }
    public Pedido Build() => Generate();
}
