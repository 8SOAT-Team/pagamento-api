using Pagamento.Adapters.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pagamento.Tests.UnitTests.Adapters;
public class JsonSerializerExtensionTests
{
    [Fact]
    public void TryDeserialize_ShouldReturnSuccessWhenValidJson()
    {
        // Arrange
        var validJson = "{\"Name\": \"Test\", \"Age\": 30}";
        var expected = new Person { Name = "Test", Age = 30 };

        // Act
        var result = validJson.TryDeserialize<Person>();

        // Assert
        Assert.True(result.IsSucceed);
        Assert.NotNull(result.Value);
        Assert.Equal(expected.Name, result.Value.Name);
        Assert.Equal(expected.Age, result.Value.Age);
    }

    [Fact]
    public void TryDeserialize_ShouldReturnEmptyWhenInvalidJson()
    {
        // Arrange
        var invalidJson = "{\"Name\": \"Test\", \"Age\": \"InvalidAge\"}"; // Invalid age format

        // Act
        var result = invalidJson.TryDeserialize<Person>();

        // Assert
        Assert.True(result.IsSucceed);
    }

    [Fact]
    public void TryDeserialize_ShouldReturnEmptyWhenJsonIsMalformed()
    {
        // Arrange
        var malformedJson = "{\"Name\": \"Test\", \"Age\": 30"; // Missing closing brace

        // Act
        var result = malformedJson.TryDeserialize<Person>();

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void TryDeserialize_ShouldReturnEmptyWhenJsonThrowsException()
    {
        // Arrange
        var invalidJson = "InvalidJson";

        // Act
        var result = invalidJson.TryDeserialize<Person>();

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void TryDeserialize_ShouldReturnEmptyWhenNullJson()
    {
        // Arrange
        string? nullJson = null;

        // Act
        var result = nullJson.TryDeserialize<Person>();

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void TryDeserialize_ShouldHandleCustomJsonOptions()
    {
        // Arrange
        var json = "{\"Name\": \"Test\", \"Age\": 30}";
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        // Act
        var result = json.TryDeserialize<Person>(options);

        // Assert
        Assert.True(result.IsSucceed);
        Assert.Equal("Test", result.Value.Name);
        Assert.Equal(30, result.Value.Age);
    }

    // Additional helper class to test deserialization
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
