using Moq;
using Microsoft.Extensions.Logging;
using CleanArch.UseCase.Options;
using Pagamentos.Apps.UseCases;
using Pagamentos.Domain.Exceptions;

namespace Pagamentos.Tests.UnitTests.Application;

public class UseCaseExtensionTests
{

    [Fact]
    public async Task ResolveAsync_DeveAdicionarErroQuandoDomainExceptionValidationForLançada()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockUseCase = new Mock<UseCase<Empty<object>, SomeResult>>(mockLogger.Object);

        var exceptionMessage = "Erro de validação";
        var domainException = new DomainExceptionValidation(exceptionMessage);
        mockUseCase.Setup(x => x.ResolveAsync(It.IsAny<Empty<object>>()))
                   .ThrowsAsync(domainException);

        // Act
        var result = await mockUseCase.Object.ResolveAsync(It.IsAny<Empty<object>>());

        // Assert
        Assert.Null(result); // Como houve erro, o resultado será Empty
        mockUseCase.Verify(x => x.ResolveAsync(It.IsAny<Empty<object>>()), Times.Once); // Verifica se o método ResolveAsync foi chamado uma vez
        mockLogger.Verify(logger => logger.LogError(It.Is<string>(s => s.Contains(exceptionMessage)), It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task ResolveAsync_DeveAdicionarErroQuandoUmaExcecaoGenericaForLançada()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockUseCase = new Mock<UseCase<Empty<object>, SomeResult>>(mockLogger.Object);

        var exceptionMessage = "Erro interno";
        var genericException = new Exception(exceptionMessage);
        mockUseCase.Setup(x => x.ResolveAsync(It.IsAny<Empty<object>>()))
                   .ThrowsAsync(genericException);

        // Act
        var result = await mockUseCase.Object.ResolveAsync(It.IsAny<Empty<object>>());

        // Assert
        Assert.Null(result); // Como houve erro, o resultado será Empty
        mockUseCase.Verify(x => x.ResolveAsync(It.IsAny<Empty<object>>()), Times.Once); // Verifica se o método ResolveAsync foi chamado uma vez
        mockLogger.Verify(logger => logger.LogError(It.Is<string>(s => s.Contains(exceptionMessage)), It.IsAny<Exception>()), Times.Once);
    }
}

// Classe fictícia de retorno
public class SomeResult { }

// Simulação da classe Any<TOut> (para o teste)
public class Any<TOut> where TOut : class
{
    public TOut Value { get; }
    public static Any<TOut> Empty => new Any<TOut>(null);

    public Any(TOut value)
    {
        Value = value;
    }
}
