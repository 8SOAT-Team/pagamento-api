using CleanArch.UseCase.Faults;
using CleanArch.UseCase;
using Moq;
using Pagamento.Adapters.Controllers;
using CleanArch.UseCase.Options;

namespace Tests.UnitTests.Adapters;
public class ControllerResultBuilderTests
{
    private readonly Mock<IUseCase> _mockUseCase;

    public ControllerResultBuilderTests()
    {
        _mockUseCase = new Mock<IUseCase>();
    }

    [Fact]
    public void WithResult_ShouldReturnCorrectResultWhenSuccess()
    {
        // Arrange
        var mockResult = Any<string>.Some("Success");
        _mockUseCase.Setup(x => x.IsFailure).Returns(false);
        _mockUseCase.Setup(x => x.GetErrors()).Returns(Array.Empty<UseCaseError>());

        var builder = ControllerResultBuilder<string, string>.ForUseCase(_mockUseCase.Object);

        // Act
        var result = builder.WithResult(mockResult).Build();

        // Assert
        Assert.NotNull(result);    
        Assert.Equal("Success", result.Value);
    }

    [Fact]
    public void WithResult_ShouldNotUpdateResultIfFailure()
    {
        // Arrange
        var mockResult = Any<string>.Some("Success");
        _mockUseCase.Setup(x => x.IsFailure).Returns(true);
        _mockUseCase.Setup(x => x.GetErrors()).Returns(new[] { new UseCaseError(UseCaseErrorType.BadRequest, "Bad Request Error") });

        var builder = ControllerResultBuilder<string, string>.ForUseCase(_mockUseCase.Object);

        // Act
        var result = builder.WithResult(mockResult).Build();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailure);

    }

    [Fact]
    public void WithInstance_ShouldSetInstanceWhenSuccess()
    {
        // Arrange
        var mockResult = Any<string>.Some("Success");
        _mockUseCase.Setup(x => x.IsFailure).Returns(false);
        _mockUseCase.Setup(x => x.GetErrors()).Returns(Array.Empty<UseCaseError>());
        var builder = ControllerResultBuilder<string, string>.ForUseCase(_mockUseCase.Object);
        // Act
        var result = builder.WithInstance("Success").Build();
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Value);
    }

    [Fact]
    public void AdaptUsing_ShouldApplyTransformation()
    {
        // Arrange
        var mockResult = Any<string>.Some("Success");
        _mockUseCase.Setup(x => x.IsFailure).Returns(false);
        _mockUseCase.Setup(x => x.GetErrors()).Returns(new[] { new UseCaseError(UseCaseErrorType.BadRequest, "Bad Request Error") });

        var builder = ControllerResultBuilder<string, string>.ForUseCase(_mockUseCase.Object);

        // Act
        var result = builder
            .WithResult(mockResult)
            .AdaptUsing(val => $"Transformed: {val}")
            .Build();

        // Assert
        Assert.NotNull(result);      
        Assert.Equal("Transformed: Success", result.Value);
    }

    [Fact]
    public void Build_ShouldReturnFailureResultIfUseCaseHasError()
    {
        // Arrange
        var error = new UseCaseError(UseCaseErrorType.InternalError, "Internal Server Error");
        _mockUseCase.Setup(x => x.IsFailure).Returns(true);
        _mockUseCase.Setup(x => x.GetErrors()).Returns(new[] { error });

        var builder = ControllerResultBuilder<string, string>.ForUseCase(_mockUseCase.Object);

        // Act
        var result = builder.Build();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailure);

    }

    [Fact]
    public void Build_ShouldReturnEmptyResultWhenNoValueAndNoFailure()
    {   // Arrange
        _mockUseCase.Setup(x => x.IsFailure).Returns(false);
        _mockUseCase.Setup(x => x.GetErrors()).Returns(Array.Empty<UseCaseError>());
        var builder = ControllerResultBuilder<string, string>.ForUseCase(_mockUseCase.Object);
        // Act
        var result = builder.Build();
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSucceed);
        Assert.Null(result.Value);

    }
}