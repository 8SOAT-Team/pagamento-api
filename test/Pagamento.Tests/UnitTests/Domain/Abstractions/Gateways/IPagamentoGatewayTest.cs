using Moq;
using Pagamento.Apps.UseCases;
using Pagamento.Domain.Entities;

namespace Tests.UnitTests.Domain.Abstractions.Gateways;
public class IPagamentoGatewayTest
{

    [Fact]
    public async Task BuscarPagamentosPorPedidoIdAsync_DeveRetornarPagamentosQuandoExistirem()
    {
        // Arrange
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();

        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
        listaItens.Add(itemPedido);
        var pedido = new Pedido(Guid.NewGuid(), listaItens);

        var pedidoId = Guid.NewGuid();
        var pagamento1 = new Pagamento.Domain.Entities.Pagamento(pedido.Id, MetodoDePagamento.Pix, 100m, "idExterno");
        var pagamento2 = new Pagamento.Domain.Entities.Pagamento(pedido.Id,MetodoDePagamento.Pix, 100m, "idExterno");
        mockPagamentoGateway.Setup(gateway => gateway.FindPagamentoByPedidoIdAsync(pedidoId))
            .ReturnsAsync(new List<Pagamento.Domain.Entities.Pagamento> { pagamento1, pagamento2 });

        var pagamentoService = new PagamentoServiceTest(mockPagamentoGateway.Object);

        // Act
        var pagamentos = await pagamentoService.BuscarPagamentosPorPedidoIdAsync(pedidoId);

        // Assert
        Assert.NotNull(pagamentos);
        Assert.Equal(2, pagamentos.Count);
    }

    [Fact]
    public async Task BuscarPagamentoPorIdAsync_DeveRetornarPagamentoQuandoExistir()
    {
        // Arrange
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();
        var id = Guid.NewGuid();
        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
        listaItens.Add(itemPedido);
        var pedido = new Pedido(Guid.NewGuid(), listaItens);

        var pagamento = new Pagamento.Domain.Entities.Pagamento(pedido.Id,MetodoDePagamento.Pix, 100m, "idExterno");
        mockPagamentoGateway.Setup(gateway => gateway.GetByIdAsync(id))
            .ReturnsAsync(pagamento);

        var pagamentoService = new PagamentoServiceTest(mockPagamentoGateway.Object);

        // Act
        var pagamentoRetornado = await pagamentoService.BuscarPagamentoPorIdAsync(id);

        // Assert
        Assert.NotNull(pagamentoRetornado);
        Assert.Equal(id, pagamentoRetornado?.Id);
    }

    [Fact]
    public async Task BuscarPagamentoPorIdAsync_DeveRetornarNullQuandoNaoExistir()
    {
        // Arrange
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();
        var idInexistente = Guid.NewGuid();
        mockPagamentoGateway.Setup(gateway => gateway.GetByIdAsync(idInexistente))
            .ReturnsAsync((Pagamento.Domain.Entities.Pagamento?)null);

        var pagamentoService = new PagamentoServiceTest(mockPagamentoGateway.Object);

        // Act
        var pagamentoRetornado = await pagamentoService.BuscarPagamentoPorIdAsync(idInexistente);

        // Assert
        Assert.Null(pagamentoRetornado);
    }

    [Fact]
    public async Task AtualizarPagamentoAsync_DeveAtualizarPagamento()
    {
        // Arrange
        var mockPagamentoGateway = new Mock<IPagamentoGateway>();
        var id = Guid.NewGuid();

        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
        listaItens.Add(itemPedido);
        var pedido = new Pedido(Guid.NewGuid(), listaItens);

        var pagamento = new Pagamento.Domain.Entities.Pagamento( pedido.Id, MetodoDePagamento.Pix, 100m, "idExterno");
        mockPagamentoGateway.Setup(gateway => gateway.UpdatePagamentoAsync(pagamento))
            .ReturnsAsync(pagamento);

        var pagamentoService = new PagamentoServiceTest(mockPagamentoGateway.Object);

        // Act
        var pagamentoAtualizado = await pagamentoService.AtualizarPagamentoAsync(pagamento);

        // Assert
        Assert.NotNull(pagamentoAtualizado);
        Assert.Equal(id, pagamentoAtualizado.Id);
    }


    [Fact]
    public async Task GetByIdAsync_DeveRetornarPagamentoQuandoExistir()
    {
        // Arrange
        var pagamentoGateway = new PagamentoGateway();
        var id = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
        listaItens.Add(itemPedido);
        var pedido = new Pedido(Guid.NewGuid(), listaItens);
        var pagamento = new Pagamento.Domain.Entities.Pagamento( pedido.Id,MetodoDePagamento.Pix, 100m, "idExterno");

        pagamentoGateway.AddPagamento(pagamento);

        // Act
        var pagamentoRetornado = await pagamentoGateway.GetByIdAsync(id);

        // Assert
        Assert.NotNull(pagamentoRetornado);
        Assert.Equal(id, pagamentoRetornado?.Id);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNullQuandoNaoExistir()
    {
        // Arrange
        var pagamentoGateway = new PagamentoGateway();
        var idInexistente = Guid.NewGuid();

        // Act
        var pagamentoRetornado = await pagamentoGateway.GetByIdAsync(idInexistente);

        // Assert
        Assert.Null(pagamentoRetornado);
    }

    [Fact]
    public async Task UpdatePagamentoAsync_DeveAtualizarPagamentoExistente()
    {
        // Arrange
        var pagamentoGateway = new PagamentoGateway();
        var id = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
        listaItens.Add(itemPedido);
        var pedido = new Pedido(Guid.NewGuid(), listaItens);
        var pagamento = new Pagamento.Domain.Entities.Pagamento( pedido.Id, MetodoDePagamento.Pix, 100m, "idExterno");
        pagamentoGateway.AddPagamento(pagamento);

        // Act
        var pagamentoAtualizado = await pagamentoGateway.UpdatePagamentoAsync(pagamento);

        // Assert
        Assert.NotNull(pagamentoAtualizado);
        Assert.Equal(StatusPagamento.Pendente, pagamentoAtualizado.Status);
    }

    [Fact]
    public async Task UpdatePagamentoAsync_DeveAdicionarPagamentoQuandoNaoExistir()
    {
        // Arrange
        var pagamentoGateway = new PagamentoGateway();
        var id = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var produto = new Produto("Lanche", "Lanche de bacon", 50m, "http://endereco/imagens/img.jpg", Guid.NewGuid());
        var itemPedido = new ItemDoPedido(Guid.NewGuid(), produto, 2);
        List<ItemDoPedido> listaItens = new List<ItemDoPedido>();
        listaItens.Add(itemPedido);
        var pedido = new Pedido(Guid.NewGuid(), listaItens);
        var pagamento = new Pagamento.Domain.Entities.Pagamento( pedido.Id, MetodoDePagamento.Pix, 100m, "idExterno");

        // Act
        var pagamentoAdicionado = await pagamentoGateway.UpdatePagamentoAsync(pagamento);

        // Assert
        Assert.NotNull(pagamentoAdicionado);
        Assert.Equal(id, pagamentoAdicionado.Id);
    }
}


public class PagamentoServiceTest
{
    private readonly IPagamentoGateway _pagamentoGateway;

    public PagamentoServiceTest(IPagamentoGateway pagamentoGateway)
    {
        _pagamentoGateway = pagamentoGateway;
    }

    public async Task<List<Pagamento.Domain.Entities.Pagamento>> BuscarPagamentosPorPedidoIdAsync(Guid pedidoId)
    {
        return await _pagamentoGateway.FindPagamentoByPedidoIdAsync(pedidoId);
    }

    public async Task<Pagamento.Domain.Entities.Pagamento?> BuscarPagamentoPorIdAsync(Guid id)
    {
        return await _pagamentoGateway.GetByIdAsync(id);
    }

    public async Task<Pagamento.Domain.Entities.Pagamento> AtualizarPagamentoAsync(Pagamento.Domain.Entities.Pagamento pagamento)
    {
        return await _pagamentoGateway.UpdatePagamentoAsync(pagamento);
    }
}


public class PagamentoGateway : IPagamentoGateway
{
    private readonly List<Pagamento.Domain.Entities.Pagamento> _pagamentos = new List<Pagamento.Domain.Entities.Pagamento>();

    public Task<List<Pagamento.Domain.Entities.Pagamento>> FindPagamentoByPedidoIdAsync(Guid pedidoId)
    {
        var pagamentos = _pagamentos.Where(p => p.PedidoId == pedidoId).ToList();
        return Task.FromResult(pagamentos);
    }

    public Task<Pagamento.Domain.Entities.Pagamento?> GetByIdAsync(Guid id)
    {
        var pagamento = _pagamentos.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(pagamento);
    }

    public Task<Pagamento.Domain.Entities.Pagamento> UpdatePagamentoAsync(Pagamento.Domain.Entities.Pagamento pagamento)
    {
        var existingPagamento = _pagamentos.FirstOrDefault(p => p.Id == pagamento.Id);
        _pagamentos.Add(pagamento);
        return Task.FromResult(pagamento);
    }

    public void AddPagamento(Pagamento.Domain.Entities.Pagamento pagamento)
    {
        _pagamentos.Add(pagamento);
    }
}
