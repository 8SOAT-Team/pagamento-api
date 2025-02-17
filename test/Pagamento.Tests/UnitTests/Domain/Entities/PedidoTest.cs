using Pagamento.Domain.Entities;
using Pagamento.Domain.Exceptions;

namespace Postech8SOAT.FastOrder.Tests.Domain.Entities
{
    public class PedidoTest
    {
        [Fact]
        public void DeveCriarNovoPedidoComSucesso()
        {
            //Arrange
            var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
            var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
            List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
            listaItens.Add(itemPedido);
            //Act
            var pedido = new Pedido(Guid.NewGuid(), listaItens);
            //Assert
            Assert.NotNull(pedido);
        }

        [Fact]
        public void DeveLancarExceptionQuandoPedidoNaoTiverItens()
        {
            //Arrange
            List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
            //Act
            Action act = () => new Pedido(Guid.NewGuid(), listaItens);
            //Assert
            Assert.Throws<DomainExceptionValidation>(() => act());
        }

        [Fact]
        public void DeveLancarExceptionQuandoIdPedidoInvalido()
        {
            //Arrange
            var itemPedido = new ItemDoPedido(Guid.NewGuid(), Guid.NewGuid(), 2);
            List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
            listaItens.Add(itemPedido);
            //Act
            Action act = () => new Pedido(Guid.Empty, Guid.NewGuid(), listaItens);
            //Assert
            Assert.Throws<DomainExceptionValidation>(() => act());
        }

        [Fact]
        public void DeveLancarExceptionQuandoPedidoTiverIdClienteInvalido()
        {
            //Arrange
            var itemPedido = new ItemDoPedido(Guid.NewGuid(), Guid.NewGuid(), 2);
            List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
            listaItens.Add(itemPedido);
            //Act
            Action act = () => new Pedido(Guid.NewGuid(), Guid.Empty, listaItens);
            //Assert
            Assert.Throws<DomainExceptionValidation>(() => act());
        }

        [Fact]
        public void Cancelar_PedidoComStatusRecebido_DeveAlterarStatusParaCancelado()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid(), new List<ItemDoPedido>());

            // Act
            pedido.Cancelar();

            // Assert
            Assert.Equal(StatusPedido.Cancelado, pedido.StatusPedido);
        }

        [Fact]
        public void Cancelar_PedidoComStatusFinalizado_NaoDeveAlterarStatus()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid(), new List<ItemDoPedido>());
            typeof(Pedido).GetProperty("StatusPedido").SetValue(pedido, StatusPedido.Finalizado);

            // Act & Assert
            var exception = Assert.Throws<DomainExceptionValidation>(() => pedido.Cancelar());
            Assert.Equal("O pedido não pode ser cancelado após ser Finalizado.", exception.Message);
        }

        [Fact]
        public void Entregar_PedidoComStatusPronto_DeveAlterarStatusParaFinalizado()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid(), new List<ItemDoPedido>());
            typeof(Pedido).GetProperty("StatusPedido").SetValue(pedido, StatusPedido.Pronto);

            // Act
            pedido.Entregar();

            // Assert
            Assert.Equal(StatusPedido.Finalizado, pedido.StatusPedido);
        }

        [Fact]
        public void Entregar_PedidoComStatusEmPreparacao_NaoDeveAlterarStatus()
        {
            // Arrange
            var pedido = new Pedido(Guid.NewGuid(), new List<ItemDoPedido>());
            typeof(Pedido).GetProperty("StatusPedido").SetValue(pedido, StatusPedido.EmPreparacao);

            // Act & Assert
            var exception = Assert.Throws<DomainExceptionValidation>(() => pedido.Entregar());
            Assert.Equal("O pedido deve estar Pronto para realizar a entrega.", exception.Message);
        }

    }
}
