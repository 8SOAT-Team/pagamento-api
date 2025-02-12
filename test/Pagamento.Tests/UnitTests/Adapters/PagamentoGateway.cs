using Microsoft.EntityFrameworkCore;
using Moq;
using Pagamento.Adapters.Gateways;
using Pagamento.Domain.Entities;
using Pagamento.Infrastructure.Databases;

namespace Tests.UnitTests.Adapters;
public class PagamentoGatewayTests
{
    private readonly Mock<PagamentoContext> _mockDbContext;
    private readonly PagamentoGateway _gateway;

    public PagamentoGatewayTests()
    {
        // Mock DbContext and DbSet
        _mockDbContext = new Mock<PagamentoContext>();

        // Mock the Pagamentos DbSet
        var mockPagamentos = new List<Pagamento.Domain.Entities.Pagamento>
            {
                new Pagamento.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 150.0m, "idExterno1") { PedidoId = Guid.NewGuid() },
                new Pagamento.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 200.0m, "idExterno2") { PedidoId = Guid.NewGuid() },
                new Pagamento.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 300.0m, "idExterno3") { PedidoId = Guid.NewGuid() }
            }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Pagamento.Domain.Entities.Pagamento>>();
        mockDbSet.As<IQueryable<Pagamento.Domain.Entities.Pagamento>>().Setup(m => m.Provider).Returns(mockPagamentos.Provider);
        mockDbSet.As<IQueryable<Pagamento.Domain.Entities.Pagamento>>().Setup(m => m.Expression).Returns(mockPagamentos.Expression);
        mockDbSet.As<IQueryable<Pagamento.Domain.Entities.Pagamento>>().Setup(m => m.ElementType).Returns(mockPagamentos.ElementType);
        mockDbSet.As<IQueryable<Pagamento.Domain.Entities.Pagamento>>().Setup(m => m.GetEnumerator()).Returns(mockPagamentos.GetEnumerator());

        // Setup DbContext to return mock DbSet
        _mockDbContext.Setup(c => c.Pagamentos).Returns(mockDbSet.Object);

        // Initialize the gateway with the mocked DbContext
        _gateway = new PagamentoGateway(_mockDbContext.Object);
    }

    [Fact]
    public async Task FindPagamentoByPedidoIdAsync_ShouldReturnPagamentosForGivenPedidoId()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pagamento1 = new Pagamento.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 150.0m, "idExterno1") { PedidoId = pedidoId };
        var pagamento2 = new Pagamento.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 200.0m, "idExterno2") { PedidoId = pedidoId };
        var pagamento3 = new Pagamento.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 300.0m, "idExterno3");

        var expectedPagamentos = new List<Pagamento.Domain.Entities.Pagamento> { pagamento1, pagamento2 };

        // Act
        var result = await _gateway.FindPagamentoByPedidoIdAsync(pedidoId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Id == pagamento1.Id);
        Assert.Contains(result, p => p.Id == pagamento2.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPagamentoForGivenId()
    {
        // Arrange
        var pagamentoId = Guid.NewGuid();
        var expectedPagamento = new Pagamento.Domain.Entities.Pagamento(pagamentoId, MetodoDePagamento.Master, 150.0m, "idExterno1");

        // Act
        var result = await _gateway.GetByIdAsync(pagamentoId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPagamento.Id, result?.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullWhenPagamentoNotFound()
    {
        // Arrange
        var pagamentoId = Guid.NewGuid();

        // Act
        var result = await _gateway.GetByIdAsync(pagamentoId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePagamentoAsync_ShouldReturnUpdatedPagamento()
    {
        // Arrange
        var pagamentoId = Guid.NewGuid();
        var pagamentoToUpdate = new Pagamento.Domain.Entities.Pagamento(pagamentoId, MetodoDePagamento.Master, 150.0m, "idExterno1");
        pagamentoToUpdate.FinalizarPagamento(true);
      

        _mockDbContext.Setup(c => c.Pagamentos.Update(It.IsAny<Pagamento.Domain.Entities.Pagamento>())).Verifiable();
        _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _gateway.UpdatePagamentoAsync(pagamentoToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusPagamento.Autorizado, result.Status);
        _mockDbContext.Verify(c => c.Pagamentos.Update(It.IsAny<Pagamento.Domain.Entities.Pagamento>()), Times.Once);
        _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}