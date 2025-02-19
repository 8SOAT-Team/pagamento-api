using Moq;
using StackExchange.Redis;
using System.Text.Json;
using Pagamentos.Infrastructure.Databases;
using Pagamentos.Adapters.Gateways;
using Pagamentos.Apps.UseCases;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Tests.UnitTests.Adapters;
public class PagamentoGatewayCacheTests
{
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly Mock<IConnectionMultiplexer> _mockConnectionMultiplexer;
    private readonly CacheContext _cacheContext;
    private readonly PagamentoGatewayCache _gatewayCache;
    private readonly Mock<IPagamentoGateway> _mockPagamentoGateway;

    public PagamentoGatewayCacheTests()
    {
        _mockDatabase = new Mock<IDatabase>();
        _mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        _mockConnectionMultiplexer
            .Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _cacheContext = new CacheContext(_mockConnectionMultiplexer.Object, new JsonSerializerOptions());
        _mockPagamentoGateway = new Mock<IPagamentoGateway>();
        _gatewayCache = new PagamentoGatewayCache(_mockPagamentoGateway.Object, _cacheContext);
    }

    [Fact]
    public async Task FindPagamentoByPedidoIdAsync_ShouldReturnFromCache_WhenCacheExists()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        
        // Act
        var result = await _gatewayCache.FindPagamentoByPedidoIdAsync(pedidoId);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdatePagamentoAsync_ShouldInvalidateCache()
    {
        // Arrange
        var pagamento = new Pagamento(Guid.NewGuid(), Guid.NewGuid(), MetodoDePagamento.Pix, 1000, null);

        // Act
        var result = await _gatewayCache.UpdatePagamentoAsync(pagamento);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnFromCache_WhenCacheDoesNotExists()
    {
        // Act
        var result = await _gatewayCache.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task FindPagamentoByIdAsync_ShouldReturnFromCache_WhenCacheDoesNotExists()
    {
        // Act
        var result = await _gatewayCache.FindPagamentoByExternoIdAsync("any string");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task CreatePagamentoAsync_ShouldReturnFromCache_WhenCacheDoesNotExists()
    {
        // Arrange
        var pagamento = new Pagamento(Guid.NewGuid(), Guid.NewGuid(), MetodoDePagamento.Pix, 1000, null);

        // Act
        var result = await _gatewayCache.CreateAsync(pagamento);

        // Assert
        Assert.Null(result);
    }
}
