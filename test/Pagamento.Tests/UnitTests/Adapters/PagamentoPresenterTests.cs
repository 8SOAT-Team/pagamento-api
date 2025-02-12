using Pagamento.Adapters.Controllers;
using Pagamento.Adapters.Presenters;
using Pagamento.Domain.Entities;


namespace Tests.UnitTests.Adapters;
public class PagamentoPresenterTests
{
    [Fact]
    public void ToPagamentoDTO_DeveRetornarDTO_Corretamente()
    {

        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
        listaItens.Add(itemPedido);

        var pedido = new Pedido(Guid.NewGuid(), listaItens);

        //Act
        var pagamento = new Pagamento.Domain.Entities.Pagamento(pedido.Id, MetodoDePagamento.Pix, 100m, "idExterno");


        pagamento.AssociarPagamentoExterno("ext123", "https://pagamento.com/ext123");
        pagamento.FinalizarPagamento(true);
     

        // Act
        var dto = PagamentoPresenter.ToPagamentoDTO(pagamento);

        // Assert
        Assert.Equal(pagamento.Id, dto.Id);
        Assert.Equal((MetodosDePagamento)pagamento.MetodoDePagamento, dto.MetodoDePagamento);
        Assert.Equal((StatusDoPagamento)pagamento.Status, dto.status);
        Assert.Equal(pagamento.ValorTotal, dto.ValorTotal);
        Assert.Equal(pagamento.PagamentoExternoId, dto.PagamentoExternoId);
        Assert.Equal(pagamento.UrlPagamento, dto.UrlPagamento);
    }

    [Fact]
    public void ToListPagamentoDTO_DeveRetornarListaDTO_Corretamente()
    {
        // Arrange
        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
        listaItens.Add(itemPedido);
        var pedido = new Pedido(Guid.NewGuid(), listaItens);
        //Act
        var pagamento = new Pagamento.Domain.Entities.Pagamento(pedido.Id, MetodoDePagamento.Pix, 100m, "idExterno");
        pagamento.AssociarPagamentoExterno("ext123", "https://pagamento.com/ext123");
        pagamento.FinalizarPagamento(true);
        var pagamentos = new List<Pagamento.Domain.Entities.Pagamento> { pagamento };
        // Act
        var dtos = PagamentoPresenter.ToListPagamentoDTO(pagamentos);
        // Assert
        Assert.Single(dtos);
        Assert.Equal(pagamento.Id, dtos.First().Id);
        Assert.Equal((MetodosDePagamento)pagamento.MetodoDePagamento, dtos.First().MetodoDePagamento);
        Assert.Equal((StatusDoPagamento)pagamento.Status, dtos.First().status);
        Assert.Equal(pagamento.ValorTotal, dtos.First().ValorTotal);
        Assert.Equal(pagamento.PagamentoExternoId, dtos.First().PagamentoExternoId);
        Assert.Equal(pagamento.UrlPagamento, dtos.First().UrlPagamento);

    }
}