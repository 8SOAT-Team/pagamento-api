using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Pagamentos.Adapters.Gateways;
using Pagamentos.Domain.Entities;
using Pagamentos.Infrastructure.Databases;
using Pagamentos.Infrastructure.Pagamentos;
using Pagamento.Adapters.Gateways;
using Pagamento.Domain.Entities;
using Pagamento.Infrastructure.Databases;
using System.Linq.Expressions;

namespace Tests.UnitTests.Adapters;
public class PagamentoGatewayTests
{
    private readonly Mock<PagamentoContext> _mockDbContext;
    private readonly PagamentoGateway _gateway;

    public PagamentoGatewayTests()
    {
        _mockDbContext = new Mock<PagamentoContext>();

        // Criando dados simulados
        var mockPagamentos = new List<Pagamentos.Domain.Entities.Pagamento>
    {
        new Pagamentos.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 150.0m, "idExterno1") { PedidoId = Guid.NewGuid() },
        new Pagamentos.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 200.0m, "idExterno2") { PedidoId = Guid.NewGuid() },
        new Pagamentos.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 300.0m, "idExterno3") { PedidoId = Guid.NewGuid() }
    }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Pagamentos.Domain.Entities.Pagamento>>();

        // Configurando o mock para suportar operações assíncronas
        mockDbSet.As<IQueryable<Pagamentos.Domain.Entities.Pagamento>>().Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<Pagamento.Domain.Entities.Pagamento>(mockPagamentos.Provider));

        mockDbSet.As<IQueryable<Pagamentos.Domain.Entities.Pagamento>>().Setup(m => m.Expression).Returns(mockPagamentos.Expression);
        mockDbSet.As<IQueryable<Pagamentos.Domain.Entities.Pagamento>>().Setup(m => m.ElementType).Returns(mockPagamentos.ElementType);
        mockDbSet.As<IQueryable<Pagamentos.Domain.Entities.Pagamento>>().Setup(m => m.GetEnumerator()).Returns(mockPagamentos.GetEnumerator());

        // Configurando suporte para IAsyncEnumerable
        mockDbSet.As<IAsyncEnumerable<Pagamento.Domain.Entities.Pagamento>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Pagamento.Domain.Entities.Pagamento>(mockPagamentos.GetEnumerator()));

        _mockDbContext.Setup(c => c.Pagamentos).Returns(mockDbSet.Object);

        _gateway = new PagamentoGateway(_mockDbContext.Object);
    }

    [Fact]
    public async Task FindPagamentoByPedidoIdAsync_ShouldReturnPagamentosForGivenPedidoId()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pagamento1 = new Pagamentos.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 150.0m, "idExterno1") { PedidoId = pedidoId };
        var pagamento2 = new Pagamentos.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 200.0m, "idExterno2") { PedidoId = pedidoId };
        var pagamento3 = new Pagamentos.Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Master, 300.0m, "idExterno3");

        var expectedPagamentos = new List<Pagamentos.Domain.Entities.Pagamento> { pagamento1, pagamento2 };

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
        var expectedPagamento = new Pagamentos.Domain.Entities.Pagamento(pagamentoId, MetodoDePagamento.Master, 150.0m, "idExterno1");

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
        var pagamentoToUpdate = new Pagamentos.Domain.Entities.Pagamento(pagamentoId, MetodoDePagamento.Master, 150.0m, "idExterno1");
        pagamentoToUpdate.FinalizarPagamento(true);
      

        _mockDbContext.Setup(c => c.Pagamentos.Update(It.IsAny<Pagamentos.Domain.Entities.Pagamento>())).Verifiable();
        _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _gateway.UpdatePagamentoAsync(pagamentoToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusPagamento.Autorizado, result.Status);
        _mockDbContext.Verify(c => c.Pagamentos.Update(It.IsAny<Pagamentos.Domain.Entities.Pagamento>()), Times.Once);
        _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}


public class TestAsyncQueryProvider<T> : IAsyncQueryProvider
{
    private readonly IQueryProvider _innerQueryProvider;

    public TestAsyncQueryProvider(IQueryProvider innerQueryProvider)
    {
        _innerQueryProvider = innerQueryProvider;
    }

    public IQueryable CreateQuery(Expression expression) =>
        new TestAsyncEnumerable<T>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
        new TestAsyncEnumerable<TElement>(expression);

    public object Execute(Expression expression) =>
        _innerQueryProvider.Execute(expression);

    public TResult Execute<TResult>(Expression expression) =>
        _innerQueryProvider.Execute<TResult>(expression);

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var expectedResultType = typeof(TResult).GetGenericArguments().FirstOrDefault();
        var executionResult = typeof(IQueryProvider)
            .GetMethods()
            .First(x => x.Name == nameof(IQueryProvider.Execute) && x.IsGenericMethod)
            .MakeGenericMethod(expectedResultType)
            .Invoke(_innerQueryProvider, new object[] { expression });

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))?
            .MakeGenericMethod(expectedResultType)
            .Invoke(null, new[] { executionResult });
    }
}

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _innerEnumerator;

    public TestAsyncEnumerator(IEnumerator<T> innerEnumerator)
    {
        _innerEnumerator = innerEnumerator;
    }

    public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_innerEnumerator.MoveNext());

    public T Current => _innerEnumerator.Current;

    public ValueTask DisposeAsync()
    {
        _innerEnumerator.Dispose();
        return ValueTask.CompletedTask;
    }
}