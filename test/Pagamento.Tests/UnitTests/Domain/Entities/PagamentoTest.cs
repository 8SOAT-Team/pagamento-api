using Pagamento.Domain.Entities;
using Pagamento.Domain.Exceptions;

namespace Tests.UnitTests.Domain.Entities
{
    public class PagamentoTest
    {
        [Fact]
        public void DeveCriarNovoPagamentoComSucesso()
        {
            //Arrange
            var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
            var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
            List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
            listaItens.Add(itemPedido);

            var pedido = new Pedido(Guid.NewGuid(), listaItens);

            //Act
            var pagamento = new Pagamento.Domain.Entities.Pagamento(pedido.Id,MetodoDePagamento.Pix, 100m, "idExterno");
            //Assert
            Assert.NotNull(pagamento);
        }

        [Fact]
        public void DeveLancarExcepetionQuandoPagamentoIdInvalido()
        {
            //Arrange
            var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
            var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
            List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
            listaItens.Add(itemPedido);

            var pedido = new Pedido(Guid.NewGuid(), listaItens);

            //Act
            Action act = () => new Pagamento.Domain.Entities.Pagamento(pedido.Id,MetodoDePagamento.Pix, 100m, "idExterno");
            //Assert
            Assert.Throws<DomainExceptionValidation>(() => act());
        }

        [Fact]
        public void DeveLancarExcepetionQuandoValorInvalido()
        {
            //Arrange
            var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
            var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
            List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
            listaItens.Add(itemPedido);

            var pedido = new Pedido(Guid.NewGuid(), listaItens);

            //Act
            Action act = () => new Pagamento.Domain.Entities.Pagamento(pedido.Id,MetodoDePagamento.Pix, -1m, "idExterno");
            //Assert
            Assert.Throws<DomainExceptionValidation>(() => act());
        }
    }
}
