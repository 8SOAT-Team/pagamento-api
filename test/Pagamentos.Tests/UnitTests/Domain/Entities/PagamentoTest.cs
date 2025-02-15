using Pagamentos.Domain.Entities;
using Pagamentos.Domain.Exceptions;

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
            var pagamento = new global::Pagamentos.Domain.Entities.Pagamento(pedido.Id, MetodoDePagamento.Pix, 100m, "idExterno");
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
            Action act = () => new global::Pagamentos.Domain.Entities.Pagamento(Guid.Empty, MetodoDePagamento.Pix, 100m, "idExterno");
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
            Action act = () => new global::Pagamentos.Domain.Entities.Pagamento(pedido.Id, MetodoDePagamento.Pix, -1m, "idExterno");
            //Assert
            Assert.Throws<DomainExceptionValidation>(() => act());
        }

        [Fact]
        public void DeveLancarExcepetionQuandoMetodoDePagamentoInvalido()
        {
            //Arrange
            var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
            var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
            List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
            listaItens.Add(itemPedido);
            var pedido = new Pedido(Guid.NewGuid(), listaItens);
            //Act
            Action act = () => new global::Pagamentos.Domain.Entities.Pagamento(pedido.Id, (MetodoDePagamento)999, 100m, "idExterno");
            //Assert
            Assert.ThrowsAny<DomainExceptionValidation>(() => act());
        }
        [Fact]
        public void DeveLancarExcepetionQuandoIdExternoInvalido()
        {
            //Arrange
            var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
            var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
            List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
            listaItens.Add(itemPedido);
            var pedido = new Pedido(Guid.NewGuid(), listaItens);
            //Act
            Action act = () => new global::Pagamentos.Domain.Entities.Pagamento(pedido.Id, MetodoDePagamento.Pix, 100m, string.Empty);
            //Assert
            Assert.NotNull(act);
        }
        [Fact]
        public void DeveLancarExcepetionQuandoPedidoIdInvalido()
        {
            //Arrange
            var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
            var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
            List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
            listaItens.Add(itemPedido);
            var pedido = new Pedido(Guid.NewGuid(), listaItens);
            //Act
            Action act = () => new global::Pagamentos.Domain.Entities.Pagamento(Guid.Empty, MetodoDePagamento.Pix, 100m, "idExterno");
            //Assert
            Assert.Throws<DomainExceptionValidation>(() => act());
        }



    }
}
