using Moq;
using Pagamentos.Adapters.Gateways;
using Pagamentos.Adapters.Types;
using Pagamentos.Infrastructure.Databases;

namespace Pagamentos.Tests.UnitTests.Adapters;
public class CacheGatewayTests
{
    private class TestCacheGateway : CacheGateway
    {
        public TestCacheGateway(ICacheContext cache) : base(cache) { }

        protected override Dictionary<string, (string cacheKey, bool InvalidateCacheOnChanges)> CacheKeys { get; } =
            new()
            {
                { "Key1", ("CacheKey1", true) },
                { "Key2", ("CacheKey2", false) },
                { "Key3", ("CacheKey3", true) }
            };

        internal async Task InvalidateCacheOnChange()
        {
            await InvalidateCacheOnChange(string.Empty);
        }

        internal async Task InvalidateCacheOnChange(string suffix)
        {
            throw new NotImplementedException();
        }
    }

    private readonly Mock<ICacheContext> _mockCache;
    private readonly TestCacheGateway _cacheGateway;

    public CacheGatewayTests()
    {
        _mockCache = new Mock<ICacheContext>();
        _cacheGateway = new TestCacheGateway(_mockCache.Object);
    }

    [Fact]
    public async Task InvalidateCacheOnChange_ShouldInvalidateOnlyKeysWithInvalidateCacheOnChangesTrue()
    {
        // Arrange
        _mockCache
            .Setup(c => c.InvalidateCacheAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<string>.Empty());

        // Act
        await _cacheGateway.InvalidateCacheOnChange();

        // Assert
        _mockCache.Verify(c => c.InvalidateCacheAsync("CacheKey1"), Times.Once);
        _mockCache.Verify(c => c.InvalidateCacheAsync("CacheKey3"), Times.Once);
        _mockCache.Verify(c => c.InvalidateCacheAsync("CacheKey2"), Times.Never);
    }

    [Fact]
    public async Task InvalidateCacheOnChange_ShouldAppendSuffix_WhenProvided()
    {
        // Arrange
        var suffix = "123";
        _mockCache
            .Setup(c => c.InvalidateCacheAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<string>.Empty());

        // Act
        await InvalidateCacheWithSuffix(suffix);

        // Assert
        _mockCache.Verify(c => c.InvalidateCacheAsync("CacheKey1:123"), Times.Once);
        _mockCache.Verify(c => c.InvalidateCacheAsync("CacheKey3:123"), Times.Once);
    }

    private Task InvalidateCacheWithSuffix(string suffix)
    {
        return InvalidateCacheOnChangeWithSuffix(suffix);
    }

    private Task InvalidateCacheOnChangeWithSuffix(string suffix)
    {
        return _cacheGateway.InvalidateCacheOnChange(suffix);
    }

    [Fact]
    public async Task InvalidateCacheOnChange_ShouldReturnCompletedTask_WhenNoKeysToInvalidate()
    {
        // Arrange
        var emptyCacheGateway = new TestCacheGatewayWithoutInvalidation(_mockCache.Object);

        // Act
        var result = await emptyCacheGateway.InvalidateCacheOnChange();

        // Assert
        Assert.Equal((IEnumerable<object>?)Task.CompletedTask, result);
        _mockCache.Verify(c => c.InvalidateCacheAsync(It.IsAny<string>()), Times.Never);
    }

    private class TestCacheGatewayWithoutInvalidation : CacheGateway
    {
        public TestCacheGatewayWithoutInvalidation(ICacheContext cache) : base(cache) { }

        protected override Dictionary<string, (string cacheKey, bool InvalidateCacheOnChanges)> CacheKeys { get; } =
            new()
            {
                { "Key1", ("CacheKey1", false) },
                { "Key2", ("CacheKey2", false) }
            };

        internal async Task<IEnumerable<object>> InvalidateCacheOnChange()
        {
            await InvalidateCacheOnChange(string.Empty);
            return (IEnumerable<object>)Task.CompletedTask;
        }
    }
}
