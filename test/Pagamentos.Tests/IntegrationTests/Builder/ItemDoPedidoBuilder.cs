using Bogus;
using Pagamentos.Domain.Entities;


namespace Pagamentos.Tests.IntegrationTests.Builder;
internal class ItemDoPedidoBuilder : Faker<ItemDoPedido>
{
    public ItemDoPedidoBuilder()
    {
        var produto = new ProdutoBuilder().Build();
        CustomInstantiator(f => new ItemDoPedido(pedidoId: f.Random.Guid(), produto, quantidade: f.Random.Int(1, 10)));
    }

    public ItemDoPedidoBuilder(Guid pedidoId)
    {
        var produto = new ProdutoBuilder().Build();
        CustomInstantiator(f => new ItemDoPedido(pedidoId, produto, quantidade: f.Random.Int(1, 10)));
    }
    public ItemDoPedido Build() => Generate();
}

