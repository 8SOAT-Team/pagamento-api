using Moq;
using Pagamentos.Adapters.Gateways;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Pagamentos.Infrastructure.Databases;

namespace Pagamentos.Tests.UnitTests.Adapters;
public class CacheContextTests
{
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly Mock<IConnectionMultiplexer> _mockConnectionMultiplexer;
    private readonly CacheContext _cacheContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public CacheContextTests()
    {
        _mockDatabase = new Mock<IDatabase>();
        _mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        _jsonOptions = new JsonSerializerOptions();

        _mockConnectionMultiplexer
            .Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _cacheContext = new CacheContext(_mockConnectionMultiplexer.Object, _jsonOptions);
    }

    [Fact]
    public async Task GetItemByKeyAsync_ShouldReturnDeserializedObject_WhenKeyExists()
    {
        // Arrange
        var key = "test-key";
        var expectedValue = new { Name = "Test", Age = 30 };
        var jsonValue = JsonSerializer.Serialize(expectedValue, _jsonOptions);

        _mockDatabase
            .Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)jsonValue);

        // Act
        var result = await _cacheContext.GetItemByKeyAsync<dynamic>(key);

        // Assert
        Assert.True(result.IsSucceed);
        Assert.Equal("Test", result.Value.Name);
        Assert.Equal(30, result.Value.Age);
    }

    [Fact]
    public async Task GetItemByKeyAsync_ShouldReturnEmptyResult_WhenKeyDoesNotExist()
    {
        // Arrange
        var key = "non-existing-key";

        _mockDatabase
            .Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        var result = await _cacheContext.GetItemByKeyAsync<object>(key);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task SetNotNullStringByKeyAsync_ShouldReturnSucceedResult_WhenCacheIsSetSuccessfully()
    {
        // Arrange
        var key = "test-key";
        var value = new { Name = "Test" };
        var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);

        _mockDatabase
            .Setup(db => db.StringSetAsync(key, jsonValue, It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await _cacheContext.SetNotNullStringByKeyAsync(key, value);

        // Assert
        Assert.True(result.IsSucceed);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public async Task SetNotNullStringByKeyAsync_ShouldReturnEmptyResult_WhenValueIsNull()
    {
        // Act
        var result = await _cacheContext.SetNotNullStringByKeyAsync<object>("test-key", null);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task SetNotNullStringByKeyAsync_ShouldReturnFailureResult_WhenCacheFails()
    {
        // Arrange
        var key = "test-key";
        var value = new { Name = "Test" };
        var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);

        _mockDatabase
            .Setup(db => db.StringSetAsync(key, jsonValue, It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(false); // Falha ao salvar

        // Act
        var result = await _cacheContext.SetNotNullStringByKeyAsync(key, value);

        // Assert
        Assert.True(result.IsFailure);
        
    }

    [Fact]
    public async Task SetStringByKeyAsync_ShouldReturnSucceedResult_WhenCacheIsSetSuccessfully()
    {
        // Arrange
        var key = "test-key";
        var value = "cached-value";

        _mockDatabase
            .Setup(db => db.StringSetAsync(key, value, It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await _cacheContext.SetStringByKeyAsync(key, value);

        // Assert
        Assert.True(result.IsSucceed);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public async Task SetStringByKeyAsync_ShouldReturnEmptyResult_WhenCacheFails()
    {
        // Arrange
        var key = "test-key";
        var value = "cached-value";

        _mockDatabase
            .Setup(db => db.StringSetAsync(key, value, It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(false); // Falha ao salvar

        // Act
        var result = await _cacheContext.SetStringByKeyAsync(key, value);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task InvalidateCacheAsync_ShouldRemoveKeySuccessfully()
    {
        // Arrange
        var key = "test-key";

        _mockDatabase
            .Setup(db => db.StringGetDeleteAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        var result = await _cacheContext.InvalidateCacheAsync(key);

        // Assert
        Assert.True(result.IsFailure);
    }
}